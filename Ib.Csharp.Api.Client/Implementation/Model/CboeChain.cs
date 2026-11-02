using System;
using System.Collections.Generic;

namespace Ib.Csharp.Api.Client.Implementation.Model;

public class CboeChain
{
    public DateTime ExpirationDate { get; set; }
    public List<OptionChainData> Data { get; set; }
}