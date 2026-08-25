using IBApi;

namespace Ib.Csharp.Api.Client.Implementation.Model;

public class TradeTickByTickUpdate
{
    public int ReqId { get; set; }
    public int TickType { get; set; }
    public long Time { get; set; }
    public double Price { get; set; }
    public decimal Size { get; set; }
    public TickAttribLast TickAttribLast { get; set; }
    public string Exchange { get; set; }
    public string SpecialConditions { get; set; }
}