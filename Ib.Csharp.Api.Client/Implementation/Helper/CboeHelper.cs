using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Ib.Csharp.Api.Client.Implementation.Model;
using IBApi;

namespace Ib.Csharp.Api.Client.Implementation.Helper;

public class CboeHelper
{
    private const string SpxUrl = "https://cdn.cboe.com/api/global/delayed_quotes/options/_SPX.json";
    private const string IbkrDateFormat = "yyyyMMdd";
    private const double StrikeRange = 200;

    private static async Task<string> DownloadAllDataAsync()
    {
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");

        using var request = new HttpRequestMessage(HttpMethod.Get, SpxUrl);
        request.Headers.Add("Cache-Control", "no-cache, no-store");
        request.Headers.Add("Pragma", "no-cache");

        using var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }

    private static async Task<CboeData> GetCboeRawDataset()
    {
        //I am reading from a static json file, just for the development process, so I don't have to call the service every single time
        //var json = await DownloadAllDataAsync();
        //await File.WriteAllTextAsync("./cboeData.json", json);
        var json = await File.ReadAllTextAsync("./cboeData.json");
        return CboeData.FromJson(json);
    }

    public static async Task<List<CboeChain>> GetCboeChains()
    {
        var rawData = await GetCboeRawDataset();
        var options = rawData?.Data?.Options;
        if (options is null || options.Length == 0)
            return [];

        var underlyingPrice = rawData.Data.CurrentPrice;

        var parsedOptions = new List<(Option Option, ParsedOption Parsed)>();
        foreach (var option in options)
        {
            if (TryParseOptionSymbol(option.OptionOption, out var parsed))
                parsedOptions.Add((option, parsed));
        }

        return parsedOptions
            .Where(x => Math.Abs(x.Parsed.Strike - underlyingPrice) <= StrikeRange)
            .GroupBy(x => new { x.Parsed.Root, x.Parsed.ExpirationDate })
            .OrderBy(x => x.Key.ExpirationDate)
            .ThenBy(x => x.Key.Root)
            .Select(chainGroup => new CboeChain
            {
                Symbol = chainGroup.Key.Root,
                ExpirationDate = chainGroup.Key.ExpirationDate,
                Data = chainGroup
                    .GroupBy(x => x.Parsed.Strike)
                    .Select(strikeGroup =>
                        BuildChainData(strikeGroup.Key, strikeGroup, underlyingPrice))
                    .OrderBy(row => row.Strike)
                    .ToList()
            })
            .ToList();
    }

    private static OptionChainData BuildChainData(
        double strike,
        IEnumerable<(Option Option, ParsedOption Parsed)> options,
        double underlyingPrice)
    {
        var row = new OptionChainData
        {
            Strike = strike,
            UnderlyingPrice = underlyingPrice
        };

        var optionList = options.ToList();

        var call = SelectOption(optionList, isCall: true);
        if (call.HasValue)
            ApplyOption(row, call.Value.Option, call.Value.Parsed);

        var put = SelectOption(optionList, isCall: false);
        if (put.HasValue)
            ApplyOption(row, put.Value.Option, put.Value.Parsed);

        return row;
    }

    private static (Option Option, ParsedOption Parsed)? SelectOption(
        List<(Option Option, ParsedOption Parsed)> options,
        bool isCall)
    {
        var matching = options.FirstOrDefault(x => x.Parsed.IsCall == isCall);
        return matching.Option is not null ? matching : null;
    }

    private static void ApplyOption(OptionChainData row, Option option, ParsedOption parsed)
    {
        var contract = CreateContract(parsed);

        if (parsed.IsCall)
        {
            row.CallContract = contract;
            row.CallBid = option.Bid;
            row.CallAsk = option.Ask;
            row.CallIv = option.Iv;
            row.CallDelta = option.Delta;
            row.CallGamma = option.Gamma;
            row.CallVega = option.Vega;
            row.CallOpenInterest = option.OpenInterest;
        }
        else
        {
            row.PutContract = contract;
            row.PutBid = option.Bid;
            row.PutAsk = option.Ask;
            row.PutIv = option.Iv;
            row.PutDelta = option.Delta;
            row.PutGamma = option.Gamma;
            row.PutVega = option.Vega;
            row.PutOpenInterest = option.OpenInterest;
        }
    }

    private static ContractDetails CreateContract(ParsedOption parsed)
    {
        var expiration = parsed.ExpirationDate.ToString(IbkrDateFormat, CultureInfo.InvariantCulture);
        var right = parsed.IsCall ? "C" : "P";

        return new ContractDetails
        {
            Contract = new Contract
            {
                Symbol = parsed.Root,
                SecType = "OPT",
                LastTradeDateOrContractMonth = expiration,
                Strike = parsed.Strike,
                Right = right,
                Multiplier = "100",
                Exchange = "CBOE",
                Currency = "USD",
                LocalSymbol = parsed.Symbol,
                TradingClass = parsed.Root
            },
            RealExpirationDate = expiration,
            UnderSymbol = parsed.Root,
            UnderSecType = "IND"
        };
    }

    private static bool TryParseOptionSymbol(string symbol, out ParsedOption parsed)
    {
        parsed = default;

        if (string.IsNullOrWhiteSpace(symbol))
            return false;

        // Cboe option symbols look like SPXW270115C07000000:
        // <root><yyMMdd><C|P><strike * 1000, zero padded to 8 digits>.
        if (symbol.Length < 16)
            return false;

        var strikeStart = symbol.Length - 8;
        var rightIndex = strikeStart - 1;
        var dateStart = rightIndex - 6;

        if (dateStart <= 0)
            return false;

        var right = symbol[rightIndex];
        if (right != 'C' && right != 'P')
            return false;

        var datePart = symbol.AsSpan(dateStart, 6);
        var strikePart = symbol.AsSpan(strikeStart, 8);

        if (!int.TryParse(datePart[..2], out var year)
            || !int.TryParse(datePart.Slice(2, 2), out var month)
            || !int.TryParse(datePart[4..], out var day)
            || !int.TryParse(strikePart, out var strikeInThousandths))
        {
            return false;
        }

        DateTime expirationDate;
        try
        {
            expirationDate = new DateTime(2000 + year, month, day);
        }
        catch (ArgumentOutOfRangeException)
        {
            return false;
        }

        parsed = new ParsedOption(
            symbol,
            symbol[..dateStart],
            expirationDate,
            strikeInThousandths / 1000d,
            right == 'C');

        return true;
    }

    private readonly record struct ParsedOption(
        string Symbol,
        string Root,
        DateTime ExpirationDate,
        double Strike,
        bool IsCall);
}
