using Ib.Csharp.Api.Client.Implementation;
using Ib.Csharp.Api.Client.Implementation.Helper;
using Ib.Csharp.Api.Client.Implementation.Model;
using IBApi;

namespace Ib.Csharp.Api.Client.Tests;

[TestFixture]
public class SendOrderTest
{
    /// <summary>
    /// This test sends orders to your account, test it against a PAPER account
    /// </summary>
    [Test]
    public async Task ShouldSendOrders()
    {
        using var client = new IbClient();
        client.OnNotification += Console.WriteLine;
        client.Start("127.0.0.1", 4001, 0);
        
        client.OnOrderStatusUpdate += statusUpdate =>
        {
            client.NotifySubscriber($"OrderId:{statusUpdate.OrderId} Status:{statusUpdate.Status}");
        };
        client.OnExecutionUpdate += executionDetails =>
        {
            client.NotifySubscriber($"Execution details - Price:{executionDetails.Execution.AvgPrice}");
        };
        client.OnCommissionAndFeesUpdate += report =>
        {
            client.NotifySubscriber($"Commission:{report.CommissionAndFees}");
        };
        
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
            Tif = "DAY",
            Transmit = true
        };
        client.PlaceOrder(contract?.Contract, order);
        await AsyncHelper.Wait(5000);
    }
}