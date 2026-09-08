using Ib.Csharp.Api.Client.Implementation;
using Ib.Csharp.Api.Client.Implementation.Helper;
using Ib.Csharp.Api.Client.Implementation.Model;
using NUnit.Framework.Legacy;

namespace Ib.Csharp.Api.Client.Tests;

[TestFixture]
public class GetPortfolioUpdatesTest
{
    [Test]
    public async Task ShouldReceivePortfolioUpdates()
    {
        using var client = new IbClient();
        client.OnNotification += Console.WriteLine;
        client.OnAccountListUpdate += client.SubscribeToPortfolioUpdates;
        client.Start("127.0.0.1", 4001, 0);
        
        var accountUpdate = await AsyncHelper.WaitForCallback<AccountUpdate>(
            subscribe => client.OnAccountUpdate += subscribe,
            10000);
        
        Console.WriteLine($"Account: {accountUpdate.AccountCode} Available:{accountUpdate.AvailableFunds} BuyingPower:{accountUpdate.BuyingPower} CashEuro:{accountUpdate.CashEuro} CashUsd:{accountUpdate.CashUsd}");
        ClassicAssert.NotNull(accountUpdate);
    }
}
