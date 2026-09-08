namespace Ib.Csharp.Api.Client.Implementation.Model;

public class RealTimeBar
{
    public long Date { get; set; }
    public double Open { get; set; }
    public double High { get; set; }
    public double Low { get; set; }
    public double Close { get; set; }
    public decimal Volume { get; set; }
    public decimal Wap { get; set; }
}