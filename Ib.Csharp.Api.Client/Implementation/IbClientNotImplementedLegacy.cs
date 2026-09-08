using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using IBApi;

namespace Ib.Csharp.Api.Client.Implementation;

public partial class IbClient
{
    private static void NotImplemented([CallerMemberName] string methodName = "")
    {
        Console.WriteLine($"Method '{methodName}' was called but is not implemented");
        throw new NotImplementedException($"Method '{methodName}' is not implemented");
    }

    public void error(string str)
    {
        NotImplemented();
    }

    public void currentTime(long time)
    {
        NotImplemented();
    }

    public void tickString(int tickerId, int field, string value)
    {
        //ignored
    }

    public void tickGeneric(int tickerId, int field, double value)
    {
        //ignored
    }

    public void tickEFP(int tickerId, int tickType, double basisPoints, string formattedBasisPoints, double impliedFuture,
        int holdDays, string futureLastTradeDate, double dividendImpact, double dividendsToLastTradeDate)
    {
        NotImplemented();
    }

    public void deltaNeutralValidation(int reqId, DeltaNeutralContract deltaNeutralContract)
    {
        NotImplemented();
    }

    public void tickSnapshotEnd(int tickerId)
    {
        NotImplemented();
    }

    public void accountSummary(int reqId, string account, string tag, string value, string currency)
    {
        NotImplemented();
    }

    public void accountSummaryEnd(int reqId)
    {
        NotImplemented();
    }

    public void bondContractDetails(int reqId, ContractDetails contract)
    {
        NotImplemented();
    }
    
    public void updateAccountTime(string timestamp)
    {
        //using time from the account download end
    }

    public void openOrderEnd()
    {
        //ignored
    }

    public void execDetailsEnd(int reqId)
    {
        //ignored
    }

    public void fundamentalData(int reqId, string data)
    {
        NotImplemented();
    }

    public void historicalDataUpdate(int reqId, Bar bar)
    {
        NotImplemented();
    }

    public void marketDataType(int reqId, int marketDataType)
    {
        //ignored
    }

    public void updateNewsBulletin(int msgId, int msgType, string message, string origExchange)
    {
        NotImplemented();
    }

    public void position(string account, Contract contract, decimal pos, double avgCost)
    {
        NotImplemented();
    }

    public void positionEnd()
    {
        NotImplemented();
    }

    public void scannerParameters(string xml)
    {
        NotImplemented();
    }

    public void scannerData(int reqId, int rank, ContractDetails contractDetails, string distance, string benchmark,
        string projection, string legsStr)
    {
        NotImplemented();
    }

    public void scannerDataEnd(int reqId)
    {
        NotImplemented();
    }

    public void receiveFA(int faDataType, string faXmlData)
    {
        NotImplemented();
    }

    public void verifyMessageAPI(string apiData)
    {
        NotImplemented();
    }

    public void verifyCompleted(bool isSuccessful, string errorText)
    {
        NotImplemented();
    }

    public void verifyAndAuthMessageAPI(string apiData, string xyzChallenge)
    {
        NotImplemented();
    }

    public void verifyAndAuthCompleted(bool isSuccessful, string errorText)
    {
        NotImplemented();
    }

    public void displayGroupList(int reqId, string groups)
    {
        NotImplemented();
    }

    public void displayGroupUpdated(int reqId, string contractInfo)
    {
        NotImplemented();
    }

    public void positionMulti(int requestId, string account, string modelCode, Contract contract, decimal pos, double avgCost)
    {
        NotImplemented();
    }

    public void positionMultiEnd(int requestId)
    {
        NotImplemented();
    }

    public void accountUpdateMulti(int requestId, string account, string modelCode, string key, string value, string currency)
    {
        NotImplemented();
    }

    public void accountUpdateMultiEnd(int requestId)
    {
        NotImplemented();
    }

    public void softDollarTiers(int reqId, SoftDollarTier[] tiers)
    {
        NotImplemented();
    }

    public void familyCodes(FamilyCode[] familyCodes)
    {
        NotImplemented();
    }

    public void symbolSamples(int reqId, ContractDescription[] contractDescriptions)
    {
        NotImplemented();
    }

    public void mktDepthExchanges(DepthMktDataDescription[] depthMktDataDescriptions)
    {
        NotImplemented();
    }

    public void tickNews(int tickerId, long timeStamp, string providerCode, string articleId, string headline, string extraData)
    {
        NotImplemented();
    }

    public void smartComponents(int reqId, Dictionary<int, KeyValuePair<string, char>> theMap)
    {
        NotImplemented();
    }

    public void tickReqParams(int tickerId, double minTick, string bboExchange, int snapshotPermissions)
    {
        //ignored
    }

    public void newsProviders(NewsProvider[] newsProviders)
    {
        NotImplemented();
    }

    public void newsArticle(int requestId, int articleType, string articleText)
    {
        NotImplemented();
    }

    public void historicalNews(int requestId, string time, string providerCode, string articleId, string headline)
    {
        NotImplemented();
    }

    public void historicalNewsEnd(int requestId, bool hasMore)
    {
        NotImplemented();
    }

    public void headTimestamp(int reqId, string headTimestamp)
    {
        NotImplemented();
    }

    public void histogramData(int reqId, HistogramEntry[] data)
    {
        NotImplemented();
    }

    public void rerouteMktDataReq(int reqId, int conId, string exchange)
    {
        NotImplemented();
    }

    public void rerouteMktDepthReq(int reqId, int conId, string exchange)
    {
        NotImplemented();
    }

    public void marketRule(int marketRuleId, PriceIncrement[] priceIncrements)
    {
        NotImplemented();
    }

    public void pnl(int reqId, double dailyPnL, double unrealizedPnL, double realizedPnL)
    {
        NotImplemented();
    }

    public void pnlSingle(int reqId, decimal pos, double dailyPnL, double unrealizedPnL, double realizedPnL, double value)
    {
        NotImplemented();
    }

    public void historicalTicks(int reqId, HistoricalTick[] ticks, bool done)
    {
        NotImplemented();
    }

    public void tickByTickMidPoint(int reqId, long time, double midPoint)
    {
        NotImplemented();
    }

    public void orderBound(long permId, int clientId, int orderId)
    {
        NotImplemented();
    }

    public void completedOrder(Contract contract, Order order, OrderState orderState)
    {
        NotImplemented();
    }

    public void completedOrdersEnd()
    {
        NotImplemented();
    }

    public void replaceFAEnd(int reqId, string text)
    {
        NotImplemented();
    }

    public void wshMetaData(int reqId, string dataJson)
    {
        NotImplemented();
    }

    public void wshEventData(int reqId, string dataJson)
    {
        NotImplemented();
    }

    public void historicalSchedule(int reqId, string startDateTime, string endDateTime, string timeZone,
        HistoricalSession[] sessions)
    {
        NotImplemented();
    }

    public void userInfo(int reqId, string whiteBrandingId)
    {
        NotImplemented();
    }

    public void currentTimeInMillis(long timeInMillis)
    {
        NotImplemented();
    }
}