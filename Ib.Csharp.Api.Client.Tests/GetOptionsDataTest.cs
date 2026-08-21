using Ib.Csharp.Api.Client.Implementation;
using Ib.Csharp.Api.Client.Implementation.Helper;
using NUnit.Framework.Legacy;

namespace Ib.Csharp.Api.Client.Tests;

[TestFixture]
public class GetOptionsDataTest
{
    [Test]
    public async Task ShouldDisplayOptionsData()
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
            Console.WriteLine($"Bid:{marketDataUpdate.BidPrice} Ask:{marketDataUpdate.AskPrice} CallOpenInterest:{marketDataUpdate.CallOpenInterest} PutOpenInterest:{marketDataUpdate.PutOpenInterest}");
        };
        client.OnMarketDataTradeUpdate += marketTradeUpdate =>
        {
            Console.WriteLine($"Trade --> Price:{marketTradeUpdate.First().Value.LastTradedPrice} Size:{marketTradeUpdate.First().Value.LastTradedSize}");
        };
        client.Start("127.0.0.1", 4001, 0);
        
        var contractList = await client.GetContractData("SPX", "IND");
        var contractDetails = contractList.First();
        var optionDefinitions = await client.GetOptionDefinitions(contractDetails.Contract);
        ClassicAssert.IsNotEmpty(optionDefinitions);
        
        var date = DateHelper.GetNextTradingDay(1);
        var optionDefinition = optionDefinitions.First(x => x.Expirations.Contains(date) && x.Exchange == "CBOE");
        ClassicAssert.IsNotNull(optionDefinition);
        
        var price = await MarketDataHelper.GetSpxPrice();
        var strike = CollectionHelper.GetClosest(optionDefinition.Strikes.ToHashSet(), MathHelper.RoundToNearest(price));
        var callSideContract =
             await OptionChainHelper.GetOptionChainContract(client, optionDefinition, "OPT", strike, "C", date);
        callSideContract.Contract.Exchange = "SMART";

        client.SubscribeToMarketData(marketDataId, callSideContract);
        
        await AsyncHelper.Wait(15000);
    }
}