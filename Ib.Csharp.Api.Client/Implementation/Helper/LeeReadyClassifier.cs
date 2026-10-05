using System;

namespace Ib.Csharp.Api.Client.Implementation.Helper;

public enum TradeSide
{
    Unknown,
    Buy,
    Sell
}

public readonly struct TimestampedQuote
{
    public long Time { get; }
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
    public long Time { get; }
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
/// Real-time in-stream Lee-Ready trade classifier.
///
/// Operates directly on incoming stream events without artificial buffering or coarse
/// 1-second timestamp lookahead bias.
///
/// 1. Quote Rule: Compares trade price against prevailing pre-trade quote (Bid/Ask).
///    - Price >= Ask => Aggressive Buy (buyer lifted offer)
///    - Price <= Bid => Aggressive Sell (seller hit bid)
///    - Inside spread (Bid < Price < Ask):
///        * Price > Midpoint => Buy
///        * Price < Midpoint => Sell
///        * Price == Midpoint => Falls back to Tick Rule
///    - Quote transition safeguard: if the book stepped just milliseconds before the trade arrived
///      on socket, trades matching the immediate prior opposite side are correctly assigned.
/// 2. Tick Rule:
///    - Price > LastTradedPrice => Uptick (Buy)
///    - Price < LastTradedPrice => Downtick (Sell)
///    - Price == LastTradedPrice => Zero-tick (inherits direction of last non-zero price change)
/// </summary>
public class LeeReadyClassifier
{
    private double _currentBid = double.NaN;
    private double _currentAsk = double.NaN;
    private double _prevBid = double.NaN;
    private double _prevAsk = double.NaN;

    private double _lastTradedPrice = double.NaN;
    private TradeSide _lastTickDirection = TradeSide.Unknown;

    public event Action<ClassifiedTrade> OnTradeClassified;

    public double CurrentBid => _currentBid;
    public double CurrentAsk => _currentAsk;
    public double LastTradedPrice => _lastTradedPrice;
    public TradeSide LastTickDirection => _lastTickDirection;

    public void OnQuote(long time, double bid, double ask)
    {
        if (bid <= 0 || ask <= 0) return;

        if (!double.IsNaN(_currentBid) && !double.IsNaN(_currentAsk))
        {
            _prevBid = _currentBid;
            _prevAsk = _currentAsk;
        }

        _currentBid = bid;
        _currentAsk = ask;
    }

    public void OnTrade(long time, double price, decimal size)
    {
        var side = Classify(price);
        OnTradeClassified?.Invoke(new ClassifiedTrade(time, price, size, side));
    }

    public TradeSide Classify(double price)
    {
        var side = DetermineSide(price);

        // Update tick direction based on actual price movement (non-zero price change)
        if (!double.IsNaN(_lastTradedPrice))
        {
            if (price > _lastTradedPrice)
                _lastTickDirection = TradeSide.Buy;
            else if (price < _lastTradedPrice)
                _lastTickDirection = TradeSide.Sell;
        }
        else if (side != TradeSide.Unknown)
        {
            _lastTickDirection = side;
        }

        _lastTradedPrice = price;

        return side;
    }

    private TradeSide DetermineSide(double price)
    {
        bool hasValidQuote = !double.IsNaN(_currentBid) && !double.IsNaN(_currentAsk) && _currentAsk > _currentBid;

        if (hasValidQuote)
        {
            // 1. At or above Ask -> Aggressive Buy
            if (price >= _currentAsk)
            {
                return TradeSide.Buy;
            }

            // 2. At or below Bid -> Aggressive Sell
            if (price <= _currentBid)
            {
                // Edge case: Quote moved up right before the trade message arrived on socket.
                // e.g. Quote was 5000.00 / 5000.25. Trade was at 5000.25 (swept the ask).
                // If quote update arrived just prior: 5000.25 / 5000.50.
                // In new quote, 5000.25 is Bid. But it was prevAsk, and quote stepped up!
                if (!double.IsNaN(_prevAsk) && Math.Abs(price - _prevAsk) < 1e-4 && _currentAsk > _prevAsk)
                {
                    return TradeSide.Buy;
                }

                return TradeSide.Sell;
            }

            // Edge case: Quote moved down right before the trade message arrived on socket.
            if (!double.IsNaN(_prevBid) && Math.Abs(price - _prevBid) < 1e-4 && _currentBid < _prevBid)
            {
                return TradeSide.Sell;
            }

            // 3. Inside the spread: compare to midpoint
            double mid = (_currentBid + _currentAsk) / 2.0;
            if (price > mid)
            {
                return TradeSide.Buy;
            }
            if (price < mid)
            {
                return TradeSide.Sell;
            }

            // Exactly at midpoint -> fall back to tick rule
        }

        // Tick Rule fallback:
        return TickRule(price);
    }

    private TradeSide TickRule(double price)
    {
        if (double.IsNaN(_lastTradedPrice))
        {
            return _lastTickDirection != TradeSide.Unknown ? _lastTickDirection : TradeSide.Unknown;
        }

        if (price > _lastTradedPrice)
        {
            return TradeSide.Buy;
        }

        if (price < _lastTradedPrice)
        {
            return TradeSide.Sell;
        }

        // Zero-tick: inherit direction of last non-zero price change
        return _lastTickDirection;
    }

    public void Pump() { }
    public void Flush() { }
}
