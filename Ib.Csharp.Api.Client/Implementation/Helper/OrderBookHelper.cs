using System.Collections.Generic;
using System.Linq;
using Ib.Csharp.Api.Client.Implementation.Model;
using IBApi;

namespace Ib.Csharp.Api.Client.Implementation.Helper;

public static class OrderBookHelper
{
    public static List<LadderRow> FormatAsLadder(OrderBookRow[] orderBook)
    {
        var asks = orderBook.Where(r => r.AskPrice > 0)
            .Select(r => new { Price = r.AskPrice, Size = r.AskSize, IsAsk = true });
        
        var bids = orderBook.Where(r => r.BidPrice > 0)
            .Select(r => new { Price = r.BidPrice, Size = r.BidSize, IsAsk = false });
        
        return asks.Concat(bids)
            .GroupBy(x => x.Price)
            .Select(g => new LadderRow
            {
                Price = g.Key,
                AskSize = g.FirstOrDefault(x => x.IsAsk)?.Size,
                BidSize = g.FirstOrDefault(x => !x.IsAsk)?.Size
            })
            .OrderByDescending(r => r.Price)
            .ToList();
    }
}
