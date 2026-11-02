using System;
using System.Collections.Generic;

namespace Ib.Csharp.Api.Client.Implementation.Model;

public class CboeChain
{
    public string Symbol { get; set; }
    public DateTime ExpirationDate { get; set; }
    public List<OptionChainData> Data { get; set; }
}