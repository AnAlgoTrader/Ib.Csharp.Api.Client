using System;

namespace Ib.Csharp.Api.Client.Implementation;

public enum IbMessageSeverity
{
    Info,
    Warning,
    Error
}

public static class IbMessageClassifier
{
    public static IbMessageSeverity Classify(string message)
    {
        var lower = message.ToLowerInvariant();

        if (lower.Contains("farm connection is ok") ||
            lower.Contains("data farm is connected") ||
            lower.Contains("inactive but should be available upon demand") ||
            lower.Contains("data maintained"))
            return IbMessageSeverity.Info;

        if (lower.Contains("farm is disconnected") ||
            lower.Contains("has been lost") ||
            lower.Contains("data lost") ||
            lower.Contains("is broken"))
            return IbMessageSeverity.Warning;

        return IbMessageSeverity.Error;
    }
}
