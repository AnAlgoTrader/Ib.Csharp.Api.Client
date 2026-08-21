using System;
using System.Threading;
using IBApi;

namespace Ib.Csharp.Api.Client.Implementation;

public partial class IbClient : EWrapper, IDisposable
{
    private readonly EClientSocket _clientSocket;
    private readonly EReaderSignal _signal;
    private Thread _messageThread;
    private volatile bool _disposed;

    public IbClient()
    {
        _signal = new EReaderMonitorSignal();
        _clientSocket = new EClientSocket(this, _signal);
    }

    public void Start(string host, int port, int id)
    {
        if (_clientSocket.IsConnected())
            return;
            
        _clientSocket.SetConnectOptions("+PACEAPI");
        _clientSocket.eConnect(host, port, id);
        var reader = new EReader(_clientSocket, _signal);
        reader.Start();
        
        _messageThread = new Thread(() => 
        { 
            while (_clientSocket.IsConnected() && !_disposed) 
            { 
                _signal.waitForSignal(); 
                reader.processMsgs(); 
            } 
        }) { IsBackground = true };
        _messageThread.Start();
    }
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed) return;
        
        _disposed = true;
        _clientSocket.eDisconnect();
        _messageThread?.Join(1000);
    }
    
    public bool IsSocketConnected() => _clientSocket.IsConnected();

    public Action OnConnectionClosed { get; set; }

    public void connectionClosed()
    {
        NotifySubscriber("Connection closed");
        OnConnectionClosed?.Invoke();
    }
    
    public void connectAck()
    {
        NotifySubscriber("Connection acknowledged");
    }
}