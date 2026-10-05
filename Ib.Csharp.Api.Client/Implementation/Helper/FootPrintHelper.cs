using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Ib.Csharp.Api.Client.Implementation.Model;

namespace Ib.Csharp.Api.Client.Implementation.Helper;

public class FootPrintHelper
{
    private readonly BufferedLeeReadyClassifier _classifier;
    private readonly Lock _lock = new();

    private List<FootPrintColumn> Columns { get; } = new();

    public FootPrintHelper()
    {
        _classifier = new BufferedLeeReadyClassifier();
        _classifier.OnTradeClassified += HandleClassifiedTrade;
    }

    public void UpdatePrice(PriceTickByTickUpdate price)
    {
        lock (_lock)
        {
            _classifier.OnQuote(price.Time, price.Bid, price.Ask);
            _classifier.Pump();
        }
    }

    public void UpdateTrade(TradeTickByTickUpdate trade)
    {
        lock (_lock)
        {
            _classifier.OnTrade(trade.Time, trade.Price, trade.Size);
            _classifier.Pump();
        }
    }

    /// <summary>
    /// Returns a thread-safe snapshot of the current columns and price levels for UI rendering.
    /// </summary>
    public List<FootPrintColumn> GetSnapshot()
    {
        lock (_lock)
        {
            _classifier.Pump();

            return Columns.Select(c => new FootPrintColumn
            {
                Time = c.Time,
                Rows = c.Rows.Select(r => new FootPrintRow
                {
                    Level = r.Level,
                    BidSize = r.BidSize,
                    AskSize = r.AskSize,
                    IsPoc = r.IsPoc
                }).ToList()
            }).ToList();
        }
    }

    private void HandleClassifiedTrade(ClassifiedTrade trade)
    {
        long minuteStartSec = trade.Time / 60 * 60;
        var columnTime = DateTimeOffset.FromUnixTimeSeconds(minuteStartSec).UtcDateTime;

        var targetColumn = Columns.FirstOrDefault(c => c.Time == columnTime);

        if (targetColumn == null)
        {
            targetColumn = new FootPrintColumn { Time = columnTime };
            Columns.Add(targetColumn);

            if (Columns.Count > 5)
            {
                Columns.RemoveAt(0);
            }
        }

        var roundedPrice = Math.Round(trade.Price, 4);
        var row = targetColumn.Rows.FirstOrDefault(r => Math.Abs(r.Level - roundedPrice) < 1e-4);
        if (row == null)
        {
            row = new FootPrintRow { Level = roundedPrice };
            targetColumn.Rows.Add(row);
            targetColumn.Rows.Sort((a, b) => b.Level.CompareTo(a.Level));
        }

        if (trade.Side == TradeSide.Buy)
        {
            row.AskSize += (double)trade.Size;
        }
        else
        {
            row.BidSize += (double)trade.Size;
        }

        var maxVol = targetColumn.Rows.Max(r => r.BidSize + r.AskSize);
        foreach (var r in targetColumn.Rows)
        {
            r.IsPoc = maxVol > 0 && Math.Abs((r.BidSize + r.AskSize) - maxVol) < 1e-4;
        }
    }

    public static string FormatColumn(FootPrintColumn column)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"Footprint Column: {column.Time:yyyy-MM-dd HH:mm:ss} UTC | Total Delta: {column.DeltaSize:F0} | Total Vol: {column.TotalSize:F0}");
        sb.AppendLine($"{"BID VOL",10} | {"PRICE",10} | {"ASK VOL",10} | {"DELTA",10}");
        sb.AppendLine(new string('-', 49));
        foreach (var row in column.Rows)
        {
            var pocTag = row.IsPoc ? " <-- POC" : "";
            sb.AppendLine($"{row.BidSize,10:F0} | {row.Level,10:F2} | {row.AskSize,10:F0} | {row.DeltaSize,10:F0}{pocTag}");
        }
        sb.AppendLine(new string('-', 49));
        sb.AppendLine($"{"TOTAL DELTA:",35} {column.DeltaSize,10:F0}");
        return sb.ToString();
    }
}