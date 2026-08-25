using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ib.Csharp.Api.Client.Implementation.Model;
using IBApi;

namespace Ib.Csharp.Api.Client.Implementation;

public partial class IbClient
{
    private readonly Dictionary<int, PriceUpdate> prices = new();
    private readonly Dictionary<int, TradeUpdate> trades = new();

    public Action<Dictionary<int, PriceUpdate>> OnMarketDataPriceUpdate { get; set; }
    public Action<Dictionary<int, TradeUpdate>> OnMarketDataTradeUpdate { get; set; }
    
    public Action<PriceTickByTickUpdate> OnMarketDataPriceTickByTickUpdate { get; set; }
    public Action<TradeTickByTickUpdate> OnMarketDataTradeTickByTickUpdate { get; set; }

    private OrderBookRow[] orderBook;
    public Action<OrderBookRow[]> OnOrderBookUpdate { get; set; }

    public void SubscribeToMarketData(int requestId, ContractDetails contractDetails)
    {
        if (!prices.ContainsKey(requestId))
        {
            prices.Add(requestId, new PriceUpdate());
            trades.Add(requestId, new TradeUpdate());
        }

        _clientSocket.reqMktData(requestId, contractDetails.Contract, "101", false, false, null);
    }

    public void SubscribeToMarketDepth(int requestId, ContractDetails contractDetails)
    {
        orderBook = new OrderBookRow[10];
        for (int i = 0; i < 10; i++)
            orderBook[i] = new OrderBookRow();

        _clientSocket.reqMarketDepth(requestId, contractDetails.Contract, 10, false, null);
    }

    public void UnsubscribeFromMarketData(int requestId)
    {
        _clientSocket.cancelMktData(requestId);
    }

    public void tickPrice(int tickerId, int field, double price, TickAttrib attribs)
    {
        prices[tickerId].RequestId = tickerId;
        prices[tickerId].setPrice(field, price);
        trades[tickerId].setPrice(field, price);
    }

    public void tickSize(int tickerId, int field, decimal size)
    {
        prices[tickerId].setSize(field, size);
        trades[tickerId].setSize(field, size);

        OnMarketDataPriceUpdate?.Invoke(prices);

        if (field == TickType.LAST_SIZE)
        {
            OnMarketDataTradeUpdate?.Invoke(trades);
        }
    }

    public Action<OptionComputation> OnOptionComputationUpdate { get; set; }

    public void tickOptionComputation(int tickerId, int field, int tickAttrib, double impliedVolatility, double delta,
        double optPrice, double pvDividend, double gamma, double vega, double theta, double undPrice)
    {
        var optionComputation = new OptionComputation
        {
            TickerId = tickerId,
            Field = field,
            TickAttrib = tickAttrib,
            ImpliedVolatility = impliedVolatility,
            Delta = delta,
            OptPrice = optPrice,
            PvDividend = pvDividend,
            Gamma = gamma,
            Vega = vega,
            Theta = theta,
            UndPrice = undPrice
        };
        OnOptionComputationUpdate?.Invoke(optionComputation);
    }

    public Action<List<Bar>> OnHistoricalDataUpdate { get; set; }
    private TaskCompletionSource<List<Bar>> historicalDataTask;
    private List<Bar> historicalDataList;
    private int _pendingHistoricalDataReqId = -1;
    private int _pendingHistoricalTickReqId = -1;

    private void OnHistoricalDataError(int id, int errorCode, string errorMsg)
    {
        var ex = new Exception($"IB Error {errorCode}: {errorMsg}");
        if (id == _pendingHistoricalDataReqId)
        {
            historicalDataTask?.TrySetException(ex);
            return;
        }
        if (id == _pendingHistoricalTickReqId)
        {
            historicalTickBidAskTask?.TrySetException(ex);
            historicalTickLastTask?.TrySetException(ex);
        }
    }

    /// <param name="reqId"></param>
    /// <param name="contract"></param>
    /// <param name="duration"></param>
    /// <param name="barSize"></param>
    /// <param name="whatToShow"></param>
    /// <param name="timeoutMs"></param>
    /// <param name="useRth"></param>
    /// Regular trading hours only. Defaults to true: outside-hours tape is thin and wide-spread,
    /// so it is rarely what a caller wants, and including it spends the request budget on data
    /// that mostly gets discarded. Pass false to get the extended session as well.
    public async Task<List<Bar>> GetHistoricalData(int reqId, Contract contract, string duration, string barSize,
        string whatToShow, int timeoutMs, bool useRth = true)
    {
        historicalDataList = [];
        historicalDataTask = new TaskCompletionSource<List<Bar>>();
        _pendingHistoricalDataReqId = reqId;

        _clientSocket.reqHistoricalData(reqId, contract, string.Empty, duration, barSize, whatToShow, useRth ? 1 : 0, 2, false, null);

        var timeoutTask = Task.Delay(timeoutMs);
        var completedTask = await Task.WhenAny(historicalDataTask.Task, timeoutTask);

        if (completedTask == timeoutTask)
            throw new TimeoutException($"GetHistoricalData timed out after {timeoutMs}ms");

        return await historicalDataTask.Task;
    }

    /// <summary>
    /// Bars for a window ending at <paramref name="endTime"/> rather than at the present.
    /// <see cref="GetHistoricalData"/> hardcodes the end to "now", which caps how far back a
    /// single request can see; supplying an end time is what makes paging further back
    /// possible. Times use the <c>yyyyMMdd-HH:mm:ss</c> form, which IBKR reads as UTC.
    /// </summary>
    /// <param name="reqId"></param>
    /// <param name="contract"></param>
    /// <param name="endTime"></param>
    /// <param name="duration"></param>
    /// <param name="barSize"></param>
    /// <param name="whatToShow"></param>
    /// <param name="timeoutMs"></param>
    /// <param name="useRth">Regular trading hours only. See <see cref="GetHistoricalData"/>.</param>
    public async Task<List<Bar>> GetHistoricalDataUntil(int reqId, Contract contract, string endTime,
        string duration, string barSize, string whatToShow, int timeoutMs, bool useRth = true)
    {
        historicalDataList = [];
        historicalDataTask = new TaskCompletionSource<List<Bar>>();
        _pendingHistoricalDataReqId = reqId;

        _clientSocket.reqHistoricalData(reqId, contract, endTime, duration, barSize, whatToShow, useRth ? 1 : 0, 2, false, null);

        var timeoutTask = Task.Delay(timeoutMs);
        var completedTask = await Task.WhenAny(historicalDataTask.Task, timeoutTask);

        if (completedTask == timeoutTask)
            throw new TimeoutException($"GetHistoricalDataUntil timed out after {timeoutMs}ms");

        return await historicalDataTask.Task;
    }

    public void historicalData(int reqId, Bar bar)
    {
        if (reqId == _pendingHistoricalDataReqId)
            historicalDataList.Add(bar);
    }

    public void historicalDataEnd(int reqId, string start, string end)
    {
        if (reqId != _pendingHistoricalDataReqId) return;
        OnHistoricalDataUpdate?.Invoke(historicalDataList);
        historicalDataTask?.TrySetResult(historicalDataList);
    }

    public Action<List<HistoricalTickBidAsk>> OnHistoricalTickBidAskUpdate { get; set; }
    private TaskCompletionSource<List<HistoricalTickBidAsk>> historicalTickBidAskTask;
    private List<HistoricalTickBidAsk> historicalTickBidAskList;
    
    /// <param name="reqId"></param>
    /// <param name="contract"></param>
    /// <param name="endTime"></param>
    /// <param name="timeoutMs"></param>
    /// <param name="useRth">Regular trading hours only. See <see cref="GetHistoricalData"/>.</param>
    public async Task<List<HistoricalTickBidAsk>> GetHistoricalTickBidAsk(int reqId, Contract contract, string endTime, int timeoutMs, bool useRth = true)
    {
        historicalTickBidAskList = [];
        historicalTickBidAskTask = new TaskCompletionSource<List<HistoricalTickBidAsk>>();
        _pendingHistoricalTickReqId = reqId;

        _clientSocket.reqHistoricalTicks(reqId, contract, string.Empty, endTime, 1000, "BID_ASK", useRth ? 1 : 0, true, null);

        var timeoutTask = Task.Delay(timeoutMs);
        var completedTask = await Task.WhenAny(historicalTickBidAskTask.Task, timeoutTask);

        if (completedTask == timeoutTask)
            throw new TimeoutException($"GetHistoricalTickBidAskTask timed out after {timeoutMs}ms");

        return await historicalTickBidAskTask.Task;
    }

    public void historicalTicksBidAsk(int reqId, HistoricalTickBidAsk[] ticks, bool done)
    {
        if (reqId != _pendingHistoricalTickReqId) return;
        historicalTickBidAskList.AddRange(ticks);

        if (done)
        {
            historicalTickBidAskList = historicalTickBidAskList.OrderBy(x => x.Time).ToList();
            OnHistoricalTickBidAskUpdate?.Invoke(historicalTickBidAskList);
            historicalTickBidAskTask?.TrySetResult(historicalTickBidAskList);
        }
    }

    public Action<List<HistoricalTickLast>> OnHistoricalTickLastUpdate { get; set; }
    private TaskCompletionSource<List<HistoricalTickLast>> historicalTickLastTask;
    private List<HistoricalTickLast> historicalTickLastList;
   
    /// <param name="reqId"></param>
    /// <param name="contract"></param>
    /// <param name="endTime"></param>
    /// <param name="timeoutMs"></param>
    /// <param name="useRth">Regular trading hours only. See <see cref="GetHistoricalData"/>.</param>
    public async Task<List<HistoricalTickLast>> GetHistoricalTickLast(int reqId, Contract contract, string endTime, int timeoutMs, bool useRth = true)
    {
        historicalTickLastList = [];
        historicalTickLastTask = new TaskCompletionSource<List<HistoricalTickLast>>();
        _pendingHistoricalTickReqId = reqId;
        _clientSocket.reqHistoricalTicks(reqId, contract, string.Empty, endTime, 1000, "TRADES", useRth ? 1 : 0, true, null);

        var timeoutTask = Task.Delay(timeoutMs);
        var completedTask = await Task.WhenAny(historicalTickLastTask.Task, timeoutTask);

        if (completedTask == timeoutTask)
            throw new TimeoutException($"GetHistoricalTickLastTask timed out after {timeoutMs}ms");

        return await historicalTickLastTask.Task;
    }

    /// <summary>
    /// <param name="startTime"></param>
    /// Trades from <paramref name="startTime"/> forward, rather than backward from an end time.
    /// IBKR requires exactly one of startDateTime / endDateTime, so this is the mirror of
    /// <see cref="GetHistoricalTickLast"/> and not a windowed variant of it — the two cannot be
    /// combined. Times use the <c>yyyyMMdd-HH:mm:ss</c> form, which IBKR reads as UTC.
    /// </summary>
    /// <param name="reqId"></param>
    /// <param name="contract"></param>
    /// <param name="startTime"></param>
    /// <param name="timeoutMs"></param>
    /// <param name="useRth">
    /// Regular trading hours only. See <see cref="GetHistoricalData"/>. Note this filters the
    /// hours, not the day — walking forward past a session close continues into the next
    /// session's regular hours, so a caller wanting one session must still bound the end itself.
    /// </param>
    
    public async Task<List<HistoricalTickLast>> GetHistoricalTickLastFrom(int reqId, Contract contract,
        string startTime, int timeoutMs, bool useRth = true)
    {
        historicalTickLastList = [];
        historicalTickLastTask = new TaskCompletionSource<List<HistoricalTickLast>>();
        _pendingHistoricalTickReqId = reqId;

        _clientSocket.reqHistoricalTicks(reqId, contract, startTime, string.Empty, 1000, "TRADES", useRth ? 1 : 0, true, null);

        var timeoutTask = Task.Delay(timeoutMs);
        var completedTask = await Task.WhenAny(historicalTickLastTask.Task, timeoutTask);

        if (completedTask == timeoutTask)
            throw new TimeoutException($"GetHistoricalTickLastFrom timed out after {timeoutMs}ms");

        return await historicalTickLastTask.Task;
    }

    public void historicalTicksLast(int reqId, HistoricalTickLast[] ticks, bool done)
    {
        if (reqId != _pendingHistoricalTickReqId) return;
        historicalTickLastList.AddRange(ticks);

        if (done)
        {
            historicalTickLastList = historicalTickLastList.OrderBy(x => x.Time).ToList();
            OnHistoricalTickLastUpdate?.Invoke(historicalTickLastList);
            historicalTickLastTask?.TrySetResult(historicalTickLastList);
        }
    }

    // position is the index row in the book
    // operation - insert(0), update(1), remove(2)
    // side - ask(0), bid(1)
    public void updateMktDepth(int tickerId, int position, int operation, int side, double price, decimal size)
    {
        var row = orderBook[position];

        if (operation == 2) // remove
        {
            if (side == 1) // bid
            {
                row.BidPrice = 0;
                row.BidSize = 0;
            }
            else // ask
            {
                row.AskPrice = 0;
                row.AskSize = 0;
            }
        }
        else // insert or update
        {
            if (side == 1) // bid
            {
                row.BidPrice = price;
                row.BidSize = size;
            }
            else // ask
            {
                row.AskPrice = price;
                row.AskSize = size;
            }
        }

        OnOrderBookUpdate?.Invoke(orderBook);
    }

    public void updateMktDepthL2(int tickerId, int position, string marketMaker, int operation, int side, double price,
        decimal size, bool isSmartDepth)
    {
        updateMktDepth(tickerId, position, operation, side, price, size);
    }

    public void SubscribeToPriceTickByTick(int reqId, Contract contract)
    {
        _clientSocket.reqTickByTickData(reqId, contract, "BidAsk", 0, true);
    }
    
    public void tickByTickBidAsk(int reqId, long time, double bidPrice, double askPrice, decimal bidSize, decimal askSize,
        TickAttribBidAsk tickAttribBidAsk)
    {
        var tickByTickUpdate = new PriceTickByTickUpdate
        {
            ReqId = reqId,
            Time = time,
            Bid = bidPrice,
            Ask = askPrice,
            BidSize = bidSize,
            AskSize = askSize,
            TickAttribBidAsk = tickAttribBidAsk
        };
        OnMarketDataPriceTickByTickUpdate?.Invoke(tickByTickUpdate);
    }
    
    public void SubscribeToTradesTickByTick(int reqId, Contract contract)
    {
        _clientSocket.reqTickByTickData(reqId, contract, "Last", 0, true);
    }
    
    public void tickByTickAllLast(int reqId, int tickType, long time, double price, decimal size, TickAttribLast tickAttribLast,
        string exchange, string specialConditions)
    {
        var tickByTickUpdate = new TradeTickByTickUpdate
        {
            ReqId = reqId,
            TickType = tickType,
            Time = time,
            Price = price,
            Size = size,
            TickAttribLast = tickAttribLast,
            Exchange = exchange,
            SpecialConditions = specialConditions
        };
        OnMarketDataTradeTickByTickUpdate?.Invoke(tickByTickUpdate);
    }
}