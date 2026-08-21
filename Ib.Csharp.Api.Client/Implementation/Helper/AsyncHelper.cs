using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ib.Csharp.Api.Client.Implementation.Helper;

public static class AsyncHelper
{
    public static async Task<T> WaitForCallback<T>(Action<Action<T>> subscribe, int timeoutMs = 5000)
    {
        var tcs = new TaskCompletionSource<T>();
        var cts = new CancellationTokenSource(timeoutMs);
            
        cts.Token.Register(() => tcs.TrySetCanceled());
        subscribe(value => tcs.TrySetResult(value));
            
        return await tcs.Task;
    }
    public static async Task Wait(int milliseconds = 1000)
    {
        await Task.Delay(milliseconds);
    }
}