using System;
using IBApi.protobuf;

namespace Ib.Csharp.Api.Client.Implementation;

public partial class IbClient
{
    public Action<string> OnNotification { get; set; }
    public Action<int, int, string> OnInfoMessage { get; set; }
    public Action<int, int, string> OnWarningMessage { get; set; }
    public Action<int, int, string> OnErrorMessage { get; set; }

    public void NotifySubscriber(string message)
    {
        var output = $"{DateTime.Now:dd MMM yyyy - HH:mm:ss} - {message}";
        OnNotification?.Invoke(output);
    }

    public void error(int id, long errorTime, int errorCode, string errorMsg, string advancedOrderRejectJson)
    {
        NotifySubscriber($"Id:{id} - Code:{errorCode} - {errorMsg}");

        switch (IbMessageClassifier.Classify(errorMsg))
        {
            case IbMessageSeverity.Info:
                OnInfoMessage?.Invoke(id, errorCode, errorMsg);
                break;
            case IbMessageSeverity.Warning:
                OnWarningMessage?.Invoke(id, errorCode, errorMsg);
                break;
            case IbMessageSeverity.Error:
                OnErrorMessage?.Invoke(id, errorCode, errorMsg);
                OnHistoricalDataError(id, errorCode, errorMsg);
                break;
        }
    }
    
    public void error(Exception e)
    {
        NotifySubscriber(e.Message);
    }
}