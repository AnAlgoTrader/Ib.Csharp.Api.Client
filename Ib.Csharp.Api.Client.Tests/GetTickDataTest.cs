using Ib.Csharp.Api.Client.Implementation;
using Ib.Csharp.Api.Client.Implementation.Helper;

namespace Ib.Csharp.Api.Client.Tests;

[TestFixture]
public class GetTickDataTest
{
    [Test]
    public async Task ShouldReceiveTickByTickData()
    {
        using var client = new IbClient();
        client.OnNotification += Console.WriteLine;
        //client.OnMarketDataPriceTickByTickUpdate += update => { Console.WriteLine($"Price -> Bid:{update.Bid}"); };
        client.OnMarketDataTradeTickByTickUpdate += update => { Console.WriteLine($"Trade -> Price:{update.Price} Size:{update.Size}"); };
        client.Start("127.0.0.1", 4001, 0);

        var contractDataList = await client.GetContractData("ES", "FUT");
        var contract = contractDataList.OrderBy(x => x.Contract.LastTradeDate).FirstOrDefault(x =>
            DateTime.ParseExact(x.Contract.LastTradeDate, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture) > DateTime.Now);
        client.NotifySubscriber($"Using contract - Exchange:{contract?.Contract.Exchange} LastDate:{contract?.Contract.LastTradeDate}");

        //client.SubscribeToPriceTickByTick(1105, contract!.Contract);
        client.SubscribeToTradesTickByTick(1105, contract!.Contract);

        await AsyncHelper.Wait(10000);
    }
}