using Ib.Csharp.Api.Client.Implementation;
using Ib.Csharp.Api.Client.Implementation.Helper;

namespace Ib.Csharp.Api.Client.Tests;

[TestFixture]
public class MarketDataUpdatesTest
{
    [Test]
    public async Task ShouldReceiveMarketData()
    {
        using var client = new IbClient();
        client.OnNotification += Console.WriteLine;
        client.Start("127.0.0.1", 4001, 0);
        const int requestId = 1005;
        client.OnMarketDataPriceUpdate += marketDataUpdate =>
        {
            Console.WriteLine($"Price --> Bid:{marketDataUpdate.First().Value.BidPrice} Ask:{marketDataUpdate.First().Value.AskPrice}");
        };
        client.OnMarketDataTradeUpdate += marketTradeUpdate =>
        {
            Console.WriteLine($"Trade --> Price:{marketTradeUpdate.First().Value.LastTradedPrice} Size:{marketTradeUpdate.First().Value.LastTradedSize}");
        };
        
        var contractDataList = await client.GetContractData("ES", "FUT");
        var contract = contractDataList.OrderBy(x=> x.Contract.LastTradeDate).FirstOrDefault(x=> 
            DateTime.ParseExact(x.Contract.LastTradeDate, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture) > DateTime.Now);
        client.NotifySubscriber($"Using contract - Exchange:{contract?.Contract.Exchange} LastDate:{contract?.Contract.LastTradeDate}");
        
        client.SubscribeToMarketData(requestId, contract);
        await AsyncHelper.Wait(10000);
        client.UnsubscribeFromMarketData(requestId);
    }
}