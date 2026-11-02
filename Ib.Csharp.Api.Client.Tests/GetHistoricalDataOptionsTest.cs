using System.Text;
using Ib.Csharp.Api.Client.Implementation;
using Ib.Csharp.Api.Client.Implementation.Helper;

namespace Ib.Csharp.Api.Client.Tests;

[TestFixture]
public class GetHistoricalDataOptionsTest
{
    [Test]
    public async Task ShouldGetHistoricalDataOptions()
    {
        using var client = new IbClient();
        client.OnNotification += Console.WriteLine;
        client.Start("127.0.0.1", 4001, 0);

        const string symbol = "SPX";
        const string exchange = "SMART";
        string expiration = DateTime.Now.AddDays(1).ToString("yyyyMMdd");
        var optionsContractList = await OptionChainHelper.GetOptionsChainContracts(client, symbol, exchange, expiration);

        var currentMarketPrice = await YahooDataHelper.GetSpxPrice();
        var strikes = optionsContractList.Select(x => x.Contract.Strike).ToList();
        var strike = CollectionHelper.GetClosest(strikes, MathHelper.RoundToNearest(currentMarketPrice));
        var contract = optionsContractList.FirstOrDefault(x => MathHelper.DoubleEquals(x.Contract.Strike, strike));

        var output = new StringBuilder();
        const string headers = "Date, Price, Size";
        output.AppendLine(headers);

        var endTime = DateTime.Now.ToString("yyyyMMdd-HH:mm:ss");
        var historicalTickBidAsk = await client.GetHistoricalTickLast(1020, contract!.Contract, endTime, 60000);
        foreach (var tickBidAsk in historicalTickBidAsk)
        {
            output.AppendLine($"{tickBidAsk.Time},{tickBidAsk.Price},{tickBidAsk.Size}");
        }

        var fileName = $"HistoricalTickLast.csv";
        await File.WriteAllTextAsync(fileName, output.ToString());
        Console.WriteLine($"Historical data saved in {fileName}");
    }
}