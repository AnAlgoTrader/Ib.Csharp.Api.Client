using IBApi;

namespace Ib.Csharp.Api.Client.Implementation.Model;

public class OptionChainData
{
    public double Strike { get; set; }
    public double UnderlyingPrice { get; set; }
    
    // Call Data
    public int CallRequestId { get; set; }
    public ContractDetails CallContract { get; set; }
    public double CallBid { get; set; }
    public double CallAsk { get; set; }
    public double CallIv { get; set; }
    public double CallDelta { get; set; }
    public double CallGamma { get; set; }
    public double CallVega { get; set; }
    public double CallOpenInterest { get; set; }
    
    // Put Data
    public int PutRequestId { get; set; }
    public ContractDetails PutContract { get; set; }
    public double PutBid { get; set; }
    public double PutAsk { get; set; }
    public double PutIv { get; set; }
    public double PutDelta { get; set; }
    public double PutGamma { get; set; }
    public double PutVega { get; set; }
    public double PutOpenInterest { get; set; }
    
    // Combined / Calculated
    public double TotalGamma => CallGamma + PutGamma;
    public double Gex { get; set; }
    public double Vex { get; set; }
    public double GammaWall { get; set; }
}