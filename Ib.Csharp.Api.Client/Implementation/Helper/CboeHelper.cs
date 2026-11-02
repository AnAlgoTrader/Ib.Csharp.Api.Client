using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Ib.Csharp.Api.Client.Implementation.Model;

namespace Ib.Csharp.Api.Client.Implementation.Helper;

public class CboeHelper
{
    private const string SpxUrl = "https://cdn.cboe.com/api/global/delayed_quotes/options/_SPX.json";

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
        return null;
    }
}