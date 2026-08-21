using Ib.Csharp.Api.Client.Implementation;
using Ib.Csharp.Api.Client.Implementation.Helper;
using Ib.Csharp.Api.Client.Implementation.Model;
using IBApi;

namespace Ib.Csharp.Api.Client.Tests;

public class WhatIfTest
{
    [Test]
    public async Task ShouldGetAWhatIfReport()
    {
        using var client = new IbClient();
        client.OnNotification += Console.WriteLine;
        client.Start("127.0.0.1", 4001, 0);
        
        var contractDataList = await client.GetContractData("ES", "FUT");
        var contract = contractDataList.OrderBy(x => x.Contract.LastTradeDate).FirstOrDefault(x =>
            DateTime.ParseExact(x.Contract.LastTradeDate, "yyyyMMdd",
                System.Globalization.CultureInfo.InvariantCulture) > DateTime.Now);
        client.NotifySubscriber(
            $"Using contract - Exchange:{contract?.Contract.Exchange} LastDate:{contract?.Contract.LastTradeDate}");

        client.SubscribeToMarketData(1050, contract);

        var marketDataUpdates = await AsyncHelper.WaitForCallback<Dictionary<int, PriceUpdate>>(
            subscribe => client.OnMarketDataPriceUpdate += subscribe,
            10000);
        client.UnsubscribeFromMarketData(1050);

        client.NotifySubscriber($"Market snapshot Bid:{marketDataUpdates.First().Value.BidPrice}");

        var order = new Order
        {
            OrderId = client.GetNextValidId(),
            Action = "SELL",
            TotalQuantity = 1,
            OrderType = "LMT",
            LmtPrice = marketDataUpdates.First().Value.BidPrice,
            Tif = "DAY"
        };
        
        var whatIfTask = AsyncHelper.WaitForCallback<OpenOrderUpdate>(
            subscribe => client.OnOpenOrderUpdate += subscribe,
            10000);

        client.WhatIf(contract?.Contract, order);
        var openOrderUpdate = await whatIfTask;

        client.NotifySubscriber($"WhatIf --> InitMargin:{openOrderUpdate.OrderState.InitMarginChange} MaintMargin:{openOrderUpdate.OrderState.MaintMarginChange} Commission:{openOrderUpdate.OrderState.CommissionAndFees}");
    }
}