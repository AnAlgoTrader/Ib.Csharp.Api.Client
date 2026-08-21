using System;

namespace Ib.Csharp.Api.Client.Implementation.Model;

public class AccountUpdate
{
    public DateTime UpdateOn { get; set; }
    public string AccountCode { get; private set; }
    public double AvailableFunds { get; private set; }
    public double BuyingPower { get; private set; }
    public double CashEuro { get; private set; }
    public double CashUsd { get; private set; }

    public void setAccountValue(string key, string value, string currency)
    {
        switch (key)
        {
            case "AccountCode":
            {
                AccountCode = value;
                break;
            }
            case "AvailableFunds":
            {
                AvailableFunds = double.Parse(value);
                break;
            }
            case "BuyingPower":
            {
                BuyingPower = double.Parse(value);
                break;
            }
            case "CashBalance":
            {
                switch (currency)
                {
                    case "EUR":
                        CashEuro = double.Parse(value);
                        break;
                    case "USD":
                        CashUsd = double.Parse(value);
                        break;
                }
                break;
            }
        }
    }
}