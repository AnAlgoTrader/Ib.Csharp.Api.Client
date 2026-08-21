using IBApi;

namespace Ib.Csharp.Api.Client.Implementation.Model;

public class PositionUpdate
{
    public double Quantity { get; set; }
    public string AccountName { get; set; }
    public double AverageCost { get; set; }
    public Contract Contract { get; set; }
    public double MarketPrice { get; set; }
    public double MarketValue { get; set; }
    public double RealizedPnl { get; set; }
    public double UnrealizedPnl { get; set; }
}