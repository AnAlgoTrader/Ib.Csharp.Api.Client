namespace Ib.Csharp.Api.Client.Implementation.Model;

public class OptionComputation
{
    public double PvDividend { get; set; }
    public int TickerId { get; set; }
    public int Field { get; set; }
    public int TickAttrib { get; set; }
    public double ImpliedVolatility { get; set; }
    public double Delta { get; set; }
    public double OptPrice { get; set; }
    public double Gamma { get; set; }
    public double Vega { get; set; }
    public double Theta { get; set; }
    public double UndPrice { get; set; }
}