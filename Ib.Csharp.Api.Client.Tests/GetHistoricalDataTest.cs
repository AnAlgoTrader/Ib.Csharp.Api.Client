using System.Text;
using Ib.Csharp.Api.Client.Implementation;
using Ib.Csharp.Api.Client.Implementation.Model;

namespace Ib.Csharp.Api.Client.Tests;

[TestFixture]
public class GetHistoricalDataTest
{
    [Test]
    public async Task ShouldGetHistoricalData()
    {
        using var client = new IbClient();
        client.OnNotification += Console.WriteLine;
        client.Start("127.0.0.1", 4001, 0);

        var contractDataList = await client.GetContractData("ES", "FUT");
        var contract = contractDataList.OrderBy(x => x.Contract.LastTradeDate).FirstOrDefault(x =>
            DateTime.ParseExact(x.Contract.LastTradeDate, "yyyyMMdd",
                System.Globalization.CultureInfo.InvariantCulture) > DateTime.Now);

        var historicalDataList = await client.GetHistoricalData(1020, contract!.Contract, $"1 {nameof(Duration.M)}", $"1 {nameof(BarSize.min)}", "TRADES", 600000);
        Assert.That(historicalDataList, Is.Not.Empty);

        var output = new StringBuilder();
        const string headers = "Date, Open, High, Low, Close, Volume";
        output.AppendLine(headers);

        foreach (var bar in historicalDataList)
        {
            if (bar.Volume != 0)
                output.AppendLine($"{bar.Time},{bar.Open},{bar.High},{bar.Low},{bar.Close},{bar.Volume}");
        }

        await File.WriteAllTextAsync("historicalData.csv", output.ToString());
        Console.WriteLine("Historical data saved in historicalData.csv");
    }
}