using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using IBApi;

namespace Ib.Csharp.Api.Client.Tests;

internal static class CommonTest
{
    public static async Task SaveAllContractsToAFile(List<ContractDetails> optionOnFuturesContractList, string fileName)
    {
        var csv = new StringBuilder();
        csv.AppendLine("Symbol, SecType, LocalSymbol, TradingClass, Strike, ExpirationDate, Exchange, Currency");
        foreach (var contractData in optionOnFuturesContractList)
        {
            var contract = contractData.Contract;
            csv.AppendLine(
                $"{contract.Symbol},{contract.SecType},{contract.LocalSymbol},{contract.TradingClass},{contract.Strike},{contract.LastTradeDateOrContractMonth},{contract.Exchange},{contract.Currency}");
        }

        await File.WriteAllTextAsync(fileName, csv.ToString());
    }
}