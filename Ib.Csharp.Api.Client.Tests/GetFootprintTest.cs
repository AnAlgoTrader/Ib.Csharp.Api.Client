using System;
using System.Linq;
using Ib.Csharp.Api.Client.Implementation;
using Ib.Csharp.Api.Client.Implementation.Helper;
using Ib.Csharp.Api.Client.Implementation.Model;

namespace Ib.Csharp.Api.Client.Tests;

[TestFixture]
public class GetFootPrintTest
{
    [Test]
    public async Task ShouldBuildFootprint()
    {
        var footPrintHelper = new FootPrintHelper();
        using var client = new IbClient();
        client.OnNotification += Console.WriteLine;
        client.OnMarketDataPriceTickByTickUpdate += footPrintHelper.UpdatePrice;
        client.OnMarketDataTradeTickByTickUpdate += footPrintHelper.UpdateTrade;
        client.Start("127.0.0.1", 4001, 0);

        var contractDataList = await client.GetContractData("ES", "FUT");
        var contract = contractDataList.OrderBy(x => x.Contract.LastTradeDate).FirstOrDefault(x =>
            DateTime.ParseExact(x.Contract.LastTradeDate, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture) > DateTime.Now);
        client.NotifySubscriber($"Using contract - Exchange:{contract?.Contract.Exchange} LastDate:{contract?.Contract.LastTradeDate}");

        client.SubscribeToPriceTickByTick(1105, contract!.Contract);
        client.SubscribeToTradesTickByTick(1106, contract.Contract);

        using var cts = new CancellationTokenSource();
        ConsoleCancelEventHandler cancelHandler = (_, e) =>
        {
            e.Cancel = true;
            cts.Cancel();
        };
        Console.CancelKeyPress += cancelHandler;

        const string logFile = "footprint.log";
        try
        {
            while (!cts.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(1000, cts.Token);
                }
                catch (OperationCanceledException)
                {
                    break;
                }

                var columns = footPrintHelper.GetSnapshot();
                var sb = new System.Text.StringBuilder();
                sb.AppendLine($"========== FOOTPRINT SNAPSHOT ({DateTime.UtcNow:HH:mm:ss} UTC) [Columns: {columns.Count}/5] ==========");
                for (int i = 0; i < columns.Count; i++)
                {
                    sb.AppendLine($"--- Column {i + 1} of {columns.Count} ---");
                    sb.AppendLine(FootPrintHelper.FormatColumn(columns[i]));
                }

                await File.WriteAllTextAsync(logFile, sb.ToString(), cts.Token);
            }
        }
        finally
        {
            Console.CancelKeyPress -= cancelHandler;
        }
    }

    [Test]
    public void ShouldClassifyTicksWithLeeReady()
    {
        var classifier = new LeeReadyClassifier();

        // 1. Initial quote
        classifier.OnQuote(1728130000, 5000.00, 5000.25);

        // Trade at Ask -> Buy
        Assert.That(classifier.Classify(5000.25), Is.EqualTo(TradeSide.Buy));

        // Trade at Bid -> Sell
        Assert.That(classifier.Classify(5000.00), Is.EqualTo(TradeSide.Sell));

        // 2. Sweeping ask: quote moves up to 5000.25 / 5000.50
        classifier.OnQuote(1728130001, 5000.25, 5000.50);

        // Trade at 5000.25 (was prevAsk before book moved) -> correctly classified as Buy
        Assert.That(classifier.Classify(5000.25), Is.EqualTo(TradeSide.Buy));

        // Trade at new Ask 5000.50 -> Buy
        Assert.That(classifier.Classify(5000.50), Is.EqualTo(TradeSide.Buy));

        // 3. Zero-tick test
        // Next trade at same price (5000.50) without quote change -> Buy
        Assert.That(classifier.Classify(5000.50), Is.EqualTo(TradeSide.Buy));

        // 4. Inside wide spread
        classifier.OnQuote(1728130002, 5000.00, 5000.75); // Mid = 5000.375
        Assert.That(classifier.Classify(5000.50), Is.EqualTo(TradeSide.Buy));
        Assert.That(classifier.Classify(5000.25), Is.EqualTo(TradeSide.Sell));
    }

    [Test]
    public void ShouldBuildFootprintFromTicks()
    {
        var helper = new FootPrintHelper();
        long t = 1728130000; // Fixed timestamp

        // Set quote: 5000.00 Bid / 5000.25 Ask
        helper.UpdatePrice(new PriceTickByTickUpdate
        {
            Time = t,
            Bid = 5000.00,
            Ask = 5000.25
        });

        // 10 contracts bought at Ask (5000.25)
        helper.UpdateTrade(new TradeTickByTickUpdate
        {
            Time = t,
            Price = 5000.25,
            Size = 10
        });

        // 4 contracts sold at Bid (5000.00)
        helper.UpdateTrade(new TradeTickByTickUpdate
        {
            Time = t,
            Price = 5000.00,
            Size = 4
        });

        var columns = helper.GetSnapshot();
        Assert.That(columns, Has.Count.EqualTo(1));

        var col = columns[0];
        Assert.That(col.Rows, Has.Count.EqualTo(2));

        var askRow = col.Rows.First(r => Math.Abs(r.Level - 5000.25) < 1e-4);
        Assert.That(askRow.AskSize, Is.EqualTo(10));
        Assert.That(askRow.BidSize, Is.EqualTo(0));
        Assert.That(askRow.DeltaSize, Is.EqualTo(10)); // Positive delta for buys

        var bidRow = col.Rows.First(r => Math.Abs(r.Level - 5000.00) < 1e-4);
        Assert.That(bidRow.BidSize, Is.EqualTo(4));
        Assert.That(bidRow.AskSize, Is.EqualTo(0));
        Assert.That(bidRow.DeltaSize, Is.EqualTo(-4)); // Negative delta for sells

        Assert.That(col.DeltaSize, Is.EqualTo(6)); // Total Delta: 10 - 4 = +6
        Assert.That(col.TotalSize, Is.EqualTo(14)); // Total Vol: 10 + 4 = 14

        var formatted = FootPrintHelper.FormatColumn(col);
        Assert.That(formatted, Does.Contain("TOTAL DELTA:"));
    }

    [Test]
    public void ShouldRollColumnEvery1000Ticks()
    {
        var helper = new FootPrintHelper(1000);
        long t = 1728130000;

        helper.UpdatePrice(new PriceTickByTickUpdate { Time = t, Bid = 5000.00, Ask = 5000.25 });

        for (var i = 0; i < 2500; i++)
        {
            helper.UpdateTrade(new TradeTickByTickUpdate
            {
                Time = t + i / 100,
                Price = i % 2 == 0 ? 5000.25 : 5000.00,
                Size = 1
            });
        }

        var columns = helper.GetSnapshot();
        Assert.That(columns, Has.Count.EqualTo(3));
        Assert.That(columns[0].TotalSize, Is.EqualTo(1000));
        Assert.That(columns[1].TotalSize, Is.EqualTo(1000));
        Assert.That(columns[2].TotalSize, Is.EqualTo(500));

        // Columns are ordered oldest -> newest with strictly increasing start times.
        Assert.That(columns[1].Time, Is.GreaterThan(columns[0].Time));
        Assert.That(columns[2].Time, Is.GreaterThan(columns[1].Time));
    }
}