using Ib.Csharp.Api.Client.Implementation;
using Ib.Csharp.Api.Client.Implementation.Helper;

namespace Ib.Csharp.Api.Client.Tests;

[TestFixture]
public class GetOptionsOnFuturesDataTest
{
    [Test]
    public async Task ShouldDisplayOptionsOnFuturesData()
    {
        using var client = new IbClient();
        const int marketDataId = 1005;
        client.OnNotification += Console.WriteLine;
        client.OnOptionComputationUpdate += computation =>
        {
            Console.WriteLine($"Delta:{computation.Delta} Vega:{computation.Vega} Gamma:{computation.Gamma} Theta:{computation.Theta}");
        };
        client.OnMarketDataPriceUpdate += updates =>
        {
            var marketDataUpdate = updates[marketDataId];
            Console.WriteLine($"Bid:{marketDataUpdate.BidPrice} Ask:{marketDataUpdate.AskPrice}");
        };
        client.Start("127.0.0.1", 4001, 0);

        const string symbol = "ES";
        var optionOnFuturesContractList = await OptionChainHelper.GetTodayOptionsOnFuturesContracts(client, symbol);
        var selectedExpiration = optionOnFuturesContractList.First().Contract.LastTradeDateOrContractMonth;
        Console.WriteLine($"Expiration: {selectedExpiration}");
        await CommonTest.SaveAllContractsToAFile(optionOnFuturesContractList, "optionsContracts.csv");

        var currentMarketPrice = await YahooDataHelper.GetESPrice();
        var strikes = optionOnFuturesContractList.Select(x => x.Contract.Strike).ToList();
        var strike = CollectionHelper.GetClosest(strikes, MathHelper.RoundToNearest(currentMarketPrice));
        var contract = optionOnFuturesContractList.FirstOrDefault(x => MathHelper.DoubleEquals(x.Contract.Strike, strike));
        client.SubscribeToMarketData(marketDataId, contract);
        
        await AsyncHelper.Wait(5000);
    }
}