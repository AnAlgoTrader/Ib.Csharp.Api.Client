using System;
using System.Collections.Generic;

namespace Ib.Csharp.Api.Client.Implementation.Model;

public class FootPrintColumn
{
    public DateTime Time { get; set; }
    public List<FootPrintRow> Rows { get; set; } = new();
}
