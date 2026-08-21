using IBApi;

namespace Ib.Csharp.Api.Client.Implementation.Model;

public class TradeUpdate
{
    public double LastTradedPrice { get; set; }
    public decimal LastTradedSize { get; set; }
    
    public void setPrice(int field, double price) {
        switch (field)
        {
            case TickType.LAST:
            {
                LastTradedPrice = price;
                break;
            }
        }
    }
    
    public void setSize(int field, decimal size) {
        switch (field)
        {
            case TickType.LAST_SIZE:
            {
                LastTradedSize = size;
                break;
            }
        }
    }
}