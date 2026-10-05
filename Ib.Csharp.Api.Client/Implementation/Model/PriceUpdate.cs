using IBApi;

namespace Ib.Csharp.Api.Client.Implementation.Model;

public class PriceUpdate
{
    public int RequestId { get; set; }
    public double BidPrice { get; set; }
    public double AskPrice { get; set; }
    public decimal BidSize { get; set; }
    public decimal AskSize { get; set; }
    public decimal CallOpenInterest { get; set; }
    public decimal PutOpenInterest { get; set; }
    
    public void setPrice(int field, double price) {
        switch (field)
        {
            case TickType.BID:
            case TickType.DELAYED_BID:
            {
                BidPrice = price;
                break;
            }
            case TickType.ASK:
            case TickType.DELAYED_ASK:
            {
                AskPrice = price;
                break;
            }
        }
    }
    
    public void setSize(int field, decimal size) {
        switch (field)
        {
            case TickType.BID_SIZE:
            case TickType.DELAYED_BID_SIZE:
            {
                BidSize = size;
                break;
            }
            case TickType.ASK_SIZE:
            case TickType.DELAYED_ASK_SIZE:
            {
                AskSize = size;
                break;
            }
            case TickType.OPTION_CALL_OPEN_INTEREST:
            {
                CallOpenInterest = size;
                break;
            }
            case TickType.OPTION_PUT_OPEN_INTEREST:
            {
                PutOpenInterest = size;
                break;
            }
        }
    }
}