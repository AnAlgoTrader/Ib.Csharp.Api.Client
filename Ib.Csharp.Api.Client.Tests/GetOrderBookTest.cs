using Ib.Csharp.Api.Client.Implementation;
using Ib.Csharp.Api.Client.Implementation.Helper;

namespace Ib.Csharp.Api.Client.Tests
{
    [TestFixture]
    public class GetOrderBookTest
    {
        [Test]
        public async Task ShouldReceiveOrderBook()
        {
            var logFile = "orderbook.log";
            using var client = new IbClient();
            client.OnNotification += msg => System.IO.File.AppendAllText(logFile, msg + Environment.NewLine);
            client.OnOrderBookUpdate += book =>
            {
                var ladder = OrderBookHelper.FormatAsLadder(book);
                var output = new System.Text.StringBuilder();
                output.AppendLine($"{DateTime.Now:HH:mm:ss.fff}");
                output.AppendLine($"{"BID SIZE",-10} | {"PRICE",8} | {"ASK SIZE",10}");
                output.AppendLine(new string('-', 35));
                foreach (var row in ladder)
                {
                    var bidSize = row.BidSize?.ToString("F0") ?? "";
                    var askSize = row.AskSize?.ToString("F0") ?? "";
                    output.AppendLine($"{bidSize,-10} | {row.Price,8:F2} | {askSize,10}");
                }
                output.AppendLine();
                System.IO.File.WriteAllText(logFile, output.ToString());
            };
            client.Start("127.0.0.1", 4001, 0);
            
            var contractDataList = await client.GetContractData("NQ", "FUT");
            var contractDetails = contractDataList.OrderBy(x=> x.Contract.LastTradeDate).FirstOrDefault(x=>
                DateTime.ParseExact(x.Contract.LastTradeDate, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture) > DateTime.Now);
            
            client.SubscribeToMarketDepth(1005, contractDetails);
            await AsyncHelper.Wait(2000);
        }
    }
}