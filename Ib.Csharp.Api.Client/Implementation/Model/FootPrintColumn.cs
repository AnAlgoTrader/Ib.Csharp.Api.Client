using System;
using System.Collections.Generic;
using System.Linq;

namespace Ib.Csharp.Api.Client.Implementation.Model;

public class FootPrintColumn
{
    public DateTime Time { get; set; }
    public List<FootPrintRow> Rows { get; set; } = new();

    public double DeltaSize => Rows.Sum(r => r.DeltaSize);
    public double TotalSize => Rows.Sum(r => r.BidSize + r.AskSize);
}
