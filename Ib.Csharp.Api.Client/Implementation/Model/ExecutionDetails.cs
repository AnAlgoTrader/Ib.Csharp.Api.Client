using IBApi;

namespace Ib.Csharp.Api.Client.Implementation.Model;

public class ExecutionDetails
{
    public int RequestId { get; set; }
    public Contract Contract { get; set; }
    public Execution Execution { get; set; }
}