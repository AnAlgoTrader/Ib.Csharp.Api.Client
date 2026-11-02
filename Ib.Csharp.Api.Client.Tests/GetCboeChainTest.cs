using Ib.Csharp.Api.Client.Implementation.Helper;

namespace Ib.Csharp.Api.Client.Tests;

[TestFixture]
public class GetCboeChainTest
{
    [Test]
    public async Task ShouldGetCboeChain()
    {
        var data = await CboeHelper.GetCboeChains();
        var symbols = data.Select(x => x.Symbol).Distinct().ToList();
    }
}