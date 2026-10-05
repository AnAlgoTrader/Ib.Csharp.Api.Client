using Ib.Csharp.Api.Client.Implementation;
using Ib.Csharp.Api.Client.Implementation.Helper;

namespace Ib.Csharp.Api.Client.Tests;

[TestFixture]
public class GetFootPrintTest
{
    [Test]
    public async Task ShouldBuildFootprint()
    {
        var footPrintHelper = new FootPrintHelper();
        using var client = new IbClient();
        client.OnNotification += Console.WriteLine;
        client.OnMarketDataPriceTickByTickUpdate += footPrintHelper.UpdatePrice;
        client.OnMarketDataTradeTickByTickUpdate += footPrintHelper.UpdateTrade;
        client.Start("127.0.0.1", 4001, 0);

        var contractDataList = await client.GetContractData("ES", "FUT");
        var contract = contractDataList.OrderBy(x => x.Contract.LastTradeDate).FirstOrDefault(x =>
            DateTime.ParseExact(x.Contract.LastTradeDate, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture) > DateTime.Now);
        client.NotifySubscriber($"Using contract - Exchange:{contract?.Contract.Exchange} LastDate:{contract?.Contract.LastTradeDate}");

        client.SubscribeToPriceTickByTick(1105, contract!.Contract);
        client.SubscribeToTradesTickByTick(1106, contract!.Contract);
    }
}