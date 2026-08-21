using IBApi;

namespace Ib.Csharp.Api.Client.Implementation.Model;

public class OpenOrderUpdate
{
    public int OrderId { get; set; }
    public Contract Contract { get; set; }
    public Order Order { get; set; }
    public OrderState OrderState { get; set; }
}
