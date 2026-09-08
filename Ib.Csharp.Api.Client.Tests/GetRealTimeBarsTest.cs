using Ib.Csharp.Api.Client.Implementation;
using Ib.Csharp.Api.Client.Implementation.Helper;

namespace Ib.Csharp.Api.Client.Tests;

[TestFixture]
public class GetRealTimeBarsTest
{
    [Test]
    public async Task ShouldReceiveRealTimeBars()
    {
        using var client = new IbClient();
        client.OnNotification += Console.WriteLine;
        client.Start("127.0.0.1", 4001, 0);
        client.OnRealTimeBarUpdate += realTimeBar =>
        {
            Console.WriteLine($"Real time bar received: Open:{realTimeBar.Open}");
        };
        const int requestId = 1005;
        var contractDataList = await client.GetContractData("ES", "FUT");
        var contractDetails = contractDataList.OrderBy(x=> x.Contract.LastTradeDate).FirstOrDefault(x=> 
            DateTime.ParseExact(x.Contract.LastTradeDate, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture) > DateTime.Now);
        client.NotifySubscriber($"Using contract - Exchange:{contractDetails?.Contract.Exchange} LastDate:{contractDetails?.Contract.LastTradeDate}");
        client.SubscribeToRealTimeBars(requestId, contractDetails!.Contract);

        await AsyncHelper.Wait(10000);
    }
}