using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ib.Csharp.Api.Client.Implementation.Model;
using IBApi;

namespace Ib.Csharp.Api.Client.Implementation.Helper;

public static class OptionChainHelper
{
    public static async Task<ContractDetails> GetOptionChainContract(IbClient client, SecurityDefinitionParameter parameter, string secType, double strike, string right, string expiration)
    {
        var contract = new Contract
        {
            Symbol = parameter.TradingClass,
            TradingClass = parameter.TradingClass,
            SecType = secType,
            Exchange = parameter.Exchange,
            Currency = "USD",
            Strike = strike,
            Right = right,
            Multiplier = parameter.Multiplier,
            LastTradeDateOrContractMonth = expiration
        };
        var contractDetailsList = await client.GetContractData(contract, throwOnFailure: true);
        return contractDetailsList?.FirstOrDefault();
    }

    public static List<double> GetStrikesAroundPrice(IEnumerable<double> strikes, double price, int count)
    {
        return strikes
            .OrderBy(s => Math.Abs(s - price))
            .Take(count)
            .OrderByDescending(s => s)
            .ToList();
    }

    public static List<double> GetStrikesAboveThreshold(IEnumerable<double> strikes, double threshold, int count)
    {
        return strikes
            .Where(n => n > threshold)
            .OrderBy(n => n)
            .Take(count)
            .Reverse()
            .ToList();
    }

    public static double[] GetSpreadStrikes(IEnumerable<double> strikes, double lastPrice, double distanceToFirstStrike, double spreadWidth, double strikeMultiple = 10.0)
    {
        var sortedStrikes = strikes.OrderBy(s => s).ToList();
        if (sortedStrikes.Count == 0) return [double.NaN, double.NaN];

        var threshold = lastPrice + distanceToFirstStrike;

        Func<double, bool> isMultipleOf = v =>
        {
            var rem = Math.Abs(v % strikeMultiple);
            return rem < 1e-6 || Math.Abs(strikeMultiple - rem) < 1e-6;
        };

        double? firstStrike = null;
        double? secondStrike = null;

        int? firstIndex = null;
        for (var i = 0; i < sortedStrikes.Count; i++)
        {
            if (sortedStrikes[i] >= threshold && isMultipleOf(sortedStrikes[i]))
            {
                firstStrike = sortedStrikes[i];
                firstIndex = i;
                break;
            }
        }

        if (!firstStrike.HasValue) return [double.NaN, double.NaN];

        var secondStrikeThreshold = firstStrike.Value + spreadWidth;
        for (var i = firstIndex.Value + 1; i < sortedStrikes.Count; i++)
        {
            if (sortedStrikes[i] >= secondStrikeThreshold && isMultipleOf(sortedStrikes[i]))
            {
                secondStrike = sortedStrikes[i];
                break;
            }
        }

        return [firstStrike.Value, secondStrike ?? double.NaN];
    }

    public static async Task<List<ContractDetails>> GetNextOptionsOnFuturesContracts(IbClient client, string symbol)
    {
        var contractList = await client.GetContractData(new Contract
        {
            Symbol = symbol,
            SecType = "FOP",
            Exchange = "CME",
            Currency = "USD"
        }, 30000);

        var today = DateTime.Now.ToString("yyyyMMdd");
        var dates = contractList.Select(x => x.Contract.LastTradeDateOrContractMonth).Distinct().OrderByDescending(x => x).ToList();
        var firstExpiration = dates.First(x => string.Compare(x, today, StringComparison.Ordinal) >= 0);
        return string.IsNullOrEmpty(firstExpiration) ? [] : 
            contractList.Where(x => x.Contract.LastTradeDateOrContractMonth == firstExpiration).OrderBy(x=> x.Contract.Strike).ToList();
    }
    
    public static async Task<List<ContractDetails>> GetTodayOptionsOnFuturesContracts(IbClient client, string symbol)
    {
        var today = DateTime.Now.ToString("yyyyMMdd");
        var contractList = await client.GetContractData(new Contract
        {
            Symbol = symbol,
            SecType = "FOP",
            Exchange = "CME",
            Currency = "USD",
            LastTradeDateOrContractMonth = today
        });
        
        return contractList.OrderBy(x=> x.Contract.Strike).ToList();
    }

    public static async Task<List<ContractDetails>> GetOptionsChainContracts(IbClient client, string symbol, string exchange, string expiration)
    {
        var data = await client.GetContractData(new Contract
        {
            Symbol = symbol,
            SecType = "OPT",
            Exchange = exchange,
            Currency = "USD",
            LastTradeDateOrContractMonth = expiration
        }, 20000);

        return data.OrderByDescending(x => x.Contract.Strike).ToList();
    }
    
    public static double FindAtmStrike(HashSet<double> strikes, double marketPrice)
    {
        if (strikes == null || strikes.Count == 0)
            throw new ArgumentException("Strikes cannot be empty.");

        return strikes.MinBy(strike => Math.Abs(strike - marketPrice));
    }
}