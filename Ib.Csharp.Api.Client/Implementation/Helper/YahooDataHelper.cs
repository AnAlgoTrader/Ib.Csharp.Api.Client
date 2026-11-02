using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

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
            
            var json = JObject.Parse(response);
            var result = json["chart"]?["result"]?[0];
            var meta = result?["meta"];
            
            double price = meta?["regularMarketPrice"]?.Value<double>() ?? -1.0;

            if (result?["indicators"]?["quote"]?[0]?["close"] is JArray { Count: > 0 } closes)
            {
                for (int i = closes.Count - 1; i >= 0; i--)
                {
                    var val = closes[i]?.Value<double?>();
                    if (val is > 0)
                    {
                        price = val.Value;
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