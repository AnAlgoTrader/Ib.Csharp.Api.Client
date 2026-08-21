using System.Collections.Generic;

namespace Ib.Csharp.Api.Client.Implementation.Model;

public class SecurityDefinitionParameter
{
    public int ReqId { get; set; }
    public string Exchange { get; set; }
    public int UnderlyingConId { get; set; }
    public string TradingClass { get; set; }
    public string Multiplier { get; set; }
    public HashSet<string> Expirations { get; set; }
    public HashSet<double> Strikes { get; set; }
}