using System;
using System.Collections.Generic;

namespace Ib.Csharp.Api.Client.Implementation.Helper;

public enum TradeSide
{
    Unknown,
    Buy,
    Sell
}

/// <summary>
/// A quote timestamped in Unix epoch seconds, matching IBKR's tick-by-tick
/// `time` field (long). Resolution is whatever IBKR provides -- typically
/// whole seconds -- so many quotes/trades may legitimately share a Time.
/// </summary>
public readonly struct TimestampedQuote
{
    public long Time { get; } // Unix epoch seconds, as given by IBKR
    public double Bid { get; }
    public double Ask { get; }

    public TimestampedQuote(long time, double bid, double ask)
    {
        Time = time;
        Bid = bid;
        Ask = ask;
    }

    public double Mid => (Bid + Ask) / 2.0;
}

public readonly struct ClassifiedTrade
{
    public long Time { get; } // Unix epoch seconds
    public double Price { get; }
    public decimal Size { get; }
    public TradeSide Side { get; }

    public ClassifiedTrade(long time, double price, decimal size, TradeSide side)
    {
        Time = time;
        Price = price;
        Size = size;
        Side = side;
    }
}

/// <summary>
/// Lee-Ready classifier with a buffering window to guard against trade/quote
/// ticks arriving out of true chronological order over separate IBKR streams.
///
/// Timestamps come in as raw Unix epoch seconds (long), matching IBKR's
/// tick-by-tick API directly -- no DateTime conversion needed on the hot path.
///
/// Two mechanisms work together:
///   1. Quotes are inserted into history ordered by their exchange Time
///      (long seconds), not by arrival order, so a late-arriving-but-earlier
///      quote still lands in the right place. Because resolution is only
///      whole seconds, this only corrects ordering ACROSS seconds -- ticks
///      sharing the same second keep arrival order between themselves,
///      since that's the only ordering information available.
///   2. Trades are not classified the instant they arrive -- they sit in a
///      queue for `bufferDelay` (real wall-clock time) so any quote still
///      in flight has a chance to arrive and be inserted correctly first.
///
/// This trades a small, fixed amount of classification latency for
/// materially better accuracy. Fine for building bars/indicators; not
/// intended for anything requiring sub-buffer-delay decisions.
/// </summary>
public class BufferedLeeReadyClassifier
{
    private readonly TimeSpan _bufferDelay;
    private readonly long _quoteRetentionSeconds;

    private readonly List<TimestampedQuote> _quotes = new();
    private readonly Queue<(long Time, double Price, decimal Size, DateTime Deadline)> _pendingTrades = new();

    private double _lastTradedPrice = double.NaN;
    private TradeSide _lastTradeSide = TradeSide.Unknown;

    public event Action<ClassifiedTrade> OnTradeClassified;

    /// <param name="bufferDelay">
    /// How long (real time) to hold a trade before classifying it. Start
    /// around 100-250ms for IBKR's consolidated/SMART feed and tune
    /// empirically (see note below).
    /// </param>
    /// <param name="quoteRetentionSeconds">
    /// How many seconds of quote history to keep. Only needs to exceed
    /// bufferDelay by a comfortable margin -- 10s is a generous default.
    /// </param>
    public BufferedLeeReadyClassifier(TimeSpan? bufferDelay = null, long quoteRetentionSeconds = 10)
    {
        _bufferDelay = bufferDelay ?? TimeSpan.FromMilliseconds(250);
        _quoteRetentionSeconds = quoteRetentionSeconds;
    }

    /// <summary>Call from your tickByTickBidAsk handler. `time` is IBKR's raw long.</summary>
    public void OnQuote(long time, double bid, double ask)
    {
        InsertQuoteInOrder(new TimestampedQuote(time, bid, ask));
        TrimQuoteHistory(time);
    }

    /// <summary>Call from your tickByTickAllLast handler. `time` is IBKR's raw long, `size` is IBKR's decimal.</summary>
    public void OnTrade(long time, double price, decimal size)
    {
        var deadline = DateTime.UtcNow + _bufferDelay; // wall clock -- our own processing delay
        _pendingTrades.Enqueue((time, price, size, deadline));
    }

    /// <summary>
    /// Flushes any trades whose buffer window has elapsed. Call this
    /// frequently -- e.g. once at the top of every tick handler, or off
    /// a short interval timer (10-20ms) if tick flow is sparse.
    /// </summary>
    public void Pump()
    {
        var now = DateTime.UtcNow;

        while (_pendingTrades.Count > 0 && _pendingTrades.Peek().Deadline <= now)
        {
            var trade = _pendingTrades.Dequeue();
            var side = Classify(trade.Time, trade.Price);
            if (side != TradeSide.Unknown)
            {
                _lastTradeSide = side;
            }
            _lastTradedPrice = trade.Price;
            OnTradeClassified?.Invoke(new ClassifiedTrade(trade.Time, trade.Price, trade.Size, side));
        }
    }

    /// <summary>
    /// Flushes all pending trades immediately without waiting for the buffer window.
    /// </summary>
    public void Flush()
    {
        while (_pendingTrades.Count > 0)
        {
            var trade = _pendingTrades.Dequeue();
            var side = Classify(trade.Time, trade.Price);
            if (side != TradeSide.Unknown)
            {
                _lastTradeSide = side;
            }
            _lastTradedPrice = trade.Price;
            OnTradeClassified?.Invoke(new ClassifiedTrade(trade.Time, trade.Price, trade.Size, side));
        }
    }

    private TradeSide Classify(long tradeTime, double price)
    {
        var quote = GetQuoteAsOf(tradeTime);

        if (quote.HasValue && quote.Value.Bid > 0 && quote.Value.Ask > 0)
        {
            double mid = quote.Value.Mid;
            if (price > mid) return TradeSide.Buy;
            if (price < mid) return TradeSide.Sell;
            // exactly at midpoint -> fall back to the tick rule
        }

        return TickRule(price);
    }

    private TradeSide TickRule(double price)
    {
        if (double.IsNaN(_lastTradedPrice)) return TradeSide.Unknown;
        if (price > _lastTradedPrice) return TradeSide.Buy;
        if (price < _lastTradedPrice) return TradeSide.Sell;
        return _lastTradeSide; // zero-tick rule: maintain previous tick direction
    }

    // Latest quote with Time <= tradeTime. Binary search since _quotes
    // stays sorted by exchange time (seconds).
    private TimestampedQuote? GetQuoteAsOf(long tradeTime)
    {
        int lo = 0, hi = _quotes.Count - 1;
        int found = -1;

        while (lo <= hi)
        {
            int mid = (lo + hi) / 2;
            if (_quotes[mid].Time <= tradeTime)
            {
                found = mid;
                lo = mid + 1;
            }
            else
            {
                hi = mid - 1;
            }
        }

        return found >= 0 ? _quotes[found] : (TimestampedQuote?)null;
    }

    // Insert keeping ascending order by exchange time (seconds). Equal
    // timestamps are kept in arrival order (stable) -- the best ordering
    // info available when multiple ticks share the same second.
    private void InsertQuoteInOrder(TimestampedQuote quote)
    {
        int idx = _quotes.Count;
        while (idx > 0 && _quotes[idx - 1].Time > quote.Time)
            idx--;
        _quotes.Insert(idx, quote);
    }

    private void TrimQuoteHistory(long latestTime)
    {
        long cutoff = latestTime - _quoteRetentionSeconds;
        int i = 0;
        while (i < _quotes.Count && _quotes[i].Time < cutoff)
            i++;
        if (i > 0)
            _quotes.RemoveRange(0, i);
    }
}