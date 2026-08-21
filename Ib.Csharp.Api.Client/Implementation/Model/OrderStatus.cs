namespace Ib.Csharp.Api.Client.Implementation.Model;

public class OrderStatus
{
    public int OrderId { get; set; }
    public string Status { get; set; }
    public decimal Filled { get; set; }
    public decimal Remaining { get; set; }
    public double AvgFillPrice { get; set; }
    public long PermId { get; set; }
    public int ParentId { get; set; }
    public double LastFillPrice { get; set; }
    public int ClientId { get; set; }
    public string WhyHeld { get; set; }
    public double MktCapPrice { get; set; }
}