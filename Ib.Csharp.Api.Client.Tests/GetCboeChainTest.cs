using Ib.Csharp.Api.Client.Implementation.Helper;

namespace Ib.Csharp.Api.Client.Tests;

[TestFixture]
public class GetCboeChainTest
{
    [Test]
    public async Task ShouldGetCboeChains()
    {
        var data = await CboeHelper.GetCboeChains();
        var symbols = data.Select(x => x.Symbol).Distinct().ToList();
        Assert.That(symbols, Has.Count.EqualTo(2));
        Assert.That(symbols, Is.Not.Empty);
    }
    
    [Test]
    public async Task ShouldGetTodayCboeChain()
    {
        var data = await CboeHelper.GetTodaySpxwChain();
        Assert.That(data, Is.Not.Empty);
    }
}