using IBApi;

namespace Ib.Csharp.Api.Client.Implementation.Model;

public class PriceTickByTickUpdate
{
    public int ReqId { get; set; }
    public long Time { get; set; }
    public double Bid { get; set; }
    public double Ask { get; set; }
    public decimal BidSize { get; set; }
    public decimal AskSize { get; set; }
    public TickAttribBidAsk TickAttribBidAsk { get; set; }
}