using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Ib.Csharp.Api.Client.Implementation.Helper;

public static class YahooDataHelper
{
    private static readonly HttpClient HttpClient = new();

    private static async Task<double> GetYahooPrice(string symbol)
    {
        try
        {
            HttpClient.DefaultRequestHeaders.Clear();
            HttpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");
            
            var response = await HttpClient.GetStringAsync($"https://query1.finance.yahoo.com/v8/finance/chart/{symbol}?range=1d&interval=1m");
            
            using var json = JsonDocument.Parse(response);
            var root = json.RootElement;

            if (!root.TryGetProperty("chart", out var chart)
                || !chart.TryGetProperty("result", out var results)
                || results.ValueKind != JsonValueKind.Array
                || results.GetArrayLength() == 0)
            {
                return -1.0;
            }

            var result = results[0];

            double price = -1.0;
            if (result.TryGetProperty("meta", out var meta)
                && meta.TryGetProperty("regularMarketPrice", out var metaPrice)
                && metaPrice.ValueKind == JsonValueKind.Number
                && metaPrice.TryGetDouble(out var metaPriceValue))
            {
                price = metaPriceValue;
            }

            if (result.TryGetProperty("indicators", out var indicators)
                && indicators.TryGetProperty("quote", out var quotes)
                && quotes.ValueKind == JsonValueKind.Array
                && quotes.GetArrayLength() > 0
                && quotes[0].TryGetProperty("close", out var closes)
                && closes.ValueKind == JsonValueKind.Array)
            {
                for (int i = closes.GetArrayLength() - 1; i >= 0; i--)
                {
                    var close = closes[i];
                    if (close.ValueKind == JsonValueKind.Number
                        && close.TryGetDouble(out var val)
                        && val > 0)
                    {
                        price = val;
                        break;
                    }
                }
            }

            return price;
        }
        catch
        {
            return -1.0;
        }
    }
    

    public static async Task<double> GetNdxPrice()
    {
        return await GetYahooPrice("%5ENDX");
    }
    
    public static async Task<double> GetSpxPrice()
    {
        return await GetYahooPrice("%5EGSPC");
    }
    
    public static async Task<double> GetESPrice()
    {
        return await GetYahooPrice("ES%3DF");
    }
}