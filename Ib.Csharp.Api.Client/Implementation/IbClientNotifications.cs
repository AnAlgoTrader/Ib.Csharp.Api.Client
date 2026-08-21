using System;
using IBApi.protobuf;

namespace Ib.Csharp.Api.Client.Implementation;

public partial class IbClient
{
    public Action<string> OnNotification { get; set; }
    
    public void NotifySubscriber(string message)
    {
        var output = $"{DateTime.Now:dd MMM yyyy - HH:mm:ss} - {message}";
        OnNotification?.Invoke(output);
    }
    
    public void error(int id, long errorTime, int errorCode, string errorMsg, string advancedOrderRejectJson)
    {
        NotifySubscriber($"Id:{id} - Code:{errorCode} - {errorMsg}");
        if (errorCode < 2000)
            OnHistoricalDataError(id, errorCode, errorMsg);
    }
    
    public void error(Exception e)
    {
        NotifySubscriber(e.Message);
    }
}