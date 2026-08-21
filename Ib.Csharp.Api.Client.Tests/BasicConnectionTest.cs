using Ib.Csharp.Api.Client.Implementation;
using Ib.Csharp.Api.Client.Implementation.Helper;
using NUnit.Framework.Legacy;

namespace Ib.Csharp.Api.Client.Tests
{
    [TestFixture]
    public class BasicConnectionTest
    {
        [Test]
        public async Task ShouldReceiveAccountsList()
        {
            using var client = new IbClient();
            client.OnNotification += Console.WriteLine;
            client.Start("127.0.0.1", 4001, 0);
            
            var accountList = await AsyncHelper.WaitForCallback<string>(
                subscribe => client.OnAccountListUpdate += subscribe,
                10000);
            
            ClassicAssert.IsNotEmpty(accountList);
            Console.WriteLine($"Account List: {accountList}");
        }
    }
}
