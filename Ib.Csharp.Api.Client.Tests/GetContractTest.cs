using System.Text.Json;
using Ib.Csharp.Api.Client.Implementation;
using NUnit.Framework.Legacy;

namespace Ib.Csharp.Api.Client.Tests;

[TestFixture]
public class GetContractTest
{
    [Test]
    public async Task ShouldGetContractData()
    {
        using var client = new IbClient();
        client.OnNotification += Console.WriteLine;
        client.Start("127.0.0.1", 4001, 0);
        
        var contractDataList = await client.GetContractData("ES", "FUT");
        
        //JSON files serialized in the bin folder
        var outputDir = Path.Combine(TestContext.CurrentContext.TestDirectory, "Tests", "TestFilesOutput");
        Directory.CreateDirectory(outputDir);
        
        var counter = 1;
        foreach (var contractData in contractDataList)
        {
            var fileName = $"{contractData.RealExpirationDate}_{counter:D3}.json";
            var filePath = Path.Combine(outputDir, fileName);
            var json = JsonSerializer.Serialize(contractData, TestHelper.GetSerializerOptions());
            
            await File.WriteAllTextAsync(filePath, json);
            Console.WriteLine($"Saved contract data to: {fileName}");
            counter++;
        }
        
        ClassicAssert.IsNotNull(contractDataList);
        ClassicAssert.IsNotEmpty(contractDataList);
    }
}