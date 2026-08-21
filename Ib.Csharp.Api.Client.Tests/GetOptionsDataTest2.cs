using Ib.Csharp.Api.Client.Implementation;
using Ib.Csharp.Api.Client.Implementation.Helper;

namespace Ib.Csharp.Api.Client.Tests;

[TestFixture]
public class GetOptionsDataTest2
{
    [Test]
    public async Task ShouldDisplayOptionsData2()
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
        client.OnMarketDataTradeUpdate += marketTradeUpdate =>
        {
            Console.WriteLine($"Trade --> Price:{marketTradeUpdate.First().Value.LastTradedPrice} Size:{marketTradeUpdate.First().Value.LastTradedSize}");
        };
        client.Start("127.0.0.1", 4001, 0);
        
        const string symbol = "SPX";
        const string exchange = "SMART";
        string expiration = DateTime.Now.AddDays(1).ToString("yyyyMMdd");
        var optionsContractList = await OptionChainHelper.GetOptionsChainContracts(client, symbol, exchange, expiration);
        await CommonTest.SaveAllContractsToAFile(optionsContractList, "optionsContracts2.csv");
        
        var currentMarketPrice = await MarketDataHelper.GetSpxPrice();
        var strikes = optionsContractList.Select(x => x.Contract.Strike).ToList();
        var strike = CollectionHelper.GetClosest(strikes, MathHelper.RoundToNearest(currentMarketPrice));
        var contract = optionsContractList.FirstOrDefault(x => MathHelper.DoubleEquals(x.Contract.Strike, strike));
        client.SubscribeToMarketData(marketDataId, contract);
        
        await AsyncHelper.Wait(5000);
    }
}