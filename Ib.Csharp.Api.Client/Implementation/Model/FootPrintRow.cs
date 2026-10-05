namespace Ib.Csharp.Api.Client.Implementation.Model;

public class FootPrintRow
{
    public double Level { get; set; }
    public double BidSize { get; set; }
    public double AskSize { get; set; }
    public double DeltaSize => BidSize - AskSize;
    public bool IsPoc { get; set; }
}