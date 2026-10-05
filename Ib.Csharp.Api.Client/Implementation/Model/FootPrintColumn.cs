using System;
using System.Collections.Generic;
using System.Linq;

namespace Ib.Csharp.Api.Client.Implementation.Model;

public enum ColumnTrend
{
    Flat,
    Up,
    Down
}

public class FootPrintColumn
{
    public DateTime Time { get; set; }
    public List<FootPrintRow> Rows { get; set; } = new();

    public double FirstPrice { get; set; }
    public double LastPrice { get; set; }

    public ColumnTrend Trend =>
        LastPrice > FirstPrice ? ColumnTrend.Up :
        LastPrice < FirstPrice ? ColumnTrend.Down :
        ColumnTrend.Flat;

    public bool IsUp => LastPrice >= FirstPrice;

    public double DeltaSize => Rows.Sum(r => r.DeltaSize);
    public double TotalSize => Rows.Sum(r => r.BidSize + r.AskSize);
}
