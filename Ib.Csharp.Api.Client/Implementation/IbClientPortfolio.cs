using System;
using Ib.Csharp.Api.Client.Implementation.Model;
using IBApi;

namespace Ib.Csharp.Api.Client.Implementation;

public partial class IbClient
{
    public Action<string> OnAccountListUpdate { get; set; }
    
    public void managedAccounts(string accountsList)
    {
        OnAccountListUpdate?.Invoke(accountsList);
    }
    
    private readonly AccountUpdate _account = new();
    public Action<AccountUpdate> OnAccountUpdate { get; set; }
    public Action<PositionUpdate> OnPositionUpdate { get; set; }
    
    public void SubscribeToPortfolioUpdates(string accountCode)
    {
        _clientSocket.reqAccountUpdates(true, accountCode);
    }
    
    public void updateAccountValue(string key, string value, string currency, string accountName)
    {
        _account.setAccountValue(key, value, currency);
    }

    public void accountDownloadEnd(string account)
    {
        _account.UpdateOn = DateTime.Now;
        OnAccountUpdate?.Invoke(_account);
    }
    
    public void updatePortfolio(Contract contract, decimal position, double marketPrice, double marketValue, double averageCost,
        double unrealizedPnl, double realizedPnl, string accountName)
    {
        var positionUpdate = new PositionUpdate
        {
            Quantity = Convert.ToDouble(position),
            AccountName = accountName,
            AverageCost = averageCost,
            Contract = contract,
            MarketPrice = marketPrice,
            MarketValue = marketValue,
            RealizedPnl = unrealizedPnl,
            UnrealizedPnl = realizedPnl
        };
        OnPositionUpdate?.Invoke(positionUpdate);
    }
}