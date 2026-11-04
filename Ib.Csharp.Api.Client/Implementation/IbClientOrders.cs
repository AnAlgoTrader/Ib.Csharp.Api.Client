using System;
using Ib.Csharp.Api.Client.Implementation.Model;
using IBApi;

namespace Ib.Csharp.Api.Client.Implementation;

public partial class IbClient
{
    private int _nextOrderId { get; set; }

    public void nextValidId(int orderId)
    {
        _nextOrderId = orderId;
    }

    public int GetNextValidId()
    {
        var orderId = _nextOrderId;
        _nextOrderId++;
        return orderId;
    }

    public void PlaceOrder(Contract contract, Order order)
    {
        _clientSocket.placeOrder(order.OrderId, contract, order);
    }

    public void WhatIf(Contract contract, Order order)
    {
        order.WhatIf = true;
        order.Transmit = true;
        _clientSocket.placeOrder(order.OrderId, contract, order);
    }

    public void CancelOrder(int orderId)
    {
        _clientSocket.cancelOrder(orderId, new OrderCancel());
    }

    public void PlaceCreditSpreadOrder(Contract underlying, int shortConId, int longConId, decimal quantity, double shortBid, double longAsk)
    {
        var contract = new Contract
        {
            Symbol = underlying.Symbol,
            SecType = "BAG",
            Currency = underlying.Currency,
            Exchange = underlying.Exchange
        };

        var shortLeg = new ComboLeg
        {
            ConId = shortConId,
            Ratio = 1,
            Action = "SELL",
            Exchange = underlying.Exchange
        };

        var longLeg = new ComboLeg
        {
            ConId = longConId,
            Ratio = 1,
            Action = "BUY",
            Exchange = underlying.Exchange
        };

        contract.ComboLegs = [shortLeg, longLeg];

        double credit = shortBid - longAsk;
        var order = new Order
        {
            OrderId = GetNextValidId(),
            Action = "BUY",
            Tif = "DAY",
            OrderType = "LMT",
            TotalQuantity = quantity,
            LmtPrice = -Math.Round(credit, 2),
            OutsideRth = true,
            Transmit = true
        };

        PlaceOrder(contract, order);
    }

    public Action<ExecutionDetails> OnExecutionUpdate { get; set; }

    public void execDetails(int reqId, Contract contract, Execution execution)
    {
        var executionDetails = new ExecutionDetails
        {
            RequestId = reqId,
            Contract = contract,
            Execution = execution
        };
        OnExecutionUpdate?.Invoke(executionDetails);
    }

    public Action<CommissionAndFeesReport> OnCommissionAndFeesUpdate { get; set; }

    public void commissionAndFeesReport(CommissionAndFeesReport commissionAndFeesReport)
    {
        OnCommissionAndFeesUpdate?.Invoke(commissionAndFeesReport);
    }

    public Action<OpenOrderUpdate> OnOpenOrderUpdate { get; set; }

    public void openOrder(int orderId, Contract contract, Order order, OrderState orderState)
    {
        var openOrderUpdate = new OpenOrderUpdate
        {
            OrderId = orderId,
            Contract = contract,
            Order = order,
            OrderState = orderState
        };
        OnOpenOrderUpdate?.Invoke(openOrderUpdate);
    }

    public Action<OrderStatus> OnOrderStatusUpdate { get; set; }

    public void orderStatus(int orderId, string status, decimal filled, decimal remaining, double avgFillPrice, long permId,
        int parentId, double lastFillPrice, int clientId, string whyHeld, double mktCapPrice)
    {
        var orderStatus = new OrderStatus
        {
            OrderId = orderId,
            Status = status,
            Filled = filled,
            Remaining = remaining,
            AvgFillPrice = avgFillPrice,
            PermId = permId,
            ParentId = parentId,
            LastFillPrice = lastFillPrice,
            ClientId = clientId,
            WhyHeld = whyHeld,
            MktCapPrice = mktCapPrice
        };
        OnOrderStatusUpdate?.Invoke(orderStatus);
    }
}