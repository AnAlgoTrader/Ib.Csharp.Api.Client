/* Copyright (C) 2025 Interactive Brokers LLC. All rights reserved. This code is subject to the terms
 * and conditions of the IB API Non-Commercial License or the IB API Commercial License, as applicable. */

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace IBApi
{
    internal class EDecoder : IDecoder
    {
        private readonly EClientMsgSink eClientMsgSink;
        private readonly EWrapper eWrapper;
        private int serverVersion;
        private BinaryReader dataReader;
        private int nDecodedLen;

        public EDecoder(int serverVersion, EWrapper callback, EClientMsgSink sink = null)
        {
            this.serverVersion = serverVersion;
            eWrapper = callback;
            eClientMsgSink = sink;
        }

        public int ParseAndProcessMsg(byte[] buf)
        {
            dataReader?.Dispose();

            dataReader = new BinaryReader(new MemoryStream(buf));
            nDecodedLen = 0;

            if (serverVersion == 0)
            {
                try
                {
                    ProcessConnectAck();
                    return nDecodedLen;
                }
                catch (Exception ex)
                {
                    eWrapper.error(IncomingMessage.NotValid, Util.CurrentTimeMillis(), EClientErrors.SOCKET_EXCEPTION.Code, EClientErrors.SOCKET_EXCEPTION.Message + ex.Message, "");
                    return 0;
                }
            }

            int msgId = serverVersion >= MinServerVer.MIN_SERVER_VER_PROTOBUF ? ReadRawInt() : ReadInt();

            return ProcessIncomingMessage(msgId, buf.Length) ? nDecodedLen : -1;
        }

        private void ProcessConnectAck()
        {
            serverVersion = ReadInt();

            var serverTime = "";

            if (serverVersion >= 20) serverTime = ReadString();

            eClientMsgSink?.serverVersion(serverVersion, serverTime);

            eWrapper.connectAck();
        }

        private bool ProcessIncomingMessage(int incomingMessage, int len)
        {
            if (incomingMessage == IncomingMessage.NotValid) return false;

            bool useProtoBuf = false;
            if (incomingMessage > Constants.PROTOBUF_MSG_ID)
            {
                useProtoBuf = true;
                incomingMessage -= Constants.PROTOBUF_MSG_ID;
            }

            if (useProtoBuf)
            {
                switch (incomingMessage)
                {
                    case IncomingMessage.OrderStatus:
                        OrderStatusEventProtoBuf(len);
                        break;
                    case IncomingMessage.Error:
                        ErrorEventProtoBuf(len);
                        break;
                    case IncomingMessage.OpenOrder:
                        OpenOrderEventProtoBuf(len);
                        break;
                    case IncomingMessage.OpenOrderEnd:
                        OpenOrderEndEventProtoBuf(len);
                        break;
                    case IncomingMessage.ExecutionData:
                        ExecutionDataEventProtoBuf(len);
                        break;
                    case IncomingMessage.ExecutionDataEnd:
                        ExecutionDataEndEventProtoBuf(len);
                        break;
                    case IncomingMessage.CompletedOrder:
                        CompletedOrderEventProtoBuf(len);
                        break;
                    case IncomingMessage.CompletedOrdersEnd:
                        CompletedOrdersEndEventProtoBuf(len);
                        break;
                    case IncomingMessage.OrderBound:
                        OrderBoundEventProtoBuf(len);
                        break;
                    case IncomingMessage.ContractData:
                        ContractDataEventProtoBuf(len);
                        break;
                    case IncomingMessage.BondContractData:
                        BondContractDataEventProtoBuf(len);
                        break;
                    case IncomingMessage.ContractDataEnd:
                        ContractDataEndEventProtoBuf(len);
                        break;
                    case IncomingMessage.TickPrice:
                        TickPriceEventProtoBuf(len);
                        break;
                    case IncomingMessage.TickSize:
                        TickSizeEventProtoBuf(len);
                        break;
                    case IncomingMessage.MarketDepth:
                        MarketDepthEventProtoBuf(len);
                        break;
                    case IncomingMessage.MarketDepthL2:
                        MarketDepthL2EventProtoBuf(len);
                        break;
                    case IncomingMessage.TickOptionComputation:
                        TickOptionComputationEventProtoBuf(len);
                        break;
                    case IncomingMessage.TickGeneric:
                        TickGenericEventProtoBuf(len);
                        break;
                    case IncomingMessage.TickString:
                        TickStringEventProtoBuf(len);
                        break;
                    case IncomingMessage.TickSnapshotEnd:
                        TickSnapshotEndEventProtoBuf(len);
                        break;
                    case IncomingMessage.MarketDataType:
                        MarketDataTypeEventProtoBuf(len);
                        break;
                    case IncomingMessage.TickReqParams:
                        TickReqParamsEventProtoBuf(len);
                        break;
                    case IncomingMessage.AccountValue:
                        AccountValueEventProtoBuf(len);
                        break;
                    case IncomingMessage.PortfolioValue:
                        PortfolioValueEventProtoBuf(len);
                        break;
                    case IncomingMessage.AccountUpdateTime:
                        AccountUpdateTimeEventProtoBuf(len);
                        break;
                    case IncomingMessage.AccountDownloadEnd:
                        AccountDataEndEventProtoBuf(len);
                        break;
                    case IncomingMessage.ManagedAccounts:
                        ManagedAccountsEventProtoBuf(len);
                        break;
                    case IncomingMessage.Position:
                        PositionEventProtoBuf(len);
                        break;
                    case IncomingMessage.PositionEnd:
                        PositionEndEventProtoBuf(len);
                        break;
                    case IncomingMessage.AccountSummary:
                        AccountSummaryEventProtoBuf(len);
                        break;
                    case IncomingMessage.AccountSummaryEnd:
                        AccountSummaryEndEventProtoBuf(len);
                        break;
                    case IncomingMessage.PositionMulti:
                        PositionMultiEventProtoBuf(len);
                        break;
                    case IncomingMessage.PositionMultiEnd:
                        PositionMultiEndEventProtoBuf(len);
                        break;
                    case IncomingMessage.AccountUpdateMulti:
                        AccountUpdateMultiEventProtoBuf(len);
                        break;
                    case IncomingMessage.AccountUpdateMultiEnd:
                        AccountUpdateMultiEndEventProtoBuf(len);
                        break;
                    case IncomingMessage.HistoricalData:
                        HistoricalDataEventProtoBuf(len);
                        break;
                    case IncomingMessage.HistoricalDataUpdate:
                        HistoricalDataUpdateEventProtoBuf(len);
                        break;
                    case IncomingMessage.HistoricalDataEnd:
                        HistoricalDataEndEventProtoBuf(len);
                        break;
                    case IncomingMessage.RealTimeBars:
                        RealTimeBarEventProtoBuf(len);
                        break;
                    case IncomingMessage.HeadTimestamp:
                        HeadTimestampEventProtoBuf(len);
                        break;
                    case IncomingMessage.HistogramData:
                        HistogramDataEventProtoBuf(len);
                        break;
                    case IncomingMessage.HistoricalTick:
                        HistoricalTicksEventProtoBuf(len);
                        break;
                    case IncomingMessage.HistoricalTickBidAsk:
                        HistoricalTicksBidAskEventProtoBuf(len);
                        break;
                    case IncomingMessage.HistoricalTickLast:
                        HistoricalTicksLastEventProtoBuf(len);
                        break;
                    case IncomingMessage.TickByTick:
                        TickByTickEventProtoBuf(len);
                        break;
                    case IncomingMessage.NewsBulletins:
                        NewsBulletinEventProtoBuf(len);
                        break;
                    case IncomingMessage.NewsArticle:
                        NewsArticleEventProtoBuf(len);
                        break;
                    case IncomingMessage.NewsProviders:
                        NewsProvidersEventProtoBuf(len);
                        break;
                    case IncomingMessage.HistoricalNews:
                        HistoricalNewsEventProtoBuf(len);
                        break;
                    case IncomingMessage.HistoricalNewsEnd:
                        HistoricalNewsEndEventProtoBuf(len);
                        break;
                    case IncomingMessage.WshMetaData:
                        WshMetaDataEventProtoBuf(len);
                        break;
                    case IncomingMessage.WshEventData:
                        WshEventDataEventProtoBuf(len);
                        break;
                    case IncomingMessage.TickNews:
                        TickNewsEventProtoBuf(len);
                        break;
                    case IncomingMessage.ScannerParameters:
                        ScannerParametersEventProtoBuf(len);
                        break;
                    case IncomingMessage.ScannerData:
                        ScannerDataEventProtoBuf(len);
                        break;
                    case IncomingMessage.FundamentalData:
                        FundamentalsDataEventProtoBuf(len);
                        break;
                    case IncomingMessage.PnL:
                        PnLEventProtoBuf(len);
                        break;
                    case IncomingMessage.PnLSingle:
                        PnLSingleEventProtoBuf(len);
                        break;
                    case IncomingMessage.ReceiveFA:
                        ReceiveFAEventProtoBuf(len);
                        break;
                    case IncomingMessage.ReplaceFAEnd:
                        ReplaceFAEndEventProtoBuf(len);
                        break;
                    case IncomingMessage.CommissionsAndFeesReport:
                        CommissionAndFeesReportEventProtoBuf(len);
                        break;
                    case IncomingMessage.HistoricalSchedule:
                        HistoricalScheduleEventProtoBuf(len);
                        break;
                    case IncomingMessage.RerouteMktDataReq:
                        RerouteMktDataReqEventProtoBuf(len);
                        break;
                    case IncomingMessage.RerouteMktDepthReq:
                        RerouteMktDepthReqEventProtoBuf(len);
                        break;
                    case IncomingMessage.SecurityDefinitionOptionParameter:
                        SecurityDefinitionOptionParameterEventProtoBuf(len);
                        break;
                    case IncomingMessage.SecurityDefinitionOptionParameterEnd:
                        SecurityDefinitionOptionParameterEndEventProtoBuf(len);
                        break;
                    case IncomingMessage.SoftDollarTier:
                        SoftDollarTiersEventProtoBuf(len);
                        break;
                    case IncomingMessage.FamilyCodes:
                        FamilyCodesEventProtoBuf(len);
                        break;
                    case IncomingMessage.SymbolSamples:
                        SymbolSamplesEventProtoBuf(len);
                        break;
                    case IncomingMessage.SmartComponents:
                        SmartComponentsEventProtoBuf(len);
                        break;
                    case IncomingMessage.MarketRule:
                        MarketRuleEventProtoBuf(len);
                        break;
                    case IncomingMessage.UserInfo:
                        UserInfoEventProtoBuf(len);
                        break;
                    case IncomingMessage.NextValidId:
                        NextValidIdEventProtoBuf(len);
                        break;
                    case IncomingMessage.CurrentTime:
                        CurrentTimeEventProtoBuf(len);
                        break;
                    case IncomingMessage.CurrentTimeInMillis:
                        CurrentTimeInMillisEventProtoBuf(len);
                        break;
                    case IncomingMessage.VerifyMessageApi:
                        VerifyMessageApiEventProtoBuf(len);
                        break;
                    case IncomingMessage.VerifyCompleted:
                        VerifyCompletedEventProtoBuf(len);
                        break;
                    case IncomingMessage.DisplayGroupList:
                        DisplayGroupListEventProtoBuf(len);
                        break;
                    case IncomingMessage.DisplayGroupUpdated:
                        DisplayGroupUpdatedEventProtoBuf(len);
                        break;
                    case IncomingMessage.MktDepthExchanges:
                        MarketDepthExchangesEventProtoBuf(len);
                        break;
                    case IncomingMessage.ConfigResponse:
                        ConfigResponseEventProtoBuf(len);
                        break;
                    case IncomingMessage.UpdateConfigResponse:
                        UpdateConfigResponseEventProtoBuf(len);
                        break;

                    default:
                        eWrapper.error(IncomingMessage.NotValid, Util.CurrentTimeMillis(), EClientErrors.UNKNOWN_ID.Code, EClientErrors.UNKNOWN_ID.Message, "");
                        return false;
                }
            }
            else
            {
                switch (incomingMessage)
                {
                    case IncomingMessage.TickPrice:
                        TickPriceEvent();
                        break;
                    case IncomingMessage.TickSize:
                        TickSizeEvent();
                        break;
                    case IncomingMessage.TickString:
                        TickStringEvent();
                        break;
                    case IncomingMessage.TickGeneric:
                        TickGenericEvent();
                        break;
                    case IncomingMessage.TickEFP:
                        TickEFPEvent();
                        break;
                    case IncomingMessage.TickSnapshotEnd:
                        TickSnapshotEndEvent();
                        break;
                    case IncomingMessage.Error:
                        ErrorEvent();
                        break;
                    case IncomingMessage.CurrentTime:
                        CurrentTimeEvent();
                        break;
                    case IncomingMessage.ManagedAccounts:
                        ManagedAccountsEvent();
                        break;
                    case IncomingMessage.NextValidId:
                        NextValidIdEvent();
                        break;
                    case IncomingMessage.DeltaNeutralValidation:
                        DeltaNeutralValidationEvent();
                        break;
                    case IncomingMessage.TickOptionComputation:
                        TickOptionComputationEvent();
                        break;
                    case IncomingMessage.AccountSummary:
                        AccountSummaryEvent();
                        break;
                    case IncomingMessage.AccountSummaryEnd:
                        AccountSummaryEndEvent();
                        break;
                    case IncomingMessage.AccountValue:
                        AccountValueEvent();
                        break;
                    case IncomingMessage.PortfolioValue:
                        PortfolioValueEvent();
                        break;
                    case IncomingMessage.AccountUpdateTime:
                        AccountUpdateTimeEvent();
                        break;
                    case IncomingMessage.AccountDownloadEnd:
                        AccountDownloadEndEvent();
                        break;
                    case IncomingMessage.OrderStatus:
                        OrderStatusEvent();
                        break;
                    case IncomingMessage.OpenOrder:
                        OpenOrderEvent();
                        break;
                    case IncomingMessage.OpenOrderEnd:
                        OpenOrderEndEvent();
                        break;
                    case IncomingMessage.ContractData:
                        ContractDataEvent();
                        break;
                    case IncomingMessage.ContractDataEnd:
                        ContractDataEndEvent();
                        break;
                    case IncomingMessage.ExecutionData:
                        ExecutionDataEvent();
                        break;
                    case IncomingMessage.ExecutionDataEnd:
                        ExecutionDataEndEvent();
                        break;
                    case IncomingMessage.CommissionsAndFeesReport:
                        CommissionAndFeesReportEvent();
                        break;
                    case IncomingMessage.FundamentalData:
                        FundamentalDataEvent();
                        break;
                    case IncomingMessage.HistoricalData:
                        HistoricalDataEvent();
                        break;
                    case IncomingMessage.MarketDataType:
                        MarketDataTypeEvent();
                        break;
                    case IncomingMessage.MarketDepth:
                        MarketDepthEvent();
                        break;
                    case IncomingMessage.MarketDepthL2:
                        MarketDepthL2Event();
                        break;
                    case IncomingMessage.NewsBulletins:
                        NewsBulletinsEvent();
                        break;
                    case IncomingMessage.Position:
                        PositionEvent();
                        break;
                    case IncomingMessage.PositionEnd:
                        PositionEndEvent();
                        break;
                    case IncomingMessage.RealTimeBars:
                        RealTimeBarsEvent();
                        break;
                    case IncomingMessage.ScannerParameters:
                        ScannerParametersEvent();
                        break;
                    case IncomingMessage.ScannerData:
                        ScannerDataEvent();
                        break;
                    case IncomingMessage.ReceiveFA:
                        ReceiveFAEvent();
                        break;
                    case IncomingMessage.BondContractData:
                        BondContractDetailsEvent();
                        break;
                    case IncomingMessage.VerifyMessageApi:
                        VerifyMessageApiEvent();
                        break;
                    case IncomingMessage.VerifyCompleted:
                        VerifyCompletedEvent();
                        break;
                    case IncomingMessage.DisplayGroupList:
                        DisplayGroupListEvent();
                        break;
                    case IncomingMessage.DisplayGroupUpdated:
                        DisplayGroupUpdatedEvent();
                        break;
                    case IncomingMessage.VerifyAndAuthMessageApi:
                        VerifyAndAuthMessageApiEvent();
                        break;
                    case IncomingMessage.VerifyAndAuthCompleted:
                        VerifyAndAuthCompletedEvent();
                        break;
                    case IncomingMessage.PositionMulti:
                        PositionMultiEvent();
                        break;
                    case IncomingMessage.PositionMultiEnd:
                        PositionMultiEndEvent();
                        break;
                    case IncomingMessage.AccountUpdateMulti:
                        AccountUpdateMultiEvent();
                        break;
                    case IncomingMessage.AccountUpdateMultiEnd:
                        AccountUpdateMultiEndEvent();
                        break;
                    case IncomingMessage.SecurityDefinitionOptionParameter:
                        SecurityDefinitionOptionParameterEvent();
                        break;
                    case IncomingMessage.SecurityDefinitionOptionParameterEnd:
                        SecurityDefinitionOptionParameterEndEvent();
                        break;
                    case IncomingMessage.SoftDollarTier:
                        SoftDollarTierEvent();
                        break;
                    case IncomingMessage.FamilyCodes:
                        FamilyCodesEvent();
                        break;
                    case IncomingMessage.SymbolSamples:
                        SymbolSamplesEvent();
                        break;
                    case IncomingMessage.MktDepthExchanges:
                        MktDepthExchangesEvent();
                        break;
                    case IncomingMessage.TickNews:
                        TickNewsEvent();
                        break;
                    case IncomingMessage.TickReqParams:
                        TickReqParamsEvent();
                        break;
                    case IncomingMessage.SmartComponents:
                        SmartComponentsEvent();
                        break;
                    case IncomingMessage.NewsProviders:
                        NewsProvidersEvent();
                        break;
                    case IncomingMessage.NewsArticle:
                        NewsArticleEvent();
                        break;
                    case IncomingMessage.HistoricalNews:
                        HistoricalNewsEvent();
                        break;
                    case IncomingMessage.HistoricalNewsEnd:
                        HistoricalNewsEndEvent();
                        break;
                    case IncomingMessage.HeadTimestamp:
                        HeadTimestampEvent();
                        break;
                    case IncomingMessage.HistogramData:
                        HistogramDataEvent();
                        break;
                    case IncomingMessage.HistoricalDataUpdate:
                        HistoricalDataUpdateEvent();
                        break;
                    case IncomingMessage.RerouteMktDataReq:
                        RerouteMktDataReqEvent();
                        break;
                    case IncomingMessage.RerouteMktDepthReq:
                        RerouteMktDepthReqEvent();
                        break;
                    case IncomingMessage.MarketRule:
                        MarketRuleEvent();
                        break;
                    case IncomingMessage.PnL:
                        PnLEvent();
                        break;
                    case IncomingMessage.PnLSingle:
                        PnLSingleEvent();
                        break;
                    case IncomingMessage.HistoricalTick:
                        HistoricalTickEvent();
                        break;
                    case IncomingMessage.HistoricalTickBidAsk:
                        HistoricalTickBidAskEvent();
                        break;
                    case IncomingMessage.HistoricalTickLast:
                        HistoricalTickLastEvent();
                        break;
                    case IncomingMessage.TickByTick:
                        TickByTickEvent();
                        break;
                    case IncomingMessage.OrderBound:
                        OrderBoundEvent();
                        break;
                    case IncomingMessage.CompletedOrder:
                        CompletedOrderEvent();
                        break;
                    case IncomingMessage.CompletedOrdersEnd:
                        CompletedOrdersEndEvent();
                        break;
                    case IncomingMessage.ReplaceFAEnd:
                        ReplaceFAEndEvent();
                        break;
                    case IncomingMessage.WshMetaData:
                        ProcessWshMetaData();
                        break;
                    case IncomingMessage.WshEventData:
                        ProcessWshEventData();
                        break;
                    case IncomingMessage.HistoricalSchedule:
                        ProcessHistoricalScheduleEvent();
                        break;
                    case IncomingMessage.UserInfo:
                        ProcessUserInfoEvent();
                        break;
                    case IncomingMessage.HistoricalDataEnd:
                        HistoricalDataEndEvent();
                        break;
                    case IncomingMessage.CurrentTimeInMillis:
                        ProcessCurrentTimeInMillisEvent();
                        break;
                    default:
                        eWrapper.error(IncomingMessage.NotValid, Util.CurrentTimeMillis(), EClientErrors.UNKNOWN_ID.Code, EClientErrors.UNKNOWN_ID.Message, "");
                        return false;
                }
            }

            return true;
        }

        private void CompletedOrderEvent()
        {
            var contract = new Contract();
            var order = new Order();
            var orderState = new OrderState();
            var eOrderDecoder = new EOrderDecoder(this, contract, order, orderState, int.MaxValue, serverVersion);

            // read contract fields
            eOrderDecoder.readContractFields();

            // read order fields
            eOrderDecoder.readAction();
            eOrderDecoder.readTotalQuantity();
            eOrderDecoder.readOrderType();
            eOrderDecoder.readLmtPrice();
            eOrderDecoder.readAuxPrice();
            eOrderDecoder.readTIF();
            eOrderDecoder.readOcaGroup();
            eOrderDecoder.readAccount();
            eOrderDecoder.readOpenClose();
            eOrderDecoder.readOrigin();
            eOrderDecoder.readOrderRef();
            eOrderDecoder.readPermId();
            eOrderDecoder.readOutsideRth();
            eOrderDecoder.readHidden();
            eOrderDecoder.readDiscretionaryAmount();
            eOrderDecoder.readGoodAfterTime();
            eOrderDecoder.readFAParams();
            eOrderDecoder.readModelCode();
            eOrderDecoder.readGoodTillDate();
            eOrderDecoder.readRule80A();
            eOrderDecoder.readPercentOffset();
            eOrderDecoder.readSettlingFirm();
            eOrderDecoder.readShortSaleParams();
            eOrderDecoder.readBoxOrderParams();
            eOrderDecoder.readPegToStkOrVolOrderParams();
            eOrderDecoder.readDisplaySize();
            eOrderDecoder.readSweepToFill();
            eOrderDecoder.readAllOrNone();
            eOrderDecoder.readMinQty();
            eOrderDecoder.readOcaType();
            eOrderDecoder.readTriggerMethod();
            eOrderDecoder.readVolOrderParams(false);
            eOrderDecoder.readTrailParams();
            eOrderDecoder.readComboLegs();
            eOrderDecoder.readSmartComboRoutingParams();
            eOrderDecoder.readScaleOrderParams();
            eOrderDecoder.readHedgeParams();
            eOrderDecoder.readClearingParams();
            eOrderDecoder.readNotHeld();
            eOrderDecoder.readDeltaNeutral();
            eOrderDecoder.readAlgoParams();
            eOrderDecoder.readSolicited();
            eOrderDecoder.readOrderStatus();
            eOrderDecoder.readVolRandomizeFlags();
            eOrderDecoder.readPegToBenchParams();
            eOrderDecoder.readConditions();
            eOrderDecoder.readStopPriceAndLmtPriceOffset();
            eOrderDecoder.readCashQty();
            eOrderDecoder.readDontUseAutoPriceForHedge();
            eOrderDecoder.readIsOmsContainer();
            eOrderDecoder.readAutoCancelDate();
            eOrderDecoder.readFilledQuantity();
            eOrderDecoder.readRefFuturesConId();
            eOrderDecoder.readAutoCancelParent();
            eOrderDecoder.readShareholder();
            eOrderDecoder.readImbalanceOnly();
            eOrderDecoder.readRouteMarketableToBbo();
            eOrderDecoder.readParentPermId();
            eOrderDecoder.readCompletedTime();
            eOrderDecoder.readCompletedStatus();
            eOrderDecoder.readPegBestPegMidOrderAttributes();
            eOrderDecoder.readCustomerAccount();
            eOrderDecoder.readProfessionalCustomer();
            eOrderDecoder.readSubmitter();

            eWrapper.completedOrder(contract, order, orderState);
        }

        private void CompletedOrderEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.CompletedOrder completedOrderProto = protobuf.CompletedOrder.Parser.ParseFrom(byteArray);

            eWrapper.completedOrderProtoBuf(completedOrderProto);

            // set contract fields
            if (completedOrderProto.Contract == null)
            {
                return;
            }
            Contract contract = EDecoderUtils.decodeContract(completedOrderProto.Contract);

            // set order fields
            if (completedOrderProto.Order == null)
            {
                return;
            }
            Order order = EDecoderUtils.decodeOrder(int.MaxValue, completedOrderProto.Contract, completedOrderProto.Order);

            // set order state fields
            if (completedOrderProto.OrderState == null)
            {
                return;
            }
            OrderState orderState = EDecoderUtils.decodeOrderState(completedOrderProto.OrderState);

            eWrapper.completedOrder(contract, order, orderState);
        }

        private void CompletedOrdersEndEvent() => eWrapper.completedOrdersEnd();

        private void CompletedOrdersEndEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.CompletedOrdersEnd completedOrdersEndProto = protobuf.CompletedOrdersEnd.Parser.ParseFrom(byteArray);

            eWrapper.completedOrdersEndProtoBuf(completedOrdersEndProto);

            eWrapper.completedOrdersEnd();
        }

        private void OrderBoundEvent()
        {
            var permId = ReadLong();
            var clientId = ReadInt();
            var orderId = ReadInt();

            eWrapper.orderBound(permId, clientId, orderId);
        }

        private void OrderBoundEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.OrderBound orderBoundProto = protobuf.OrderBound.Parser.ParseFrom(byteArray);

            eWrapper.orderBoundProtoBuf(orderBoundProto);

            long permId = orderBoundProto.HasPermId ? orderBoundProto.PermId : long.MaxValue;
            int clientId = orderBoundProto.HasClientId ? orderBoundProto.ClientId : int.MaxValue;
            int orderId = orderBoundProto.HasOrderId ? orderBoundProto.OrderId : int.MaxValue;

            eWrapper.orderBound(permId, clientId, orderId);
        }

        private void TickByTickEvent()
        {
            var reqId = ReadInt();
            var tickType = ReadInt();
            var time = ReadLong();
            BitMask mask;

            switch (tickType)
            {
                case 0: // None
                    break;
                case 1: // Last
                case 2: // AllLast
                    var price = ReadDouble();
                    var size = ReadDecimal();
                    mask = new BitMask(ReadInt());
                    var tickAttribLast = new TickAttribLast
                    {
                        PastLimit = mask[0],
                        Unreported = mask[1]
                    };
                    var exchange = ReadString();
                    var specialConditions = ReadString();
                    eWrapper.tickByTickAllLast(reqId, tickType, time, price, size, tickAttribLast, exchange, specialConditions);
                    break;
                case 3: // BidAsk
                    var bidPrice = ReadDouble();
                    var askPrice = ReadDouble();
                    var bidSize = ReadDecimal();
                    var askSize = ReadDecimal();
                    mask = new BitMask(ReadInt());
                    var tickAttribBidAsk = new TickAttribBidAsk
                    {
                        BidPastLow = mask[0],
                        AskPastHigh = mask[1]
                    };
                    eWrapper.tickByTickBidAsk(reqId, time, bidPrice, askPrice, bidSize, askSize, tickAttribBidAsk);
                    break;
                case 4: // MidPoint
                    var midPoint = ReadDouble();
                    eWrapper.tickByTickMidPoint(reqId, time, midPoint);
                    break;
            }
        }

        private void TickByTickEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);

            protobuf.TickByTickData tickByTickDataProto = protobuf.TickByTickData.Parser.ParseFrom(byteArray);
            eWrapper.tickByTickDataProtoBuf(tickByTickDataProto);

            int reqId = tickByTickDataProto.HasReqId ? tickByTickDataProto.ReqId : IncomingMessage.NotValid;
            int tickType = tickByTickDataProto.HasTickType ? tickByTickDataProto.TickType : 0;

            switch (tickType)
            {
                case 0: // None
                    break;
                case 1: // Last
                case 2: // AllLast
                    if (tickByTickDataProto.HistoricalTickLast != null)
                    {
                        HistoricalTickLast historicalTickLast = EDecoderUtils.decodeHistoricalTickLast(tickByTickDataProto.HistoricalTickLast);
                        eWrapper.tickByTickAllLast(reqId, tickType, historicalTickLast.Time, historicalTickLast.Price,
                                                    historicalTickLast.Size, historicalTickLast.TickAttribLast,
                                                    historicalTickLast.Exchange, historicalTickLast.SpecialConditions);
                    }
                    break;

                case 3: // BidAsk
                    if (tickByTickDataProto.HistoricalTickBidAsk != null)
                    {
                        HistoricalTickBidAsk historicalTickBidAsk = EDecoderUtils.decodeHistoricalTickBidAsk(tickByTickDataProto.HistoricalTickBidAsk);
                        eWrapper.tickByTickBidAsk(reqId, historicalTickBidAsk.Time, historicalTickBidAsk.PriceBid,
                                                    historicalTickBidAsk.PriceAsk, historicalTickBidAsk.SizeBid,
                                                    historicalTickBidAsk.SizeAsk, historicalTickBidAsk.TickAttribBidAsk);
                    }
                    break;

                case 4: // MidPoint
                    if (tickByTickDataProto.HistoricalTickMidPoint != null)
                    {
                        HistoricalTick historicalTick = EDecoderUtils.decodeHistoricalTick(tickByTickDataProto.HistoricalTickMidPoint);
                        eWrapper.tickByTickMidPoint(reqId, historicalTick.Time, historicalTick.Price);
                    }
                    break;
            }
        }
        private void HistoricalTickLastEvent()
        {
            var reqId = ReadInt();
            var nTicks = ReadInt();
            var ticks = new HistoricalTickLast[nTicks];

            for (var i = 0; i < nTicks; i++)
            {
                var time = ReadLong();
                var mask = new BitMask(ReadInt());
                var tickAttribLast = new TickAttribLast
                {
                    PastLimit = mask[0],
                    Unreported = mask[1]
                };
                var price = ReadDouble();
                var size = ReadDecimal();
                var exchange = ReadString();
                var specialConditions = ReadString();

                ticks[i] = new HistoricalTickLast(time, tickAttribLast, price, size, exchange, specialConditions);
            }

            var done = ReadBoolFromInt();

            eWrapper.historicalTicksLast(reqId, ticks, done);
        }

        private void HistoricalTicksLastEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.HistoricalTicksLast historicalTicksLastProto = protobuf.HistoricalTicksLast.Parser.ParseFrom(byteArray);

            eWrapper.historicalTicksLastProtoBuf(historicalTicksLastProto);

            int reqId = historicalTicksLastProto.HasReqId ? historicalTicksLastProto.ReqId : IncomingMessage.NotValid;

            HistoricalTickLast[] historicalTicksLast = new HistoricalTickLast[historicalTicksLastProto.HistoricalTicksLast_.Count];
            for (int i = 0; i < historicalTicksLastProto.HistoricalTicksLast_.Count; i++)
            {
                historicalTicksLast[i] = EDecoderUtils.decodeHistoricalTickLast(historicalTicksLastProto.HistoricalTicksLast_[i]);
            }

            bool done = historicalTicksLastProto.HasIsDone ? historicalTicksLastProto.IsDone : false;

            eWrapper.historicalTicksLast(reqId, historicalTicksLast, done);
        }

        private void HistoricalTickBidAskEvent()
        {
            var reqId = ReadInt();
            var nTicks = ReadInt();
            var ticks = new HistoricalTickBidAsk[nTicks];

            for (var i = 0; i < nTicks; i++)
            {
                var time = ReadLong();
                var mask = new BitMask(ReadInt());
                var tickAttribBidAsk = new TickAttribBidAsk
                {
                    AskPastHigh = mask[0],
                    BidPastLow = mask[1]
                };
                var priceBid = ReadDouble();
                var priceAsk = ReadDouble();
                var sizeBid = ReadDecimal();
                var sizeAsk = ReadDecimal();

                ticks[i] = new HistoricalTickBidAsk(time, tickAttribBidAsk, priceBid, priceAsk, sizeBid, sizeAsk);
            }

            var done = ReadBoolFromInt();

            eWrapper.historicalTicksBidAsk(reqId, ticks, done);
        }

        private void HistoricalTicksBidAskEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.HistoricalTicksBidAsk historicalTicksBidAskProto = protobuf.HistoricalTicksBidAsk.Parser.ParseFrom(byteArray);

            eWrapper.historicalTicksBidAskProtoBuf(historicalTicksBidAskProto);

            int reqId = historicalTicksBidAskProto.HasReqId ? historicalTicksBidAskProto.ReqId : IncomingMessage.NotValid;

            HistoricalTickBidAsk[] historicalTicksBidAsk = new HistoricalTickBidAsk[historicalTicksBidAskProto.HistoricalTicksBidAsk_.Count];
            for (int i = 0; i < historicalTicksBidAskProto.HistoricalTicksBidAsk_.Count; i++)
            {
                historicalTicksBidAsk[i] = EDecoderUtils.decodeHistoricalTickBidAsk(historicalTicksBidAskProto.HistoricalTicksBidAsk_[i]);
            }

            bool isDone = historicalTicksBidAskProto.HasIsDone ? historicalTicksBidAskProto.IsDone : false;

            eWrapper.historicalTicksBidAsk(reqId, historicalTicksBidAsk, isDone);
        }

        private void HistoricalTickEvent()
        {
            var reqId = ReadInt();
            var nTicks = ReadInt();
            var ticks = new HistoricalTick[nTicks];

            for (var i = 0; i < nTicks; i++)
            {
                var time = ReadLong();
                ReadInt(); // for consistency
                var price = ReadDouble();
                var size = ReadDecimal();

                ticks[i] = new HistoricalTick(time, price, size);
            }

            var done = ReadBoolFromInt();

            eWrapper.historicalTicks(reqId, ticks, done);
        }

        private void HistoricalTicksEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.HistoricalTicks historicalTicksProto = protobuf.HistoricalTicks.Parser.ParseFrom(byteArray);

            eWrapper.historicalTicksProtoBuf(historicalTicksProto);

            int reqId = historicalTicksProto.HasReqId ? historicalTicksProto.ReqId : IncomingMessage.NotValid;

            HistoricalTick[] historicalTicks = new HistoricalTick[historicalTicksProto.HistoricalTicks_.Count];
            for (int i = 0; i < historicalTicksProto.HistoricalTicks_.Count; i++)
            {
                historicalTicks[i] = EDecoderUtils.decodeHistoricalTick(historicalTicksProto.HistoricalTicks_[i]);
            }

            bool isDone = historicalTicksProto.HasIsDone ? historicalTicksProto.IsDone : false;

            eWrapper.historicalTicks(reqId, historicalTicks, isDone);
        }

        private void MarketRuleEvent()
        {
            var marketRuleId = ReadInt();
            var priceIncrements = Array.Empty<PriceIncrement>();
            var nPriceIncrements = ReadInt();

            if (nPriceIncrements > 0)
            {
                Array.Resize(ref priceIncrements, nPriceIncrements);

                for (var i = 0; i < nPriceIncrements; ++i)
                {
                    priceIncrements[i] = new PriceIncrement(ReadDouble(), ReadDouble());
                }
            }

            eWrapper.marketRule(marketRuleId, priceIncrements);
        }

        private void MarketRuleEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.MarketRule marketRuleProto = protobuf.MarketRule.Parser.ParseFrom(byteArray);

            eWrapper.marketRuleProtoBuf(marketRuleProto);

            int marketRuleId = marketRuleProto.HasMarketRuleId ? marketRuleProto.MarketRuleId : 0;

            PriceIncrement[] priceIncrements = new PriceIncrement[0];
            if (marketRuleProto.PriceIncrements.Count > 0)
            {
                priceIncrements = new PriceIncrement[marketRuleProto.PriceIncrements.Count];
                for (int i = 0; i < marketRuleProto.PriceIncrements.Count; i++)
                {
                    priceIncrements[i] = EDecoderUtils.decodePriceIncrement(marketRuleProto.PriceIncrements[i]);
                }
            }

            eWrapper.marketRule(marketRuleId, priceIncrements);
        }

        private void RerouteMktDepthReqEvent()
        {
            var reqId = ReadInt();
            var conId = ReadInt();
            var exchange = ReadString();

            eWrapper.rerouteMktDepthReq(reqId, conId, exchange);
        }

        private void RerouteMktDepthReqEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.RerouteMarketDepthRequest rerouteMarketDepthRequestProto = protobuf.RerouteMarketDepthRequest.Parser.ParseFrom(byteArray);

            eWrapper.rerouteMarketDepthRequestProtoBuf(rerouteMarketDepthRequestProto);

            int reqId = rerouteMarketDepthRequestProto.HasReqId ? rerouteMarketDepthRequestProto.ReqId : IncomingMessage.NotValid;
            int conId = rerouteMarketDepthRequestProto.HasConId ? rerouteMarketDepthRequestProto.ConId : 0;
            string exchange = rerouteMarketDepthRequestProto.HasExchange ? rerouteMarketDepthRequestProto.Exchange : "";

            eWrapper.rerouteMktDepthReq(reqId, conId, exchange);
        }

        private void RerouteMktDataReqEvent()
        {
            var reqId = ReadInt();
            var conId = ReadInt();
            var exchange = ReadString();

            eWrapper.rerouteMktDataReq(reqId, conId, exchange);
        }

        private void RerouteMktDataReqEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.RerouteMarketDataRequest rerouteMarketDataRequestProto = protobuf.RerouteMarketDataRequest.Parser.ParseFrom(byteArray);

            eWrapper.rerouteMarketDataRequestProtoBuf(rerouteMarketDataRequestProto);

            int reqId = rerouteMarketDataRequestProto.HasReqId ? rerouteMarketDataRequestProto.ReqId : IncomingMessage.NotValid;
            int conId = rerouteMarketDataRequestProto.HasConId ? rerouteMarketDataRequestProto.ConId : 0;
            string exchange = rerouteMarketDataRequestProto.HasExchange ? rerouteMarketDataRequestProto.Exchange : "";

            eWrapper.rerouteMktDataReq(reqId, conId, exchange);
        }

        private void HistoricalDataUpdateEvent()
        {
            var requestId = ReadInt();
            var barCount = ReadInt();
            var date = ReadString();
            var open = ReadDouble();
            var close = ReadDouble();
            var high = ReadDouble();
            var low = ReadDouble();
            var WAP = ReadDecimal();
            var volume = ReadDecimal();

            eWrapper.historicalDataUpdate(requestId, new Bar(date, open, high, low, close, volume, barCount, WAP));
        }

        private void HistoricalDataUpdateEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.HistoricalDataUpdate historicalDataUpdateProto = protobuf.HistoricalDataUpdate.Parser.ParseFrom(byteArray);

            eWrapper.historicalDataUpdateProtoBuf(historicalDataUpdateProto);

            int reqId = historicalDataUpdateProto.HasReqId ? historicalDataUpdateProto.ReqId : IncomingMessage.NotValid;

            if (historicalDataUpdateProto.HistoricalDataBar != null)
            {
                Bar bar = EDecoderUtils.decodeHistoricalDataBar(historicalDataUpdateProto.HistoricalDataBar);
                eWrapper.historicalDataUpdate(reqId, bar);
            }
        }

        private void PnLSingleEvent()
        {
            var reqId = ReadInt();
            var pos = ReadDecimal();
            var dailyPnL = ReadDouble();
            var unrealizedPnL = double.MaxValue;
            var realizedPnL = double.MaxValue;

            if (serverVersion >= MinServerVer.UNREALIZED_PNL) unrealizedPnL = ReadDouble();

            if (serverVersion >= MinServerVer.REALIZED_PNL) realizedPnL = ReadDouble();

            var value = ReadDouble();

            eWrapper.pnlSingle(reqId, pos, dailyPnL, unrealizedPnL, realizedPnL, value);
        }

        private void PnLSingleEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);

            protobuf.PnLSingle pnlSingleProto = protobuf.PnLSingle.Parser.ParseFrom(byteArray);

            eWrapper.pnlSingleProtoBuf(pnlSingleProto);

            int reqId = pnlSingleProto.HasReqId ? pnlSingleProto.ReqId : IncomingMessage.NotValid;
            decimal pos = pnlSingleProto.HasPosition ? Util.StringToDecimal(pnlSingleProto.Position) : decimal.MaxValue;
            double dailyPnL = pnlSingleProto.HasDailyPnL ? pnlSingleProto.DailyPnL : double.MaxValue;
            double unrealizedPnL = pnlSingleProto.HasUnrealizedPnL ? pnlSingleProto.UnrealizedPnL : double.MaxValue;
            double realizedPnL = pnlSingleProto.HasRealizedPnL ? pnlSingleProto.RealizedPnL : double.MaxValue;
            double value = pnlSingleProto.HasValue ? pnlSingleProto.Value : double.MaxValue;

            eWrapper.pnlSingle(reqId, pos, dailyPnL, unrealizedPnL, realizedPnL, value);
        }

        private void PnLEvent()
        {
            var reqId = ReadInt();
            var dailyPnL = ReadDouble();
            var unrealizedPnL = double.MaxValue;
            var realizedPnL = double.MaxValue;

            if (serverVersion >= MinServerVer.UNREALIZED_PNL) unrealizedPnL = ReadDouble();

            if (serverVersion >= MinServerVer.REALIZED_PNL) realizedPnL = ReadDouble();

            eWrapper.pnl(reqId, dailyPnL, unrealizedPnL, realizedPnL);
        }

        private void PnLEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);

            protobuf.PnL pnlProto = protobuf.PnL.Parser.ParseFrom(byteArray);

            eWrapper.pnlProtoBuf(pnlProto);

            int reqId = pnlProto.HasReqId ? pnlProto.ReqId : IncomingMessage.NotValid;
            double dailyPnL = pnlProto.HasDailyPnL ? pnlProto.DailyPnL : double.MaxValue;
            double unrealizedPnL = pnlProto.HasUnrealizedPnL ? pnlProto.UnrealizedPnL : double.MaxValue;
            double realizedPnL = pnlProto.HasRealizedPnL ? pnlProto.RealizedPnL : double.MaxValue;

            eWrapper.pnl(reqId, dailyPnL, unrealizedPnL, realizedPnL);
        }

        private void HistogramDataEvent()
        {
            var reqId = ReadInt();
            var n = ReadInt();
            var data = new HistogramEntry[n];

            for (var i = 0; i < n; i++)
            {
                data[i].Price = ReadDouble();
                data[i].Size = ReadDecimal();
            }

            eWrapper.histogramData(reqId, data);
        }

        private void HistogramDataEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.HistogramData histogramDataProto = protobuf.HistogramData.Parser.ParseFrom(byteArray);

            eWrapper.histogramDataProtoBuf(histogramDataProto);

            int reqId = histogramDataProto.HasReqId ? histogramDataProto.ReqId : IncomingMessage.NotValid;

            HistogramEntry[] entries = new HistogramEntry[histogramDataProto.HistogramDataEntries.Count];
            for (int i = 0; i < histogramDataProto.HistogramDataEntries.Count; i++)
            {
                entries[i] = EDecoderUtils.decodeHistogramDataEntry(histogramDataProto.HistogramDataEntries[i]);
            }

            eWrapper.histogramData(reqId, entries);
        }

        private void HeadTimestampEvent()
        {
            var reqId = ReadInt();
            var headTimestamp = ReadString();

            eWrapper.headTimestamp(reqId, headTimestamp);
        }

        private void HeadTimestampEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.HeadTimestamp headTimestampProto = protobuf.HeadTimestamp.Parser.ParseFrom(byteArray);

            eWrapper.headTimestampProtoBuf(headTimestampProto);

            int reqId = headTimestampProto.HasReqId ? headTimestampProto.ReqId : IncomingMessage.NotValid;
            string headTimestamp = headTimestampProto.HasHeadTimestamp_ ? headTimestampProto.HeadTimestamp_ : "";

            eWrapper.headTimestamp(reqId, headTimestamp);
        }

        private void HistoricalNewsEvent()
        {
            var requestId = ReadInt();
            var time = ReadString();
            var providerCode = ReadString();
            var articleId = ReadString();
            var headline = ReadString();

            eWrapper.historicalNews(requestId, time, providerCode, articleId, headline);
        }

        private void HistoricalNewsEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.HistoricalNews historicalNewsProto = protobuf.HistoricalNews.Parser.ParseFrom(byteArray);

            eWrapper.historicalNewsProtoBuf(historicalNewsProto);

            int reqId = historicalNewsProto.HasReqId ? historicalNewsProto.ReqId : IncomingMessage.NotValid;
            string time = historicalNewsProto.HasTime ? historicalNewsProto.Time : "";
            string providerCode = historicalNewsProto.HasProviderCode ? historicalNewsProto.ProviderCode : "";
            string articleId = historicalNewsProto.HasArticleId ? historicalNewsProto.ArticleId : "";
            string headline = historicalNewsProto.HasHeadline ? historicalNewsProto.Headline : "";

            eWrapper.historicalNews(reqId, time, providerCode, articleId, headline);
        }

        private void HistoricalNewsEndEvent()
        {
            var requestId = ReadInt();
            var hasMore = ReadBoolFromInt();

            eWrapper.historicalNewsEnd(requestId, hasMore);
        }

        private void HistoricalNewsEndEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.HistoricalNewsEnd historicalNewsEndProto = protobuf.HistoricalNewsEnd.Parser.ParseFrom(byteArray);

            eWrapper.historicalNewsEndProtoBuf(historicalNewsEndProto);

            int reqId = historicalNewsEndProto.HasReqId ? historicalNewsEndProto.ReqId : IncomingMessage.NotValid;
            bool hasMore = historicalNewsEndProto.HasHasMore ? historicalNewsEndProto.HasMore : false;

            eWrapper.historicalNewsEnd(reqId, hasMore);
        }

        private void NewsArticleEvent()
        {
            var requestId = ReadInt();
            var articleType = ReadInt();
            var articleText = ReadString();

            eWrapper.newsArticle(requestId, articleType, articleText);
        }

        private void NewsArticleEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.NewsArticle newsArticleProto = protobuf.NewsArticle.Parser.ParseFrom(byteArray);

            eWrapper.newsArticleProtoBuf(newsArticleProto);

            int reqId = newsArticleProto.HasReqId ? newsArticleProto.ReqId : IncomingMessage.NotValid;
            int articleType = newsArticleProto.HasArticleType ? newsArticleProto.ArticleType : 0;
            string articleText = newsArticleProto.HasArticleText ? newsArticleProto.ArticleText : "";

            eWrapper.newsArticle(reqId, articleType, articleText);
        }

        private void NewsProvidersEvent()
        {
            var newsProviders = Array.Empty<NewsProvider>();
            var nNewsProviders = ReadInt();

            if (nNewsProviders > 0)
            {
                Array.Resize(ref newsProviders, nNewsProviders);

                for (var i = 0; i < nNewsProviders; ++i)
                {
                    newsProviders[i] = new NewsProvider(ReadString(), ReadString());
                }
            }

            eWrapper.newsProviders(newsProviders);
        }

        private void NewsProvidersEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.NewsProviders newsProvidersProto = protobuf.NewsProviders.Parser.ParseFrom(byteArray);

            eWrapper.newsProvidersProtoBuf(newsProvidersProto);

            NewsProvider[] newsProviders = new NewsProvider[0];

            if (newsProvidersProto.NewsProviders_.Count > 0)
            {
                newsProviders = new NewsProvider[newsProvidersProto.NewsProviders_.Count];

                for (int i = 0; i < newsProvidersProto.NewsProviders_.Count; i++)
                {
                    protobuf.NewsProvider newsProviderProto = newsProvidersProto.NewsProviders_[i];
                    newsProviders[i] = new NewsProvider(newsProviderProto.HasProviderCode ? newsProviderProto.ProviderCode : "", newsProviderProto.HasProviderName ? newsProviderProto.ProviderName : "");
                }
            }

            eWrapper.newsProviders(newsProviders);
        }

        private void SmartComponentsEvent()
        {
            var reqId = ReadInt();
            var n = ReadInt();
            var theMap = new Dictionary<int, KeyValuePair<string, char>>();

            for (var i = 0; i < n; i++)
            {
                var bitNumber = ReadInt();
                var exchange = ReadString();
                var exchangeLetter = ReadChar();

                theMap.Add(bitNumber, new KeyValuePair<string, char>(exchange, exchangeLetter));
            }

            eWrapper.smartComponents(reqId, theMap);
        }

        private void SmartComponentsEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.SmartComponents smartComponentsProto = protobuf.SmartComponents.Parser.ParseFrom(byteArray);

            eWrapper.smartComponentsProtoBuf(smartComponentsProto);

            int reqId = smartComponentsProto.HasReqId ? smartComponentsProto.ReqId : IncomingMessage.NotValid;
            Dictionary<int, KeyValuePair<string, char>> theMap = EDecoderUtils.decodeSmartComponents(smartComponentsProto);

            eWrapper.smartComponents(reqId, theMap);
        }

        private void TickReqParamsEvent()
        {
            var tickerId = ReadInt();
            var minTick = ReadDouble();
            var bboExchange = ReadString();
            var snapshotPermissions = ReadInt();

            eWrapper.tickReqParams(tickerId, minTick, bboExchange, snapshotPermissions);
        }

        private void TickReqParamsEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.TickReqParams tickReqParamsProto = protobuf.TickReqParams.Parser.ParseFrom(byteArray);

            eWrapper.tickReqParamsProtoBuf(tickReqParamsProto);

            int reqId = tickReqParamsProto.HasReqId ? tickReqParamsProto.ReqId : IncomingMessage.NotValid;
            double minTick = tickReqParamsProto.HasMinTick ? Util.StringToDoubleMax(tickReqParamsProto.MinTick) : double.MaxValue;
            string bboExchange = tickReqParamsProto.HasBboExchange ? tickReqParamsProto.BboExchange : "";
            int snapshotPermissions = tickReqParamsProto.HasSnapshotPermissions ? tickReqParamsProto.SnapshotPermissions : int.MaxValue;

            eWrapper.tickReqParams(reqId, minTick, bboExchange, snapshotPermissions);
        }

        private void TickNewsEvent()
        {
            var tickerId = ReadInt();
            var timeStamp = ReadLong();
            var providerCode = ReadString();
            var articleId = ReadString();
            var headline = ReadString();
            var extraData = ReadString();

            eWrapper.tickNews(tickerId, timeStamp, providerCode, articleId, headline, extraData);
        }

        private void TickNewsEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.TickNews tickNewsProto = protobuf.TickNews.Parser.ParseFrom(byteArray);

            eWrapper.tickNewsProtoBuf(tickNewsProto);

            int reqId = tickNewsProto.HasReqId ? tickNewsProto.ReqId : IncomingMessage.NotValid;
            long timestamp = tickNewsProto.HasTimestamp ? tickNewsProto.Timestamp : 0;
            string providerCode = tickNewsProto.HasProviderCode ? tickNewsProto.ProviderCode : "";
            string articleId = tickNewsProto.HasArticleId ? tickNewsProto.ArticleId : "";
            string headline = tickNewsProto.HasHeadline ? tickNewsProto.Headline : "";
            string extraData = tickNewsProto.HasExtraData ? tickNewsProto.ExtraData : "";

            eWrapper.tickNews(reqId, timestamp, providerCode, articleId, headline, extraData);
        }

        private void SymbolSamplesEvent()
        {
            var reqId = ReadInt();
            var contractDescriptions = Array.Empty<ContractDescription>();
            var nContractDescriptions = ReadInt();

            if (nContractDescriptions > 0)
            {
                Array.Resize(ref contractDescriptions, nContractDescriptions);

                for (var i = 0; i < nContractDescriptions; ++i)
                {
                    // read contract fields
                    var contract = new Contract
                    {
                        ConId = ReadInt(),
                        Symbol = ReadString(),
                        SecType = ReadString(),
                        PrimaryExch = ReadString(),
                        Currency = ReadString()
                    };

                    // read derivative sec types list
                    var derivativeSecTypes = Array.Empty<string>();
                    var nDerivativeSecTypes = ReadInt();
                    if (nDerivativeSecTypes > 0)
                    {
                        Array.Resize(ref derivativeSecTypes, nDerivativeSecTypes);
                        for (var j = 0; j < nDerivativeSecTypes; ++j)
                        {
                            derivativeSecTypes[j] = ReadString();
                        }
                    }
                    if (serverVersion >= MinServerVer.MIN_SERVER_VER_BOND_ISSUERID)
                    {
                        contract.Description = ReadString();
                        contract.IssuerId = ReadString();
                    }

                    var contractDescription = new ContractDescription(contract, derivativeSecTypes);
                    contractDescriptions[i] = contractDescription;
                }
            }

            eWrapper.symbolSamples(reqId, contractDescriptions);
        }

        private void SymbolSamplesEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.SymbolSamples symbolSamplesProto = protobuf.SymbolSamples.Parser.ParseFrom(byteArray);

            eWrapper.symbolSamplesProtoBuf(symbolSamplesProto);

            int reqId = symbolSamplesProto.HasReqId ? symbolSamplesProto.ReqId : IncomingMessage.NotValid;

            ContractDescription[] contractDescriptions = new ContractDescription[0];
            if (symbolSamplesProto.ContractDescriptions.Count > 0)
            {
                contractDescriptions = new ContractDescription[symbolSamplesProto.ContractDescriptions.Count];
                for (int i = 0; i < symbolSamplesProto.ContractDescriptions.Count; i++)
                {
                    protobuf.ContractDescription contractDescriptionProto = symbolSamplesProto.ContractDescriptions[i];
                    if (contractDescriptionProto.Contract is null)
                    {
                        continue;
                    }
                    Contract contract = EDecoderUtils.decodeContract(contractDescriptionProto.Contract);

                    string[] derivativeSecTypes = new string[0];
                    if (contractDescriptionProto.DerivativeSecTypes.Count > 0)
                    {
                        derivativeSecTypes = new string[contractDescriptionProto.DerivativeSecTypes.Count];
                        for (int j = 0; j < contractDescriptionProto.DerivativeSecTypes.Count; j++)
                        {
                            derivativeSecTypes[j] = contractDescriptionProto.DerivativeSecTypes[j];
                        }
                    }

                    contractDescriptions[i] = new ContractDescription(contract, derivativeSecTypes);
                }
            }

            eWrapper.symbolSamples(reqId, contractDescriptions);
        }

        private void FamilyCodesEvent()
        {
            var familyCodes = Array.Empty<FamilyCode>();
            var nFamilyCodes = ReadInt();

            if (nFamilyCodes > 0)
            {
                Array.Resize(ref familyCodes, nFamilyCodes);

                for (var i = 0; i < nFamilyCodes; ++i)
                {
                    familyCodes[i] = new FamilyCode(ReadString(), ReadString());
                }
            }

            eWrapper.familyCodes(familyCodes);
        }
        private void FamilyCodesEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.FamilyCodes familyCodesProto = protobuf.FamilyCodes.Parser.ParseFrom(byteArray);

            eWrapper.familyCodesProtoBuf(familyCodesProto);

            FamilyCode[] familyCodes = new FamilyCode[0];
            if (familyCodesProto.FamilyCodes_.Count > 0)
            {
                familyCodes = new FamilyCode[familyCodesProto.FamilyCodes_.Count];
                for (int i = 0; i < familyCodesProto.FamilyCodes_.Count; i++)
                {
                    familyCodes[i] = EDecoderUtils.decodeFamilyCode(familyCodesProto.FamilyCodes_[i]);
                }
            }

            eWrapper.familyCodes(familyCodes);
        }

        private void MktDepthExchangesEvent()
        {
            var depthMktDataDescriptions = Array.Empty<DepthMktDataDescription>();
            var nDescriptions = ReadInt();

            if (nDescriptions > 0)
            {
                Array.Resize(ref depthMktDataDescriptions, nDescriptions);

                for (var i = 0; i < nDescriptions; i++)
                {
                    if (serverVersion >= MinServerVer.SERVICE_DATA_TYPE)
                    {
                        depthMktDataDescriptions[i] = new DepthMktDataDescription(ReadString(), ReadString(), ReadString(), ReadString(), ReadIntMax());
                    }
                    else
                    {
                        depthMktDataDescriptions[i] = new DepthMktDataDescription(ReadString(), ReadString(), "", ReadBoolFromInt() ? "Deep2" : "Deep", int.MaxValue);
                    }
                }
            }

            eWrapper.mktDepthExchanges(depthMktDataDescriptions);
        }

        private void MarketDepthExchangesEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.MarketDepthExchanges marketDepthExchangesProto = protobuf.MarketDepthExchanges.Parser.ParseFrom(byteArray);

            eWrapper.marketDepthExchangesProtoBuf(marketDepthExchangesProto);

            DepthMktDataDescription[] depthMktDataDescriptions = new DepthMktDataDescription[0];
            if (marketDepthExchangesProto.DepthMarketDataDescriptions.Count > 0)
            {
                depthMktDataDescriptions = new DepthMktDataDescription[marketDepthExchangesProto.DepthMarketDataDescriptions.Count];
                for (int i = 0; i < marketDepthExchangesProto.DepthMarketDataDescriptions.Count; i++)
                {
                    depthMktDataDescriptions[i] = EDecoderUtils.decodeDepthMarketDataDescription(marketDepthExchangesProto.DepthMarketDataDescriptions[i]);
                }
            }

            eWrapper.mktDepthExchanges(depthMktDataDescriptions);
        }

        private void SoftDollarTierEvent()
        {
            var reqId = ReadInt();
            var nTiers = ReadInt();
            var tiers = new SoftDollarTier[nTiers];

            for (var i = 0; i < nTiers; i++)
            {
                tiers[i] = new SoftDollarTier(ReadString(), ReadString(), ReadString());
            }

            eWrapper.softDollarTiers(reqId, tiers);
        }

        private void SoftDollarTiersEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.SoftDollarTiers softDollarTiersProto = protobuf.SoftDollarTiers.Parser.ParseFrom(byteArray);

            eWrapper.softDollarTiersProtoBuf(softDollarTiersProto);

            int reqId = softDollarTiersProto.HasReqId ? softDollarTiersProto.ReqId : IncomingMessage.NotValid;

            SoftDollarTier[] tiers = new SoftDollarTier[0];
            if (softDollarTiersProto.SoftDollarTiers_.Count > 0)
            {
                tiers = new SoftDollarTier[softDollarTiersProto.SoftDollarTiers_.Count];
                for (int i = 0; i < softDollarTiersProto.SoftDollarTiers_.Count; i++)
                {
                    SoftDollarTier tier = EDecoderUtils.decodeSoftDollarTier(softDollarTiersProto.SoftDollarTiers_[i]);
                    tiers[i] = tier != null ? tier : new SoftDollarTier("", "", "");
                }
            }

            eWrapper.softDollarTiers(reqId, tiers);
        }

        private void SecurityDefinitionOptionParameterEndEvent()
        {
            var reqId = ReadInt();

            eWrapper.securityDefinitionOptionParameterEnd(reqId);
        }

        private void SecurityDefinitionOptionParameterEndEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.SecDefOptParameterEnd secDefOptParameterEndProto = protobuf.SecDefOptParameterEnd.Parser.ParseFrom(byteArray);

            eWrapper.secDefOptParameterEndProtoBuf(secDefOptParameterEndProto);

            int reqId = secDefOptParameterEndProto.HasReqId ? secDefOptParameterEndProto.ReqId : IncomingMessage.NotValid;

            eWrapper.securityDefinitionOptionParameterEnd(reqId);
        }

        private void SecurityDefinitionOptionParameterEvent()
        {
            var reqId = ReadInt();
            var exchange = ReadString();
            var underlyingConId = ReadInt();
            var tradingClass = ReadString();
            var multiplier = ReadString();
            var expirationsSize = ReadInt();
            var expirations = new HashSet<string>();
            var strikes = new HashSet<double>();

            for (var i = 0; i < expirationsSize; i++)
            {
                expirations.Add(ReadString());
            }

            var strikesSize = ReadInt();

            for (var i = 0; i < strikesSize; i++)
            {
                strikes.Add(ReadDouble());
            }

            eWrapper.securityDefinitionOptionParameter(reqId, exchange, underlyingConId, tradingClass, multiplier, expirations, strikes);
        }

        private void SecurityDefinitionOptionParameterEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.SecDefOptParameter secDefOptParameterProto = protobuf.SecDefOptParameter.Parser.ParseFrom(byteArray);

            eWrapper.secDefOptParameterProtoBuf(secDefOptParameterProto);

            int reqId = secDefOptParameterProto.HasReqId ? secDefOptParameterProto.ReqId : IncomingMessage.NotValid;
            string exchange = secDefOptParameterProto.HasExchange ? secDefOptParameterProto.Exchange : "";
            int underlyingConId = secDefOptParameterProto.HasUnderlyingConId ? secDefOptParameterProto.UnderlyingConId : 0;
            string tradingClass = secDefOptParameterProto.HasTradingClass ? secDefOptParameterProto.TradingClass : "";
            string multiplier = secDefOptParameterProto.HasMultiplier ? secDefOptParameterProto.Multiplier : "";

            HashSet<string> expirations = new HashSet<string>();
            HashSet<double> strikes = new HashSet<double>();

            if (secDefOptParameterProto.Expirations.Count > 0)
            {
                foreach (string expiration in secDefOptParameterProto.Expirations)
                {
                    expirations.Add(expiration);
                }
            }

            if (secDefOptParameterProto.Strikes.Count > 0)
            {
                foreach (double strike in secDefOptParameterProto.Strikes)
                {
                    strikes.Add(strike);
                }
            }

            eWrapper.securityDefinitionOptionParameter(reqId, exchange, underlyingConId, tradingClass, multiplier, expirations, strikes);
        }

        private void DisplayGroupUpdatedEvent()
        {
            _ = ReadInt(); //msgVersion
            var reqId = ReadInt();
            var contractInfo = ReadString();

            eWrapper.displayGroupUpdated(reqId, contractInfo);
        }

        private void DisplayGroupUpdatedEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.DisplayGroupUpdated displayGroupUpdatedProto = protobuf.DisplayGroupUpdated.Parser.ParseFrom(byteArray);

            eWrapper.displayGroupUpdatedProtoBuf(displayGroupUpdatedProto);

            int reqId = displayGroupUpdatedProto.HasReqId ? displayGroupUpdatedProto.ReqId : IncomingMessage.NotValid;
            string contractInfo = displayGroupUpdatedProto.HasContractInfo ? displayGroupUpdatedProto.ContractInfo : "";

            eWrapper.displayGroupUpdated(reqId, contractInfo);
        }

        private void DisplayGroupListEvent()
        {
            _ = ReadInt(); //msgVersion
            var reqId = ReadInt();
            var groups = ReadString();

            eWrapper.displayGroupList(reqId, groups);
        }

        private void DisplayGroupListEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.DisplayGroupList displayGroupListProto = protobuf.DisplayGroupList.Parser.ParseFrom(byteArray);

            eWrapper.displayGroupListProtoBuf(displayGroupListProto);

            int reqId = displayGroupListProto.HasReqId ? displayGroupListProto.ReqId : IncomingMessage.NotValid;
            string groups = displayGroupListProto.HasGroups ? displayGroupListProto.Groups : "";

            eWrapper.displayGroupList(reqId, groups);
        }

        private void VerifyCompletedEvent()
        {
            _ = ReadInt(); //msgVersion
            var isSuccessful = string.Equals(ReadString(), "true", StringComparison.OrdinalIgnoreCase);
            var errorText = ReadString();

            eWrapper.verifyCompleted(isSuccessful, errorText);
        }

        private void VerifyCompletedEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.VerifyCompleted verifyCompletedProto = protobuf.VerifyCompleted.Parser.ParseFrom(byteArray);

            eWrapper.verifyCompletedProtoBuf(verifyCompletedProto);

            bool isSuccessful = verifyCompletedProto.HasIsSuccessful ? verifyCompletedProto.IsSuccessful : false;
            string errorText = verifyCompletedProto.HasErrorText ? verifyCompletedProto.ErrorText : "";

            eWrapper.verifyCompleted(isSuccessful, errorText);
        }

        private void VerifyMessageApiEvent()
        {
            _ = ReadInt(); //msgVersion
            var apiData = ReadString();

            eWrapper.verifyMessageAPI(apiData);
        }

        private void VerifyMessageApiEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.VerifyMessageApi verifyMessageApiProto = protobuf.VerifyMessageApi.Parser.ParseFrom(byteArray);

            eWrapper.verifyMessageApiProtoBuf(verifyMessageApiProto);

            string apiData = verifyMessageApiProto.HasApiData ? verifyMessageApiProto.ApiData : "";

            eWrapper.verifyMessageAPI(apiData);
        }

        private void VerifyAndAuthCompletedEvent()
        {
            _ = ReadInt(); //msgVersion
            var isSuccessful = string.Equals(ReadString(), "true", StringComparison.OrdinalIgnoreCase);
            var errorText = ReadString();

            eWrapper.verifyAndAuthCompleted(isSuccessful, errorText);
        }

        private void VerifyAndAuthMessageApiEvent()
        {
            _ = ReadInt(); //msgVersion
            var apiData = ReadString();
            var xyzChallenge = ReadString();

            eWrapper.verifyAndAuthMessageAPI(apiData, xyzChallenge);
        }

        private void TickPriceEvent()
        {
            var msgVersion = ReadInt();
            var requestId = ReadInt();
            var tickType = ReadInt();
            var price = ReadDouble();
            decimal size = 0;

            if (msgVersion >= 2) size = ReadDecimal();

            var attr = new TickAttrib();

            if (msgVersion >= 3)
            {
                var attrMask = ReadInt();

                attr.CanAutoExecute = attrMask == 1;

                if (serverVersion >= MinServerVer.PAST_LIMIT)
                {
                    var mask = new BitMask(attrMask);

                    attr.CanAutoExecute = mask[0];
                    attr.PastLimit = mask[1];

                    if (serverVersion >= MinServerVer.PRE_OPEN_BID_ASK) attr.PreOpen = mask[2];
                }
            }


            eWrapper.tickPrice(requestId, tickType, price, attr);

            if (msgVersion < 2) return;
            var sizeTickType = -1; //not a tick
            switch (tickType)
            {
                case TickType.BID:
                    sizeTickType = TickType.BID_SIZE;
                    break;
                case TickType.ASK:
                    sizeTickType = TickType.ASK_SIZE;
                    break;
                case TickType.LAST:
                    sizeTickType = TickType.LAST_SIZE;
                    break;
                case TickType.DELAYED_BID:
                    sizeTickType = TickType.DELAYED_BID_SIZE;
                    break;
                case TickType.DELAYED_ASK:
                    sizeTickType = TickType.DELAYED_ASK_SIZE;
                    break;
                case TickType.DELAYED_LAST:
                    sizeTickType = TickType.DELAYED_LAST_SIZE;
                    break;
            }
            if (sizeTickType != -1) eWrapper.tickSize(requestId, sizeTickType, size);
        }

        private void TickPriceEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.TickPrice tickPriceProto = protobuf.TickPrice.Parser.ParseFrom(byteArray);

            eWrapper.tickPriceProtoBuf(tickPriceProto);

            int reqId = tickPriceProto.HasReqId ? tickPriceProto.ReqId : IncomingMessage.NotValid;
            int tickType = tickPriceProto.HasTickType ? tickPriceProto.TickType : 0;
            double price = tickPriceProto.HasPrice ? tickPriceProto.Price : 0;
            decimal size = tickPriceProto.HasSize ? Util.StringToDecimal(tickPriceProto.Size) : decimal.MaxValue;
            var attribs = new TickAttrib();
            int attrMask = tickPriceProto.HasAttrMask ? tickPriceProto.AttrMask : 0;
            var mask = new BitMask(attrMask);
            attribs.CanAutoExecute = mask[0];
            attribs.PastLimit = mask[1];
            attribs.PreOpen = mask[2];

            eWrapper.tickPrice(reqId, tickType, price, attribs);

            var sizeTickType = -1; // not a tick
            switch (tickType)
            {
                case TickType.BID:
                    sizeTickType = TickType.BID_SIZE;
                    break;
                case TickType.ASK:
                    sizeTickType = TickType.ASK_SIZE;
                    break;
                case TickType.LAST:
                    sizeTickType = TickType.LAST_SIZE;
                    break;
                case TickType.DELAYED_BID:
                    sizeTickType = TickType.DELAYED_BID_SIZE;
                    break;
                case TickType.DELAYED_ASK:
                    sizeTickType = TickType.DELAYED_ASK_SIZE;
                    break;
                case TickType.DELAYED_LAST:
                    sizeTickType = TickType.DELAYED_LAST_SIZE;
                    break;
            }
            if (sizeTickType != -1)
            {
                eWrapper.tickSize(reqId, sizeTickType, size);
            }
        }

        private void TickSizeEvent()
        {
            _ = ReadInt(); //msgVersion
            var requestId = ReadInt();
            var tickType = ReadInt();
            var size = ReadDecimal();
            eWrapper.tickSize(requestId, tickType, size);
        }

        private void TickSizeEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.TickSize tickSizeProto = protobuf.TickSize.Parser.ParseFrom(byteArray);

            eWrapper.tickSizeProtoBuf(tickSizeProto);

            int reqId = tickSizeProto.HasReqId ? tickSizeProto.ReqId : IncomingMessage.NotValid;
            int tickType = tickSizeProto.HasTickType ? tickSizeProto.TickType : 0;
            decimal size = tickSizeProto.HasSize ? Util.StringToDecimal(tickSizeProto.Size) : decimal.MaxValue;

            eWrapper.tickSize(reqId, tickType, size);
        }

        private void TickStringEvent()
        {
            _ = ReadInt(); //msgVersion
            var requestId = ReadInt();
            var tickType = ReadInt();
            var value = ReadString();
            eWrapper.tickString(requestId, tickType, value);
        }

        private void TickStringEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.TickString tickStringProto = protobuf.TickString.Parser.ParseFrom(byteArray);

            eWrapper.tickStringProtoBuf(tickStringProto);

            int reqId = tickStringProto.HasReqId ? tickStringProto.ReqId : IncomingMessage.NotValid;
            int tickType = tickStringProto.HasTickType ? tickStringProto.TickType : 0;
            string value = tickStringProto.HasValue ? tickStringProto.Value : "";

            eWrapper.tickString(reqId, tickType, value);
        }

        private void TickGenericEvent()
        {
            _ = ReadInt(); //msgVersion
            var requestId = ReadInt();
            var tickType = ReadInt();
            var value = ReadDouble();
            eWrapper.tickGeneric(requestId, tickType, value);
        }

        private void TickGenericEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.TickGeneric tickGenericProto = protobuf.TickGeneric.Parser.ParseFrom(byteArray);

            eWrapper.tickGenericProtoBuf(tickGenericProto);

            int reqId = tickGenericProto.HasReqId ? tickGenericProto.ReqId : IncomingMessage.NotValid;
            int tickType = tickGenericProto.HasTickType ? tickGenericProto.TickType : 0;
            double value = tickGenericProto.HasValue ? tickGenericProto.Value : 0;

            eWrapper.tickGeneric(reqId, tickType, value);
        }

        private void TickEFPEvent()
        {
            _ = ReadInt(); //msgVersion
            var requestId = ReadInt();
            var tickType = ReadInt();
            var basisPoints = ReadDouble();
            var formattedBasisPoints = ReadString();
            var impliedFuturesPrice = ReadDouble();
            var holdDays = ReadInt();
            var futureLastTradeDate = ReadString();
            var dividendImpact = ReadDouble();
            var dividendsToLastTradeDate = ReadDouble();
            eWrapper.tickEFP(requestId, tickType, basisPoints, formattedBasisPoints, impliedFuturesPrice, holdDays, futureLastTradeDate, dividendImpact, dividendsToLastTradeDate);
        }

        private void TickSnapshotEndEvent()
        {
            _ = ReadInt(); //msgVersion
            var requestId = ReadInt();
            eWrapper.tickSnapshotEnd(requestId);
        }

        private void TickSnapshotEndEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.TickSnapshotEnd tickSnapshotEndProto = protobuf.TickSnapshotEnd.Parser.ParseFrom(byteArray);

            eWrapper.tickSnapshotEndProtoBuf(tickSnapshotEndProto);

            int reqId = tickSnapshotEndProto.HasReqId ? tickSnapshotEndProto.ReqId : IncomingMessage.NotValid;

            eWrapper.tickSnapshotEnd(reqId);
        }

        private void ErrorEvent()
        {
            if (serverVersion < MinServerVer.MIN_SERVER_VER_ERROR_TIME) 
            {
                _ = ReadInt(); //msgVersion
            }
            var id = ReadInt();
            var errorCode = ReadInt();
            var errorMsg = serverVersion >= MinServerVer.ENCODE_MSG_ASCII7 ? Regex.Unescape(ReadString()) : ReadString();
            var advancedOrderRejectJson = "";
            if (serverVersion >= MinServerVer.ADVANCED_ORDER_REJECT)
            {
                var tempStr = ReadString();
                if (!Util.StringIsEmpty(tempStr)) advancedOrderRejectJson = Regex.Unescape(tempStr);
            }
            var errorTime = 0L;
            if (serverVersion >= MinServerVer.MIN_SERVER_VER_ERROR_TIME)
            {
                errorTime = ReadLong();
            }

            eWrapper.error(id, errorTime, errorCode, errorMsg, advancedOrderRejectJson);
        }

        private void ErrorEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.ErrorMessage errorMessageProto = protobuf.ErrorMessage.Parser.ParseFrom(byteArray);

            eWrapper.errorProtoBuf(errorMessageProto);

            int id = errorMessageProto.HasId ? errorMessageProto.Id : 0;
            int errorCode = errorMessageProto.HasErrorCode ? errorMessageProto.ErrorCode : 0;
            String errorMsg = errorMessageProto.HasErrorMsg ? errorMessageProto.ErrorMsg : "";
            String advancedOrderRejectJson = errorMessageProto.HasAdvancedOrderRejectJson ? errorMessageProto.AdvancedOrderRejectJson : "";
            long errorTime = errorMessageProto.HasErrorTime ? errorMessageProto.ErrorTime : 0;

            eWrapper.error(id, errorTime, errorCode, errorMsg, advancedOrderRejectJson);
        }
        private void CurrentTimeEvent()
        {
            _ = ReadInt(); //msgVersion
            var time = ReadLong();
            eWrapper.currentTime(time);
        }
        private void CurrentTimeEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.CurrentTime currentTimeProto = protobuf.CurrentTime.Parser.ParseFrom(byteArray);

            eWrapper.currentTimeProtoBuf(currentTimeProto);

            long time = currentTimeProto.HasCurrentTime_ ? currentTimeProto.CurrentTime_ : 0;

            eWrapper.currentTime(time);
        }

        private void ManagedAccountsEvent()
        {
            _ = ReadInt(); //msgVersion
            var accountsList = ReadString();
            eWrapper.managedAccounts(accountsList);
        }

        private void ManagedAccountsEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.ManagedAccounts managedAccountsProto = protobuf.ManagedAccounts.Parser.ParseFrom(byteArray);

            eWrapper.managedAccountsProtoBuf(managedAccountsProto);

            string accountsList = managedAccountsProto.HasAccountsList ? managedAccountsProto.AccountsList : "";

            eWrapper.managedAccounts(accountsList);
        }

        private void NextValidIdEvent()
        {
            _ = ReadInt(); //msgVersion
            var orderId = ReadInt();
            eWrapper.nextValidId(orderId);
        }

        private void NextValidIdEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.NextValidId nextValidIdProto = protobuf.NextValidId.Parser.ParseFrom(byteArray);

            eWrapper.nextValidIdProtoBuf(nextValidIdProto);

            int orderId = nextValidIdProto.HasOrderId ? nextValidIdProto.OrderId : 0;

            eWrapper.nextValidId(orderId);
        }

        private void DeltaNeutralValidationEvent()
        {
            _ = ReadInt(); //msgVersion
            var requestId = ReadInt();
            var deltaNeutralContract = new DeltaNeutralContract
            {
                ConId = ReadInt(),
                Delta = ReadDouble(),
                Price = ReadDouble()
            };
            eWrapper.deltaNeutralValidation(requestId, deltaNeutralContract);
        }

        private void TickOptionComputationEvent()
        {
            var msgVersion = serverVersion >= MinServerVer.PRICE_BASED_VOLATILITY ? int.MaxValue : ReadInt();

            var requestId = ReadInt();
            var tickType = ReadInt();
            var tickAttrib = int.MaxValue;
            if (serverVersion >= MinServerVer.PRICE_BASED_VOLATILITY)
            {
                tickAttrib = ReadInt();
            }
            var impliedVolatility = ReadDouble();
            if (impliedVolatility.Equals(-1))
            { // -1 is the "not yet computed" indicator
                impliedVolatility = double.MaxValue;
            }
            var delta = ReadDouble();
            if (delta.Equals(-2))
            { // -2 is the "not yet computed" indicator
                delta = double.MaxValue;
            }
            var optPrice = double.MaxValue;
            var pvDividend = double.MaxValue;
            var gamma = double.MaxValue;
            var vega = double.MaxValue;
            var theta = double.MaxValue;
            var undPrice = double.MaxValue;
            if (msgVersion >= 6 || tickType == TickType.MODEL_OPTION || tickType == TickType.DELAYED_MODEL_OPTION)
            {
                optPrice = ReadDouble();
                if (optPrice.Equals(-1))
                { // -1 is the "not yet computed" indicator
                    optPrice = double.MaxValue;
                }
                pvDividend = ReadDouble();
                if (pvDividend.Equals(-1))
                { // -1 is the "not yet computed" indicator
                    pvDividend = double.MaxValue;
                }
            }
            if (msgVersion >= 6)
            {
                gamma = ReadDouble();
                if (gamma.Equals(-2))
                { // -2 is the "not yet computed" indicator
                    gamma = double.MaxValue;
                }
                vega = ReadDouble();
                if (vega.Equals(-2))
                { // -2 is the "not yet computed" indicator
                    vega = double.MaxValue;
                }
                theta = ReadDouble();
                if (theta.Equals(-2))
                { // -2 is the "not yet computed" indicator
                    theta = double.MaxValue;
                }
                undPrice = ReadDouble();
                if (undPrice.Equals(-1))
                { // -1 is the "not yet computed" indicator
                    undPrice = double.MaxValue;
                }
            }

            eWrapper.tickOptionComputation(requestId, tickType, tickAttrib, impliedVolatility, delta, optPrice, pvDividend, gamma, vega, theta, undPrice);
        }

        private void TickOptionComputationEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.TickOptionComputation tickOptionComputationProto = protobuf.TickOptionComputation.Parser.ParseFrom(byteArray);

            eWrapper.tickOptionComputationProtoBuf(tickOptionComputationProto);

            int reqId = tickOptionComputationProto.HasReqId ? tickOptionComputationProto.ReqId : IncomingMessage.NotValid;
            int tickType = tickOptionComputationProto.HasTickType ? tickOptionComputationProto.TickType : 0;
            int tickAttrib = tickOptionComputationProto.HasTickAttrib ? tickOptionComputationProto.TickAttrib : 0;

            double impliedVol = tickOptionComputationProto.HasImpliedVol ? tickOptionComputationProto.ImpliedVol : double.MaxValue;
            if (impliedVol.Equals(-1))
            { // -1 is the "not yet computed" indicator
                impliedVol = double.MaxValue;
            }

            double delta = tickOptionComputationProto.HasDelta ? tickOptionComputationProto.Delta : double.MaxValue;
            if (delta.Equals(-2))
            { // -2 is the "not yet computed" indicator
                delta = double.MaxValue;
            }

            double optPrice = tickOptionComputationProto.HasOptPrice ? tickOptionComputationProto.OptPrice : double.MaxValue;
            if (optPrice.Equals(-1))
            { // -1 is the "not yet computed" indicator
                optPrice = double.MaxValue;
            }

            double pvDividend = tickOptionComputationProto.HasPvDividend ? tickOptionComputationProto.PvDividend : double.MaxValue;
            if (pvDividend.Equals(-1))
            { // -1 is the "not yet computed" indicator
                pvDividend = double.MaxValue;
            }

            double gamma = tickOptionComputationProto.HasGamma ? tickOptionComputationProto.Gamma : double.MaxValue;
            if (gamma.Equals(-2))
            { // -2 is the "not yet computed" indicator
                gamma = double.MaxValue;
            }

            double vega = tickOptionComputationProto.HasVega ? tickOptionComputationProto.Vega : double.MaxValue;
            if (vega.Equals(-2))
            { // -2 is the "not yet computed" indicator
                vega = double.MaxValue;
            }

            double theta = tickOptionComputationProto.HasTheta ? tickOptionComputationProto.Theta : double.MaxValue;
            if (theta.Equals(-2))
            { // -2 is the "not yet computed" indicator
                theta = double.MaxValue;
            }

            double undPrice = tickOptionComputationProto.HasUndPrice ? tickOptionComputationProto.UndPrice : double.MaxValue;
            if (undPrice.Equals(-1))
            { // -1 is the "not yet computed" indicator
                undPrice = double.MaxValue;
            }

            eWrapper.tickOptionComputation(reqId, tickType, tickAttrib, impliedVol, delta, optPrice, pvDividend, gamma, vega, theta, undPrice);
        }

        private void AccountSummaryEvent()
        {
            _ = ReadInt(); //msgVersion
            var requestId = ReadInt();
            var account = ReadString();
            var tag = ReadString();
            var value = ReadString();
            var currency = ReadString();
            eWrapper.accountSummary(requestId, account, tag, value, currency);
        }

        private void AccountSummaryEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.AccountSummary accountSummaryProto = protobuf.AccountSummary.Parser.ParseFrom(byteArray);

            eWrapper.accountSummaryProtoBuf(accountSummaryProto);

            int reqId = accountSummaryProto.HasReqId ? accountSummaryProto.ReqId : IncomingMessage.NotValid;
            string account = accountSummaryProto.HasAccount ? accountSummaryProto.Account : "";
            string tag = accountSummaryProto.HasTag ? accountSummaryProto.Tag : "";
            string value = accountSummaryProto.HasValue ? accountSummaryProto.Value : "";
            string currency = accountSummaryProto.HasCurrency ? accountSummaryProto.Currency : "";

            eWrapper.accountSummary(reqId, account, tag, value, currency);
        }

        private void AccountSummaryEndEvent()
        {
            _ = ReadInt(); //msgVersion
            var requestId = ReadInt();
            eWrapper.accountSummaryEnd(requestId);
        }

        private void AccountSummaryEndEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.AccountSummaryEnd accountSummaryEndProto = protobuf.AccountSummaryEnd.Parser.ParseFrom(byteArray);

            eWrapper.accountSummaryEndProtoBuf(accountSummaryEndProto);

            int reqId = accountSummaryEndProto.HasReqId ? accountSummaryEndProto.ReqId : IncomingMessage.NotValid;

            eWrapper.accountSummaryEnd(reqId);
        }

        private void AccountValueEvent()
        {
            var msgVersion = ReadInt();
            var key = ReadString();
            var value = ReadString();
            var currency = ReadString();
            string accountName = null;
            if (msgVersion >= 2) accountName = ReadString();
            eWrapper.updateAccountValue(key, value, currency, accountName);
        }

        private void AccountValueEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.AccountValue accountValueProto = protobuf.AccountValue.Parser.ParseFrom(byteArray);

            eWrapper.updateAccountValueProtoBuf(accountValueProto);

            string key = accountValueProto.HasKey ? accountValueProto.Key : "";
            string value = accountValueProto.HasValue ? accountValueProto.Value : "";
            string currency = accountValueProto.HasCurrency ? accountValueProto.Currency : "";
            string accountName = accountValueProto.HasAccountName ? accountValueProto.AccountName : "";

            eWrapper.updateAccountValue(key, value, currency, accountName);
        }

        private void BondContractDetailsEvent()
        {
            var msgVersion = 6;
            if (serverVersion < MinServerVer.SIZE_RULES) msgVersion = ReadInt();
            var requestId = -1;
            if (msgVersion >= 3) requestId = ReadInt();

            var contract = new ContractDetails();

            contract.Contract.Symbol = ReadString();
            contract.Contract.SecType = ReadString();
            contract.Cusip = ReadString();
            contract.Coupon = ReadDouble();
            readLastTradeDate(contract, true);
            contract.IssueDate = ReadString();
            contract.Ratings = ReadString();
            contract.BondType = ReadString();
            contract.CouponType = ReadString();
            contract.Convertible = ReadBoolFromInt();
            contract.Callable = ReadBoolFromInt();
            contract.Putable = ReadBoolFromInt();
            contract.DescAppend = ReadString();
            contract.Contract.Exchange = ReadString();
            contract.Contract.Currency = ReadString();
            contract.MarketName = ReadString();
            contract.Contract.TradingClass = ReadString();
            contract.Contract.ConId = ReadInt();
            contract.MinTick = ReadDouble();
            if (serverVersion >= MinServerVer.MD_SIZE_MULTIPLIER && serverVersion < MinServerVer.SIZE_RULES) ReadInt(); // MdSizeMultiplier - not used anymore
            contract.OrderTypes = ReadString();
            contract.ValidExchanges = ReadString();
            if (msgVersion >= 2)
            {
                contract.NextOptionDate = ReadString();
                contract.NextOptionType = ReadString();
                contract.NextOptionPartial = ReadBoolFromInt();
                contract.Notes = ReadString();
            }
            if (msgVersion >= 4) contract.LongName = ReadString();
            if (serverVersion >= MinServerVer.MIN_SERVER_VER_BOND_TRADING_HOURS)
            {
                contract.TimeZoneId = ReadString();
                contract.TradingHours = ReadString();
                contract.LiquidHours = ReadString();
            }
            if (msgVersion >= 6)
            {
                contract.EvRule = ReadString();
                contract.EvMultiplier = ReadDouble();
            }
            if (msgVersion >= 5)
            {
                var secIdListCount = ReadInt();
                if (secIdListCount > 0)
                {
                    contract.SecIdList = new List<TagValue>();
                    for (var i = 0; i < secIdListCount; ++i)
                    {
                        var tagValue = new TagValue
                        {
                            Tag = ReadString(),
                            Value = ReadString()
                        };
                        contract.SecIdList.Add(tagValue);
                    }
                }
            }
            if (serverVersion >= MinServerVer.AGG_GROUP) contract.AggGroup = ReadInt();
            if (serverVersion >= MinServerVer.MARKET_RULES) contract.MarketRuleIds = ReadString();
            if (serverVersion >= MinServerVer.SIZE_RULES)
            {
                contract.MinSize = ReadDecimal();
                contract.SizeIncrement = ReadDecimal();
                contract.SuggestedSizeIncrement = ReadDecimal();
            }

            eWrapper.bondContractDetails(requestId, contract);
        }

        private void BondContractDataEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.ContractData contractDataProto = protobuf.ContractData.Parser.ParseFrom(byteArray);

            eWrapper.bondContractDataProtoBuf(contractDataProto);

            int reqId = contractDataProto.HasReqId ? contractDataProto.ReqId : IncomingMessage.NotValid;

            if (contractDataProto.Contract is null || contractDataProto.ContractDetails is null)
            {
                return;
            }
            // set contract details fields
            ContractDetails contractDetails = EDecoderUtils.decodeContractDetails(contractDataProto.Contract, contractDataProto.ContractDetails, true);

            eWrapper.bondContractDetails(reqId, contractDetails);
        }

        private void PortfolioValueEvent()
        {
            var msgVersion = ReadInt();
            var contract = new Contract();
            if (msgVersion >= 6) contract.ConId = ReadInt();
            contract.Symbol = ReadString();
            contract.SecType = ReadString();
            contract.LastTradeDateOrContractMonth = ReadString();
            contract.Strike = ReadDouble();
            contract.Right = ReadString();
            if (msgVersion >= 7)
            {
                contract.Multiplier = ReadString();
                contract.PrimaryExch = ReadString();
            }
            contract.Currency = ReadString();
            if (msgVersion >= 2) contract.LocalSymbol = ReadString();
            if (msgVersion >= 8) contract.TradingClass = ReadString();

            var position = ReadDecimal();
            var marketPrice = ReadDouble();
            var marketValue = ReadDouble();
            var averageCost = 0.0;
            var unrealizedPNL = 0.0;
            var realizedPNL = 0.0;
            if (msgVersion >= 3)
            {
                averageCost = ReadDouble();
                unrealizedPNL = ReadDouble();
                realizedPNL = ReadDouble();
            }

            string accountName = null;
            if (msgVersion >= 4) accountName = ReadString();

            if (msgVersion == 6 && serverVersion == 39) contract.PrimaryExch = ReadString();

            eWrapper.updatePortfolio(contract, position, marketPrice, marketValue, averageCost, unrealizedPNL, realizedPNL, accountName);
        }

        private void PortfolioValueEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.PortfolioValue portfolioValueProto = protobuf.PortfolioValue.Parser.ParseFrom(byteArray);

            eWrapper.updatePortfolioProtoBuf(portfolioValueProto);

            if (portfolioValueProto.Contract == null)
            {
                return;
            }

            Contract contract = EDecoderUtils.decodeContract(portfolioValueProto.Contract);
            Decimal position = portfolioValueProto.HasPosition ? Util.StringToDecimal(portfolioValueProto.Position) : Decimal.MaxValue;
            double marketPrice = portfolioValueProto.HasMarketPrice ? portfolioValueProto.MarketPrice : 0;
            double marketValue = portfolioValueProto.HasMarketValue ? portfolioValueProto.MarketValue : 0;
            double averageCost = portfolioValueProto.HasAverageCost ? portfolioValueProto.AverageCost : 0;
            double unrealizedPNL = portfolioValueProto.HasUnrealizedPNL ? portfolioValueProto.UnrealizedPNL : 0;
            double realizedPNL = portfolioValueProto.HasRealizedPNL ? portfolioValueProto.RealizedPNL : 0;
            string accountName = portfolioValueProto.HasAccountName ? portfolioValueProto.AccountName : "";

            eWrapper.updatePortfolio(contract, position, marketPrice, marketValue, averageCost, unrealizedPNL, realizedPNL, accountName);
        }

        private void AccountUpdateTimeEvent()
        {
            _ = ReadInt(); //msgVersion
            var timestamp = ReadString();
            eWrapper.updateAccountTime(timestamp);
        }

        private void AccountUpdateTimeEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.AccountUpdateTime accountUpdateTimeProto = protobuf.AccountUpdateTime.Parser.ParseFrom(byteArray);

            eWrapper.updateAccountTimeProtoBuf(accountUpdateTimeProto);

            string timestamp = accountUpdateTimeProto.HasTimeStamp ? accountUpdateTimeProto.TimeStamp : "";

            eWrapper.updateAccountTime(timestamp);
        }

        private void AccountDownloadEndEvent()
        {
            _ = ReadInt(); //msgVersion
            var account = ReadString();
            eWrapper.accountDownloadEnd(account);
        }

        private void AccountDataEndEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.AccountDataEnd accountDataEndProto = protobuf.AccountDataEnd.Parser.ParseFrom(byteArray);

            eWrapper.accountDataEndProtoBuf(accountDataEndProto);

            string accountName = accountDataEndProto.HasAccountName ? accountDataEndProto.AccountName : "";

            eWrapper.accountDownloadEnd(accountName);
        }

        private void OrderStatusEvent()
        {
            var msgVersion = serverVersion >= MinServerVer.MARKET_CAP_PRICE ? int.MaxValue : ReadInt();
            var id = ReadInt();
            var status = ReadString();
            var filled = ReadDecimal();
            var remaining = ReadDecimal();
            var avgFillPrice = ReadDouble();

            long permId = 0;
            if (msgVersion >= 2) permId = ReadLong();

            var parentId = 0;
            if (msgVersion >= 3) parentId = ReadInt();

            double lastFillPrice = 0;
            if (msgVersion >= 4) lastFillPrice = ReadDouble();

            var clientId = 0;
            if (msgVersion >= 5) clientId = ReadInt();

            string whyHeld = null;
            if (msgVersion >= 6) whyHeld = ReadString();

            var mktCapPrice = double.MaxValue;

            if (serverVersion >= MinServerVer.MARKET_CAP_PRICE) mktCapPrice = ReadDouble();

            eWrapper.orderStatus(id, status, filled, remaining, avgFillPrice, permId, parentId, lastFillPrice, clientId, whyHeld, mktCapPrice);
        }

        private void OrderStatusEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.OrderStatus orderStatusProto = protobuf.OrderStatus.Parser.ParseFrom(byteArray);

            eWrapper.orderStatusProtoBuf(orderStatusProto);

            int orderId = orderStatusProto.HasOrderId ? orderStatusProto.OrderId : int.MaxValue;
            string status = orderStatusProto.HasStatus ? orderStatusProto.Status : "";
            decimal filled = orderStatusProto.HasFilled ? Util.StringToDecimal(orderStatusProto.Filled) : decimal.MaxValue;
            decimal remaining = orderStatusProto.HasRemaining ? Util.StringToDecimal(orderStatusProto.Remaining) : decimal.MaxValue;
            double avgFillPrice = orderStatusProto.HasAvgFillPrice ? orderStatusProto.AvgFillPrice : double.MaxValue;
            long permId = orderStatusProto.HasPermId ? orderStatusProto.PermId : long.MaxValue;
            int parentId = orderStatusProto.HasParentId ? orderStatusProto.ParentId : int.MaxValue;
            double lastFillPrice = orderStatusProto.HasLastFillPrice ? orderStatusProto.LastFillPrice : double.MaxValue;
            int clientId = orderStatusProto.HasClientId ? orderStatusProto.ClientId : int.MaxValue;
            string whyHeld = orderStatusProto.HasWhyHeld ? orderStatusProto.WhyHeld : "";
            double mktCapPrice = orderStatusProto.HasMktCapPrice ? orderStatusProto.MktCapPrice : double.MaxValue;

            eWrapper.orderStatus(orderId, status, filled, remaining, avgFillPrice, permId, parentId, lastFillPrice, clientId, whyHeld, mktCapPrice);
        }

        private void OpenOrderEvent()
        {
            var msgVersion = serverVersion < MinServerVer.ORDER_CONTAINER ? ReadInt() : serverVersion;

            var contract = new Contract();
            var order = new Order();
            var orderState = new OrderState();
            var eOrderDecoder = new EOrderDecoder(this, contract, order, orderState, msgVersion, serverVersion);

            // read order id
            eOrderDecoder.readOrderId();

            // read contract fields
            eOrderDecoder.readContractFields();

            // read order fields
            eOrderDecoder.readAction();
            eOrderDecoder.readTotalQuantity();
            eOrderDecoder.readOrderType();
            eOrderDecoder.readLmtPrice();
            eOrderDecoder.readAuxPrice();
            eOrderDecoder.readTIF();
            eOrderDecoder.readOcaGroup();
            eOrderDecoder.readAccount();
            eOrderDecoder.readOpenClose();
            eOrderDecoder.readOrigin();
            eOrderDecoder.readOrderRef();
            eOrderDecoder.readClientId();
            eOrderDecoder.readPermId();
            eOrderDecoder.readOutsideRth();
            eOrderDecoder.readHidden();
            eOrderDecoder.readDiscretionaryAmount();
            eOrderDecoder.readGoodAfterTime();
            eOrderDecoder.skipSharesAllocation();
            eOrderDecoder.readFAParams();
            eOrderDecoder.readModelCode();
            eOrderDecoder.readGoodTillDate();
            eOrderDecoder.readRule80A();
            eOrderDecoder.readPercentOffset();
            eOrderDecoder.readSettlingFirm();
            eOrderDecoder.readShortSaleParams();
            eOrderDecoder.readAuctionStrategy();
            eOrderDecoder.readBoxOrderParams();
            eOrderDecoder.readPegToStkOrVolOrderParams();
            eOrderDecoder.readDisplaySize();
            eOrderDecoder.readOldStyleOutsideRth();
            eOrderDecoder.readBlockOrder();
            eOrderDecoder.readSweepToFill();
            eOrderDecoder.readAllOrNone();
            eOrderDecoder.readMinQty();
            eOrderDecoder.readOcaType();
            eOrderDecoder.skipETradeOnly();
            eOrderDecoder.skipFirmQuoteOnly();
            eOrderDecoder.skipNbboPriceCap();
            eOrderDecoder.readParentId();
            eOrderDecoder.readTriggerMethod();
            eOrderDecoder.readVolOrderParams(true);
            eOrderDecoder.readTrailParams();
            eOrderDecoder.readBasisPoints();
            eOrderDecoder.readComboLegs();
            eOrderDecoder.readSmartComboRoutingParams();
            eOrderDecoder.readScaleOrderParams();
            eOrderDecoder.readHedgeParams();
            eOrderDecoder.readOptOutSmartRouting();
            eOrderDecoder.readClearingParams();
            eOrderDecoder.readNotHeld();
            eOrderDecoder.readDeltaNeutral();
            eOrderDecoder.readAlgoParams();
            eOrderDecoder.readSolicited();
            eOrderDecoder.readWhatIfInfoAndCommissionAndFees();
            eOrderDecoder.readVolRandomizeFlags();
            eOrderDecoder.readPegToBenchParams();
            eOrderDecoder.readConditions();
            eOrderDecoder.readAdjustedOrderParams();
            eOrderDecoder.readSoftDollarTier();
            eOrderDecoder.readCashQty();
            eOrderDecoder.readDontUseAutoPriceForHedge();
            eOrderDecoder.readIsOmsContainer();
            eOrderDecoder.readDiscretionaryUpToLimitPrice();
            eOrderDecoder.readUsePriceMgmtAlgo();
            eOrderDecoder.readDuration();
            eOrderDecoder.readPostToAts();
            eOrderDecoder.readAutoCancelParent(MinServerVer.AUTO_CANCEL_PARENT);
            eOrderDecoder.readPegBestPegMidOrderAttributes();
            eOrderDecoder.readCustomerAccount();
            eOrderDecoder.readProfessionalCustomer();
            eOrderDecoder.readBondAccruedInterest();
            eOrderDecoder.readIncludeOvernight();
            eOrderDecoder.readCMETaggingFields();
            eOrderDecoder.readSubmitter();
            eOrderDecoder.readImbalanceOnly(MinServerVer.MIN_SERVER_VER_IMBALANCE_ONLY);

            eWrapper.openOrder(order.OrderId, contract, order, orderState);
        }

        private void OpenOrderEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.OpenOrder openOrderProto = protobuf.OpenOrder.Parser.ParseFrom(byteArray);

            eWrapper.openOrderProtoBuf(openOrderProto);

            int orderId = openOrderProto.HasOrderId ? openOrderProto.OrderId : 0;

            // set contract fields
            if (openOrderProto.Contract == null)
            {
                return;
            }
            Contract contract = EDecoderUtils.decodeContract(openOrderProto.Contract);

            // set order fields
            if (openOrderProto.Order == null)
            {
                return;
            }
            Order order = EDecoderUtils.decodeOrder(orderId, openOrderProto.Contract, openOrderProto.Order);

            // set order state fields
            if (openOrderProto.OrderState == null)
            {
                return;
            }
            OrderState orderState = EDecoderUtils.decodeOrderState(openOrderProto.OrderState);

            eWrapper.openOrder(orderId, contract, order, orderState);
        }

        private void OpenOrderEndEvent()
        {
            _ = ReadInt(); //msgVersion
            eWrapper.openOrderEnd();
        }

        private void OpenOrderEndEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.OpenOrdersEnd openOrdersEndProto = protobuf.OpenOrdersEnd.Parser.ParseFrom(byteArray);

            eWrapper.openOrdersEndProtoBuf(openOrdersEndProto);

            eWrapper.openOrderEnd();
        }

        private void ContractDataEvent()
        {
            var msgVersion = 8;
            if (serverVersion < MinServerVer.SIZE_RULES) msgVersion = ReadInt();
            var requestId = -1;
            if (msgVersion >= 3) requestId = ReadInt();
            var contract = new ContractDetails();
            contract.Contract.Symbol = ReadString();
            contract.Contract.SecType = ReadString();
            readLastTradeDate(contract, false);
            if (serverVersion >= MinServerVer.MIN_SERVER_VER_LAST_TRADE_DATE) contract.Contract.LastTradeDate = ReadString();
            contract.Contract.Strike = ReadDouble();
            contract.Contract.Right = ReadString();
            contract.Contract.Exchange = ReadString();
            contract.Contract.Currency = ReadString();
            contract.Contract.LocalSymbol = ReadString();
            contract.MarketName = ReadString();
            contract.Contract.TradingClass = ReadString();
            contract.Contract.ConId = ReadInt();
            contract.MinTick = ReadDouble();
            if (serverVersion >= MinServerVer.MD_SIZE_MULTIPLIER && serverVersion < MinServerVer.SIZE_RULES) ReadInt(); // MdSizeMultiplier - not used anymore
            contract.Contract.Multiplier = ReadString();
            contract.OrderTypes = ReadString();
            contract.ValidExchanges = ReadString();
            if (msgVersion >= 2) contract.PriceMagnifier = ReadInt();
            if (msgVersion >= 4) contract.UnderConId = ReadInt();
            if (msgVersion >= 5)
            {
                contract.LongName = serverVersion >= MinServerVer.ENCODE_MSG_ASCII7 ? Regex.Unescape(ReadString()) : ReadString();
                contract.Contract.PrimaryExch = ReadString();
            }
            if (msgVersion >= 6)
            {
                contract.ContractMonth = ReadString();
                contract.Industry = ReadString();
                contract.Category = ReadString();
                contract.Subcategory = ReadString();
                contract.TimeZoneId = ReadString();
                contract.TradingHours = ReadString();
                contract.LiquidHours = ReadString();
            }
            if (msgVersion >= 8)
            {
                contract.EvRule = ReadString();
                contract.EvMultiplier = ReadDouble();
            }
            if (msgVersion >= 7)
            {
                var secIdListCount = ReadInt();
                if (secIdListCount > 0)
                {
                    contract.SecIdList = new List<TagValue>(secIdListCount);
                    for (var i = 0; i < secIdListCount; ++i)
                    {
                        var tagValue = new TagValue
                        {
                            Tag = ReadString(),
                            Value = ReadString()
                        };
                        contract.SecIdList.Add(tagValue);
                    }
                }
            }
            if (serverVersion >= MinServerVer.AGG_GROUP) contract.AggGroup = ReadInt();
            if (serverVersion >= MinServerVer.UNDERLYING_INFO)
            {
                contract.UnderSymbol = ReadString();
                contract.UnderSecType = ReadString();
            }
            if (serverVersion >= MinServerVer.MARKET_RULES) contract.MarketRuleIds = ReadString();
            if (serverVersion >= MinServerVer.REAL_EXPIRATION_DATE) contract.RealExpirationDate = ReadString();
            if (serverVersion >= MinServerVer.STOCK_TYPE) contract.StockType = ReadString();
            if (serverVersion >= MinServerVer.FRACTIONAL_SIZE_SUPPORT && serverVersion < MinServerVer.SIZE_RULES) ReadDecimal(); // SizeMinTick - not used anymore
            if (serverVersion >= MinServerVer.SIZE_RULES)
            {
                contract.MinSize = ReadDecimal();
                contract.SizeIncrement = ReadDecimal();
                contract.SuggestedSizeIncrement = ReadDecimal();
            }
            if (serverVersion >= MinServerVer.MIN_SERVER_VER_FUND_DATA_FIELDS && contract.Contract.SecType == "FUND")
            {
                contract.FundName = ReadString();
                contract.FundFamily = ReadString();
                contract.FundType = ReadString();
                contract.FundFrontLoad = ReadString();
                contract.FundBackLoad = ReadString();
                contract.FundBackLoadTimeInterval = ReadString();
                contract.FundManagementFee = ReadString();
                contract.FundClosed = ReadBoolFromInt();
                contract.FundClosedForNewInvestors = ReadBoolFromInt();
                contract.FundClosedForNewMoney = ReadBoolFromInt();
                contract.FundNotifyAmount = ReadString();
                contract.FundMinimumInitialPurchase = ReadString();
                contract.FundSubsequentMinimumPurchase = ReadString();
                contract.FundBlueSkyStates = ReadString();
                contract.FundBlueSkyTerritories = ReadString();
                contract.FundDistributionPolicyIndicator = CFundDistributionPolicyIndicator.getFundDistributionPolicyIndicator(ReadString());
                contract.FundAssetType = CFundAssetType.getFundAssetType(ReadString());
            }

            if (serverVersion >= MinServerVer.MIN_SERVER_VER_INELIGIBILITY_REASONS)
            {
                var ineligibilityReasonCount = ReadInt();
                if (ineligibilityReasonCount > 0)
                {
                    contract.IneligibilityReasonList = new List<IneligibilityReason>();
                    for (var i = 0; i < ineligibilityReasonCount; ++i)
                    {
                        var ineligibilityReason = new IneligibilityReason
                        {
                            Id = ReadString(),
                            Description = ReadString()
                        };
                        contract.IneligibilityReasonList.Add(ineligibilityReason);
                    }
                }
            }

            eWrapper.contractDetails(requestId, contract);
        }

        private void ContractDataEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.ContractData contractDataProto = protobuf.ContractData.Parser.ParseFrom(byteArray);

            eWrapper.contractDataProtoBuf(contractDataProto);

            int reqId = contractDataProto.HasReqId ? contractDataProto.ReqId : IncomingMessage.NotValid;

            if (contractDataProto.Contract is null || contractDataProto.ContractDetails is null)
            {
                return;
            }
            // set contract details fields
            ContractDetails contractDetails = EDecoderUtils.decodeContractDetails(contractDataProto.Contract, contractDataProto.ContractDetails, false);

            eWrapper.contractDetails(reqId, contractDetails);
        }

        private void ContractDataEndEvent()
        {
            _ = ReadInt(); //msgVersion
            var requestId = ReadInt();
            eWrapper.contractDetailsEnd(requestId);
        }

        private void ContractDataEndEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.ContractDataEnd contractDataEndProto = protobuf.ContractDataEnd.Parser.ParseFrom(byteArray);

            eWrapper.contractDataEndProtoBuf(contractDataEndProto);

            int reqId = contractDataEndProto.HasReqId ? contractDataEndProto.ReqId : IncomingMessage.NotValid;

            eWrapper.contractDetailsEnd(reqId);
        }

        private void ExecutionDataEvent()
        {
            var msgVersion = serverVersion;

            if (serverVersion < MinServerVer.LAST_LIQUIDITY) msgVersion = ReadInt();

            var requestId = -1;
            if (msgVersion >= 7) requestId = ReadInt();
            var orderId = ReadInt();
            var contract = new Contract();
            if (msgVersion >= 5) contract.ConId = ReadInt();
            contract.Symbol = ReadString();
            contract.SecType = ReadString();
            contract.LastTradeDateOrContractMonth = ReadString();
            contract.Strike = ReadDouble();
            contract.Right = ReadString();
            if (msgVersion >= 9) contract.Multiplier = ReadString();
            contract.Exchange = ReadString();
            contract.Currency = ReadString();
            contract.LocalSymbol = ReadString();
            if (msgVersion >= 10) contract.TradingClass = ReadString();

            var exec = new Execution
            {
                OrderId = orderId,
                ExecId = ReadString(),
                Time = ReadString(),
                AcctNumber = ReadString(),
                Exchange = ReadString(),
                Side = ReadString(),
                Shares = ReadDecimal(),
                Price = ReadDouble()
            };
            if (msgVersion >= 2) exec.PermId = ReadLong();
            if (msgVersion >= 3) exec.ClientId = ReadInt();
            if (msgVersion >= 4) exec.Liquidation = ReadInt();
            if (msgVersion >= 6)
            {
                exec.CumQty = ReadDecimal();
                exec.AvgPrice = ReadDouble();
            }
            if (msgVersion >= 8) exec.OrderRef = ReadString();
            if (msgVersion >= 9)
            {
                exec.EvRule = ReadString();
                exec.EvMultiplier = ReadDouble();
            }
            if (serverVersion >= MinServerVer.MODELS_SUPPORT) exec.ModelCode = ReadString();

            if (serverVersion >= MinServerVer.LAST_LIQUIDITY) exec.LastLiquidity = new Liquidity(ReadInt());

            if (serverVersion >= MinServerVer.MIN_SERVER_VER_PENDING_PRICE_REVISION) exec.PendingPriceRevision = ReadBoolFromInt();

            if (serverVersion >= MinServerVer.MIN_SERVER_VER_SUBMITTER) exec.Submitter = ReadString();

            eWrapper.execDetails(requestId, contract, exec);
        }

        private void ExecutionDataEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.ExecutionDetails executionDetailsProto = protobuf.ExecutionDetails.Parser.ParseFrom(byteArray);

            eWrapper.execDetailsProtoBuf(executionDetailsProto);

            int reqId = executionDetailsProto.HasReqId ? executionDetailsProto.ReqId : IncomingMessage.NotValid;

            // set contract fields
            if (executionDetailsProto.Contract is null)
            {
                return;
            }
            Contract contract = EDecoderUtils.decodeContract(executionDetailsProto.Contract);

            // set execution fields
            if (executionDetailsProto.Execution is null)
            {
                return;
            }
            Execution execution = EDecoderUtils.decodeExecution(executionDetailsProto.Execution);

            eWrapper.execDetails(reqId, contract, execution);
        }

        private void ExecutionDataEndEvent()
        {
            _ = ReadInt(); //msgVersion
            var requestId = ReadInt();
            eWrapper.execDetailsEnd(requestId);
        }

        private void ExecutionDataEndEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.ExecutionDetailsEnd executionDetailsEndProto = protobuf.ExecutionDetailsEnd.Parser.ParseFrom(byteArray);

            eWrapper.execDetailsEndProtoBuf(executionDetailsEndProto);

            int reqId = executionDetailsEndProto.HasReqId ? executionDetailsEndProto.ReqId : IncomingMessage.NotValid;

            eWrapper.execDetailsEnd(reqId);
        }

        private void CommissionAndFeesReportEvent()
        {
            _ = ReadInt(); //msgVersion
            var commissionAndFeesReport = new CommissionAndFeesReport
            {
                ExecId = ReadString(),
                CommissionAndFees = ReadDouble(),
                Currency = ReadString(),
                RealizedPNL = ReadDouble(),
                Yield = ReadDouble(),
                YieldRedemptionDate = ReadInt()
            };
            eWrapper.commissionAndFeesReport(commissionAndFeesReport);
        }

        private void CommissionAndFeesReportEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.CommissionAndFeesReport commissionAndFeesReportProto = protobuf.CommissionAndFeesReport.Parser.ParseFrom(byteArray);

            eWrapper.commissionAndFeesReportProtoBuf(commissionAndFeesReportProto);

            CommissionAndFeesReport commissionAndFeesReport = new CommissionAndFeesReport();
            commissionAndFeesReport.ExecId = commissionAndFeesReportProto.HasExecId ? commissionAndFeesReportProto.ExecId : "";
            commissionAndFeesReport.CommissionAndFees = commissionAndFeesReportProto.HasCommissionAndFees ? commissionAndFeesReportProto.CommissionAndFees : 0;
            commissionAndFeesReport.Currency = commissionAndFeesReportProto.HasCurrency ? commissionAndFeesReportProto.Currency : "";
            commissionAndFeesReport.RealizedPNL = commissionAndFeesReportProto.HasRealizedPNL ? commissionAndFeesReportProto.RealizedPNL : 0;
            commissionAndFeesReport.Yield = commissionAndFeesReportProto.HasBondYield ? commissionAndFeesReportProto.BondYield : 0;
            commissionAndFeesReport.YieldRedemptionDate = commissionAndFeesReportProto.HasYieldRedemptionDate ? Util.StringToIntMax(commissionAndFeesReportProto.YieldRedemptionDate) : 0;

            eWrapper.commissionAndFeesReport(commissionAndFeesReport);
        }

        private void FundamentalDataEvent()
        {
            _ = ReadInt(); //msgVersion
            var requestId = ReadInt();
            var fundamentalData = ReadString();
            eWrapper.fundamentalData(requestId, fundamentalData);
        }

        private void FundamentalsDataEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);

            protobuf.FundamentalsData fundamentalsDataProto = protobuf.FundamentalsData.Parser.ParseFrom(byteArray);

            eWrapper.fundamentalsDataProtoBuf(fundamentalsDataProto);

            int reqId = fundamentalsDataProto.HasReqId ? fundamentalsDataProto.ReqId : IncomingMessage.NotValid;
            string data = fundamentalsDataProto.HasData ? fundamentalsDataProto.Data : "";

            eWrapper.fundamentalData(reqId, data);
        }

        private void HistoricalDataEvent()
        {
            var msgVersion = int.MaxValue;

            if (serverVersion < MinServerVer.SYNT_REALTIME_BARS) msgVersion = ReadInt();

            var requestId = ReadInt();
            var startDateStr = "";
            var endDateStr = "";

            if (msgVersion >= 2 && serverVersion < MinServerVer.MIN_SERVER_VER_HISTORICAL_DATA_END)
            {
                startDateStr = ReadString();
                endDateStr = ReadString();
            }

            var itemCount = ReadInt();

            for (var ctr = 0; ctr < itemCount; ctr++)
            {
                var date = ReadString();
                var open = ReadDouble();
                var high = ReadDouble();
                var low = ReadDouble();
                var close = ReadDouble();
                var volume = ReadDecimal();
                var WAP = ReadDecimal();

                if (serverVersion < MinServerVer.SYNT_REALTIME_BARS)
                {
                    /*string hasGaps = */
                    ReadString();
                }

                var barCount = -1;

                if (msgVersion >= 3) barCount = ReadInt();

                eWrapper.historicalData(requestId, new Bar(date, open, high, low, close, volume, barCount, WAP));
            }

            if (serverVersion < MinServerVer.MIN_SERVER_VER_HISTORICAL_DATA_END) 
            {
                // send end of dataset marker.
                eWrapper.historicalDataEnd(requestId, startDateStr, endDateStr);
            }
        }

        private void HistoricalDataEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.HistoricalData historicalDataProto = protobuf.HistoricalData.Parser.ParseFrom(byteArray);

            eWrapper.historicalDataProtoBuf(historicalDataProto);

            int reqId = historicalDataProto.HasReqId ? historicalDataProto.ReqId : IncomingMessage.NotValid;

            if (historicalDataProto.HistoricalDataBars != null && historicalDataProto.HistoricalDataBars.Count > 0)
            {
                foreach (var historicalDataBarProto in historicalDataProto.HistoricalDataBars)
                {
                    Bar bar = EDecoderUtils.decodeHistoricalDataBar(historicalDataBarProto);
                    eWrapper.historicalData(reqId, bar);
                }
            }
        }

        private void HistoricalDataEndEvent()
        {
            var requestId = ReadInt();
            var startDateStr = ReadString();
            var endDateStr = ReadString();

            eWrapper.historicalDataEnd(requestId, startDateStr, endDateStr);
        }

        private void HistoricalDataEndEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.HistoricalDataEnd historicalDataEndProto = protobuf.HistoricalDataEnd.Parser.ParseFrom(byteArray);

            eWrapper.historicalDataEndProtoBuf(historicalDataEndProto);

            int reqId = historicalDataEndProto.HasReqId ? historicalDataEndProto.ReqId : IncomingMessage.NotValid;
            string startDateStr = historicalDataEndProto.HasStartDateStr ? historicalDataEndProto.StartDateStr : "";
            string endDateStr = historicalDataEndProto.HasEndDateStr ? historicalDataEndProto.EndDateStr : "";

            eWrapper.historicalDataEnd(reqId, startDateStr, endDateStr);
        }

        private void MarketDataTypeEvent()
        {
            _ = ReadInt(); //msgVersion
            var requestId = ReadInt();
            var marketDataType = ReadInt();
            eWrapper.marketDataType(requestId, marketDataType);
        }

        private void MarketDataTypeEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.MarketDataType marketDataTypeProto = protobuf.MarketDataType.Parser.ParseFrom(byteArray);

            eWrapper.marketDataTypeProtoBuf(marketDataTypeProto);

            int reqId = marketDataTypeProto.HasReqId ? marketDataTypeProto.ReqId : IncomingMessage.NotValid;
            int marketDataType = marketDataTypeProto.HasMarketDataType_ ? marketDataTypeProto.MarketDataType_ : 0;

            eWrapper.marketDataType(reqId, marketDataType);
        }

        private void MarketDepthEvent()
        {
            _ = ReadInt(); //msgVersion
            var requestId = ReadInt();
            var position = ReadInt();
            var operation = ReadInt();
            var side = ReadInt();
            var price = ReadDouble();
            var size = ReadDecimal();
            eWrapper.updateMktDepth(requestId, position, operation, side, price, size);
        }

        private void MarketDepthEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.MarketDepth marketDepthProto = protobuf.MarketDepth.Parser.ParseFrom(byteArray);

            eWrapper.updateMarketDepthProtoBuf(marketDepthProto);

            int reqId = marketDepthProto.HasReqId ? marketDepthProto.ReqId : IncomingMessage.NotValid;

            if (marketDepthProto.MarketDepthData is null)
            {
                return;
            }

            protobuf.MarketDepthData marketDepthDataProto = marketDepthProto.MarketDepthData;
            int position = marketDepthDataProto.HasPosition ? marketDepthDataProto.Position : 0;
            int operation = marketDepthDataProto.HasOperation ? marketDepthDataProto.Operation : 0;
            int side = marketDepthDataProto.HasSide ? marketDepthDataProto.Side : 0;
            double price = marketDepthDataProto.HasPrice ? marketDepthDataProto.Price : 0;
            decimal size = marketDepthDataProto.HasSize ? Util.StringToDecimal(marketDepthDataProto.Size) : decimal.MaxValue;

            eWrapper.updateMktDepth(reqId, position, operation, side, price, size);
        }

        private void MarketDepthL2Event()
        {
            _ = ReadInt(); //msgVersion
            var requestId = ReadInt();
            var position = ReadInt();
            var marketMaker = ReadString();
            var operation = ReadInt();
            var side = ReadInt();
            var price = ReadDouble();
            var size = ReadDecimal();

            var isSmartDepth = false;
            if (serverVersion >= MinServerVer.SMART_DEPTH) isSmartDepth = ReadBoolFromInt();

            eWrapper.updateMktDepthL2(requestId, position, marketMaker, operation, side, price, size, isSmartDepth);
        }

        private void MarketDepthL2EventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.MarketDepthL2 marketDepthL2Proto = protobuf.MarketDepthL2.Parser.ParseFrom(byteArray);

            eWrapper.updateMarketDepthL2ProtoBuf(marketDepthL2Proto);

            int reqId = marketDepthL2Proto.HasReqId ? marketDepthL2Proto.ReqId : IncomingMessage.NotValid;

            if (marketDepthL2Proto.MarketDepthData is null)
            {
                return;
            }

            protobuf.MarketDepthData marketDepthDataProto = marketDepthL2Proto.MarketDepthData;
            int position = marketDepthDataProto.HasPosition ? marketDepthDataProto.Position : 0;
            string marketMaker = marketDepthDataProto.HasMarketMaker ? marketDepthDataProto.MarketMaker : "";
            int operation = marketDepthDataProto.HasOperation ? marketDepthDataProto.Operation : 0;
            int side = marketDepthDataProto.HasSide ? marketDepthDataProto.Side : 0;
            double price = marketDepthDataProto.HasPrice ? marketDepthDataProto.Price : 0;
            decimal size = marketDepthDataProto.HasSize ? Util.StringToDecimal(marketDepthDataProto.Size) : decimal.MaxValue;
            bool isSmartDepth = marketDepthDataProto.HasIsSmartDepth ? marketDepthDataProto.IsSmartDepth : false;

            eWrapper.updateMktDepthL2(reqId, position, marketMaker, operation, side, price, size, isSmartDepth);
        }

        private void NewsBulletinsEvent()
        {
            _ = ReadInt(); //msgVersion
            var newsMsgId = ReadInt();
            var newsMsgType = ReadInt();
            var newsMessage = ReadString();
            var originatingExch = ReadString();
            eWrapper.updateNewsBulletin(newsMsgId, newsMsgType, newsMessage, originatingExch);
        }

        private void NewsBulletinEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.NewsBulletin newsBulletinProto = protobuf.NewsBulletin.Parser.ParseFrom(byteArray);

            eWrapper.updateNewsBulletinProtoBuf(newsBulletinProto);

            int msgId = newsBulletinProto.HasNewsMsgId ? newsBulletinProto.NewsMsgId : 0;
            int msgType = newsBulletinProto.HasNewsMsgType ? newsBulletinProto.NewsMsgType : 0;
            string message = newsBulletinProto.HasNewsMessage ? newsBulletinProto.NewsMessage : "";
            string origExchange = newsBulletinProto.HasOriginatingExch ? newsBulletinProto.OriginatingExch : "";

            eWrapper.updateNewsBulletin(msgId, msgType, message, origExchange);
        }

        private void PositionEvent()
        {
            var msgVersion = ReadInt();
            var account = ReadString();
            var contract = new Contract
            {
                ConId = ReadInt(),
                Symbol = ReadString(),
                SecType = ReadString(),
                LastTradeDateOrContractMonth = ReadString(),
                Strike = ReadDouble(),
                Right = ReadString(),
                Multiplier = ReadString(),
                Exchange = ReadString(),
                Currency = ReadString(),
                LocalSymbol = ReadString()
            };
            if (msgVersion >= 2) contract.TradingClass = ReadString();

            var pos = ReadDecimal();
            double avgCost = 0;
            if (msgVersion >= 3) avgCost = ReadDouble();
            eWrapper.position(account, contract, pos, avgCost);
        }

        private void PositionEndEvent()
        {
            _ = ReadInt(); //msgVersion
            eWrapper.positionEnd();
        }

        private void PositionEndEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.PositionEnd positionEndProto = protobuf.PositionEnd.Parser.ParseFrom(byteArray);

            eWrapper.positionEndProtoBuf(positionEndProto);

            eWrapper.positionEnd();
        }

        private void PositionEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.Position positionProto = protobuf.Position.Parser.ParseFrom(byteArray);

            eWrapper.positionProtoBuf(positionProto);

            if (positionProto.Contract == null)
            {
                return;
            }

            Contract contract = EDecoderUtils.decodeContract(positionProto.Contract);
            Decimal position = positionProto.HasPosition_ ? Util.StringToDecimal(positionProto.Position_) : Decimal.MaxValue;
            double avgCost = positionProto.HasAvgCost ? positionProto.AvgCost : 0;
            string account = positionProto.HasAccount ? positionProto.Account : "";

            eWrapper.position(account, contract, position, avgCost);
        }

        private void RealTimeBarsEvent()
        {
            _ = ReadInt(); //msgVersion
            var requestId = ReadInt();
            var time = ReadLong();
            var open = ReadDouble();
            var high = ReadDouble();
            var low = ReadDouble();
            var close = ReadDouble();
            var volume = ReadDecimal();
            var wap = ReadDecimal();
            var count = ReadInt();
            eWrapper.realtimeBar(requestId, time, open, high, low, close, volume, wap, count);
        }

        private void RealTimeBarEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.RealTimeBarTick realTimeBarTickProto = protobuf.RealTimeBarTick.Parser.ParseFrom(byteArray);

            eWrapper.realTimeBarTickProtoBuf(realTimeBarTickProto);

            int reqId = realTimeBarTickProto.HasReqId ? realTimeBarTickProto.ReqId : IncomingMessage.NotValid;
            long time = realTimeBarTickProto.HasTime ? realTimeBarTickProto.Time : 0;
            double open = realTimeBarTickProto.HasOpen ? realTimeBarTickProto.Open : 0;
            double high = realTimeBarTickProto.HasHigh ? realTimeBarTickProto.High : 0;
            double low = realTimeBarTickProto.HasLow ? realTimeBarTickProto.Low : 0;
            double close = realTimeBarTickProto.HasClose ? realTimeBarTickProto.Close : 0;
            decimal volume = realTimeBarTickProto.HasVolume ? Util.StringToDecimal(realTimeBarTickProto.Volume) : decimal.MaxValue;
            decimal wap = realTimeBarTickProto.HasWAP ? Util.StringToDecimal(realTimeBarTickProto.WAP) : decimal.MaxValue;
            int count = realTimeBarTickProto.HasCount ? realTimeBarTickProto.Count : 0;

            eWrapper.realtimeBar(reqId, time, open, high, low, close, volume, wap, count);
        }

        private void ScannerParametersEvent()
        {
            _ = ReadInt(); //msgVersion
            var xml = ReadString();
            eWrapper.scannerParameters(xml);
        }

        private void ScannerParametersEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);

            protobuf.ScannerParameters scannerParametersProto = protobuf.ScannerParameters.Parser.ParseFrom(byteArray);

            eWrapper.scannerParametersProtoBuf(scannerParametersProto);

            string xml = scannerParametersProto.HasXml ? scannerParametersProto.Xml : "";

            eWrapper.scannerParameters(xml);
        }

        private void ScannerDataEvent()
        {
            var msgVersion = ReadInt();
            var requestId = ReadInt();
            var numberOfElements = ReadInt();
            for (var i = 0; i < numberOfElements; i++)
            {
                var rank = ReadInt();
                var conDet = new ContractDetails();
                if (msgVersion >= 3) conDet.Contract.ConId = ReadInt();
                conDet.Contract.Symbol = ReadString();
                conDet.Contract.SecType = ReadString();
                conDet.Contract.LastTradeDateOrContractMonth = ReadString();
                conDet.Contract.Strike = ReadDouble();
                conDet.Contract.Right = ReadString();
                conDet.Contract.Exchange = ReadString();
                conDet.Contract.Currency = ReadString();
                conDet.Contract.LocalSymbol = ReadString();
                conDet.MarketName = ReadString();
                conDet.Contract.TradingClass = ReadString();
                var distance = ReadString();
                var benchmark = ReadString();
                var projection = ReadString();
                string legsStr = null;
                if (msgVersion >= 2) legsStr = ReadString();
                eWrapper.scannerData(requestId, rank, conDet, distance, benchmark, projection, legsStr);
            }
            eWrapper.scannerDataEnd(requestId);
        }

        private void ScannerDataEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);

            protobuf.ScannerData scannerDataProto = protobuf.ScannerData.Parser.ParseFrom(byteArray);

            eWrapper.scannerDataProtoBuf(scannerDataProto);

            int reqId = scannerDataProto.HasReqId ? scannerDataProto.ReqId : IncomingMessage.NotValid;

            if (scannerDataProto.ScannerDataElement.Count > 0)
            {
                foreach (protobuf.ScannerDataElement element in scannerDataProto.ScannerDataElement)
                {
                    int rank = element.HasRank ? element.Rank : 0;

                    // Set contract details
                    if (element.Contract == null) continue;
                    Contract contract = EDecoderUtils.decodeContract(element.Contract);
                    ContractDetails contractDetails = new ContractDetails();
                    if (contract != null) contractDetails.Contract = contract;
                    contractDetails.MarketName = element.HasMarketName ? element.MarketName : "";

                    string distance = element.HasDistance ? element.Distance : "";
                    string benchmark = element.HasBenchmark ? element.Benchmark : "";
                    string projection = element.HasProjection ? element.Projection : "";
                    string comboKey = element.HasComboKey ? element.ComboKey : "";

                    eWrapper.scannerData(reqId, rank, contractDetails, distance, benchmark, projection, comboKey);
                }
            }

            eWrapper.scannerDataEnd(reqId);
        }

        private void ReceiveFAEvent()
        {
            _ = ReadInt(); //msgVersion
            var faDataType = ReadInt();
            var faData = ReadString();
            eWrapper.receiveFA(faDataType, faData);
        }

        private void ReceiveFAEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.ReceiveFA receiveFAProto = protobuf.ReceiveFA.Parser.ParseFrom(byteArray);

            eWrapper.receiveFAProtoBuf(receiveFAProto);

            int faDataType = receiveFAProto.HasFaDataType ? receiveFAProto.FaDataType : 0;
            string xml = receiveFAProto.HasXml ? receiveFAProto.Xml : "";

            eWrapper.receiveFA(faDataType, xml);
        }

        private void PositionMultiEvent()
        {
            _ = ReadInt(); //msgVersion
            var requestId = ReadInt();
            var account = ReadString();
            var contract = new Contract
            {
                ConId = ReadInt(),
                Symbol = ReadString(),
                SecType = ReadString(),
                LastTradeDateOrContractMonth = ReadString(),
                Strike = ReadDouble(),
                Right = ReadString(),
                Multiplier = ReadString(),
                Exchange = ReadString(),
                Currency = ReadString(),
                LocalSymbol = ReadString(),
                TradingClass = ReadString()
            };
            var pos = ReadDecimal();
            var avgCost = ReadDouble();
            var modelCode = ReadString();
            eWrapper.positionMulti(requestId, account, modelCode, contract, pos, avgCost);
        }

        private void PositionMultiEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.PositionMulti positionMultiProto = protobuf.PositionMulti.Parser.ParseFrom(byteArray);

            eWrapper.positionMultiProtoBuf(positionMultiProto);

            int reqId = positionMultiProto.HasReqId ? positionMultiProto.ReqId : IncomingMessage.NotValid;
            string account = positionMultiProto.HasAccount ? positionMultiProto.Account : "";
            string modelCode = positionMultiProto.HasModelCode ? positionMultiProto.ModelCode : "";

            if (positionMultiProto.Contract == null)
            {
                return;
            }

            Contract contract = EDecoderUtils.decodeContract(positionMultiProto.Contract);
            Decimal position = positionMultiProto.HasPosition ? Util.StringToDecimal(positionMultiProto.Position) : Decimal.MaxValue;
            double avgCost = positionMultiProto.HasAvgCost ? positionMultiProto.AvgCost : 0;

            eWrapper.positionMulti(reqId, account, modelCode, contract, position, avgCost);
        }

        private void PositionMultiEndEvent()
        {
            _ = ReadInt(); //msgVersion
            var requestId = ReadInt();
            eWrapper.positionMultiEnd(requestId);
        }

        private void PositionMultiEndEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.PositionMultiEnd positionMultiEndProto = protobuf.PositionMultiEnd.Parser.ParseFrom(byteArray);

            eWrapper.positionMultiEndProtoBuf(positionMultiEndProto);

            int reqId = positionMultiEndProto.HasReqId ? positionMultiEndProto.ReqId : IncomingMessage.NotValid;

            eWrapper.positionMultiEnd(reqId);
        }

        private void AccountUpdateMultiEvent()
        {
            _ = ReadInt(); //msgVersion
            var requestId = ReadInt();
            var account = ReadString();
            var modelCode = ReadString();
            var key = ReadString();
            var value = ReadString();
            var currency = ReadString();
            eWrapper.accountUpdateMulti(requestId, account, modelCode, key, value, currency);
        }

        private void AccountUpdateMultiEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.AccountUpdateMulti accountUpdateMultiProto = protobuf.AccountUpdateMulti.Parser.ParseFrom(byteArray);

            eWrapper.accountUpdateMultiProtoBuf(accountUpdateMultiProto);

            int reqId = accountUpdateMultiProto.HasReqId ? accountUpdateMultiProto.ReqId : IncomingMessage.NotValid;
            string account = accountUpdateMultiProto.HasAccount ? accountUpdateMultiProto.Account : "";
            string modelCode = accountUpdateMultiProto.HasModelCode ? accountUpdateMultiProto.ModelCode : "";
            string key = accountUpdateMultiProto.HasKey ? accountUpdateMultiProto.Key : "";
            string value = accountUpdateMultiProto.HasValue ? accountUpdateMultiProto.Value : "";
            string currency = accountUpdateMultiProto.HasCurrency ? accountUpdateMultiProto.Currency : "";

            eWrapper.accountUpdateMulti(reqId, account, modelCode, key, value, currency);
        }

        private void AccountUpdateMultiEndEvent()
        {
            _ = ReadInt(); //msgVersion
            var requestId = ReadInt();
            eWrapper.accountUpdateMultiEnd(requestId);
        }

        private void AccountUpdateMultiEndEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.AccountUpdateMultiEnd accountUpdateMultiEndProto = protobuf.AccountUpdateMultiEnd.Parser.ParseFrom(byteArray);

            eWrapper.accountUpdateMultiEndProtoBuf(accountUpdateMultiEndProto);

            int reqId = accountUpdateMultiEndProto.HasReqId ? accountUpdateMultiEndProto.ReqId : IncomingMessage.NotValid;

            eWrapper.accountUpdateMultiEnd(reqId);
        }

        private void ReplaceFAEndEvent()
        {
            var reqId = ReadInt();
            var text = ReadString();
            eWrapper.replaceFAEnd(reqId, text);
        }

        private void ReplaceFAEndEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.ReplaceFAEnd replaceFAEndProto = protobuf.ReplaceFAEnd.Parser.ParseFrom(byteArray);

            eWrapper.replaceFAEndProtoBuf(replaceFAEndProto);

            int reqId = replaceFAEndProto.HasReqId ? replaceFAEndProto.ReqId : IncomingMessage.NotValid;
            string text = replaceFAEndProto.HasText ? replaceFAEndProto.Text : "";

            eWrapper.replaceFAEnd(reqId, text);
        }

        private void ProcessWshMetaData()
        {
            var reqId = ReadInt();
            var dataJson = ReadString();

            eWrapper.wshMetaData(reqId, dataJson);
        }

        private void WshMetaDataEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.WshMetaData wshMetaDataProto = protobuf.WshMetaData.Parser.ParseFrom(byteArray);

            eWrapper.wshMetaDataProtoBuf(wshMetaDataProto);

            int reqId = wshMetaDataProto.HasReqId ? wshMetaDataProto.ReqId : IncomingMessage.NotValid;
            string dataJson = wshMetaDataProto.HasDataJson ? wshMetaDataProto.DataJson : "";

            eWrapper.wshMetaData(reqId, dataJson);
        }

        private void ProcessWshEventData()
        {
            var reqId = ReadInt();
            var dataJson = ReadString();
            eWrapper.wshEventData(reqId, dataJson);
        }

        private void WshEventDataEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.WshEventData wshEventDataProto = protobuf.WshEventData.Parser.ParseFrom(byteArray);

            eWrapper.wshEventDataProtoBuf(wshEventDataProto);

            int reqId = wshEventDataProto.HasReqId ? wshEventDataProto.ReqId : IncomingMessage.NotValid;
            string dataJson = wshEventDataProto.HasDataJson ? wshEventDataProto.DataJson : "";

            eWrapper.wshEventData(reqId, dataJson);
        }

        private void ProcessHistoricalScheduleEvent()
        {
            var reqId = ReadInt();
            var startDateTime = ReadString();
            var endDateTime = ReadString();
            var timeZone = ReadString();

            var sessionsCount = ReadInt();
            var sessions = new HistoricalSession[sessionsCount];

            for (var i = 0; i < sessionsCount; i++)
            {
                var sessionStartDateTime = ReadString();
                var sessionEndDateTime = ReadString();
                var sessionRefDate = ReadString();

                sessions[i] = new HistoricalSession(sessionStartDateTime, sessionEndDateTime, sessionRefDate);
            }

            eWrapper.historicalSchedule(reqId, startDateTime, endDateTime, timeZone, sessions);
        }

        private void HistoricalScheduleEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.HistoricalSchedule historicalScheduleProto = protobuf.HistoricalSchedule.Parser.ParseFrom(byteArray);

            eWrapper.historicalScheduleProtoBuf(historicalScheduleProto);

            int reqId = historicalScheduleProto.HasReqId ? historicalScheduleProto.ReqId : IncomingMessage.NotValid;
            string startDateTime = historicalScheduleProto.HasStartDateTime ? historicalScheduleProto.StartDateTime : "";
            string endDateTime = historicalScheduleProto.HasEndDateTime ? historicalScheduleProto.EndDateTime : "";
            string timeZone = historicalScheduleProto.HasTimeZone ? historicalScheduleProto.TimeZone : "";

            List<HistoricalSession> sessions = new List<HistoricalSession>();
            if (historicalScheduleProto.HistoricalSessions != null && historicalScheduleProto.HistoricalSessions.Count > 0)
            {
                foreach (protobuf.HistoricalSession historicalSessionProto in historicalScheduleProto.HistoricalSessions)
                {
                    string sessionStartDateTime = historicalSessionProto.HasStartDateTime ? historicalSessionProto.StartDateTime : "";
                    string sessionEndDateTime = historicalSessionProto.HasEndDateTime ? historicalSessionProto.EndDateTime : "";
                    string sessionRefDate = historicalSessionProto.HasRefDate ? historicalSessionProto.RefDate : "";
                    sessions.Add(new HistoricalSession(sessionStartDateTime, sessionEndDateTime, sessionRefDate));
                }
            }

            eWrapper.historicalSchedule(reqId, startDateTime, endDateTime, timeZone, sessions.ToArray());
        }

        private void ProcessUserInfoEvent()
        {
            var reqId = ReadInt();
            var whiteBrandingId = ReadString();

            eWrapper.userInfo(reqId, whiteBrandingId);
        }

        private void UserInfoEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.UserInfo userInfoProto = protobuf.UserInfo.Parser.ParseFrom(byteArray);

            eWrapper.userInfoProtoBuf(userInfoProto);

            int reqId = userInfoProto.HasReqId ? userInfoProto.ReqId : IncomingMessage.NotValid;
            string whiteBrandingId = userInfoProto.HasWhiteBrandingId ? userInfoProto.WhiteBrandingId : "";

            eWrapper.userInfo(reqId, whiteBrandingId);
        }

        private void ProcessCurrentTimeInMillisEvent()
        {
            var timeInMillis = ReadLong();
            eWrapper.currentTimeInMillis(timeInMillis);
        }

        private void CurrentTimeInMillisEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.CurrentTimeInMillis currentTimeInMillisProto = protobuf.CurrentTimeInMillis.Parser.ParseFrom(byteArray);

            eWrapper.currentTimeInMillisProtoBuf(currentTimeInMillisProto);

            long timeInMillis = currentTimeInMillisProto.HasCurrentTimeInMillis_ ? currentTimeInMillisProto.CurrentTimeInMillis_ : 0;

            eWrapper.currentTimeInMillis(timeInMillis);
        }

        private void ConfigResponseEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.ConfigResponse configResponseProto = protobuf.ConfigResponse.Parser.ParseFrom(byteArray);

            eWrapper.configResponseProtoBuf(configResponseProto);
        }

        private void UpdateConfigResponseEventProtoBuf(int len)
        {
            byte[] byteArray = ReadByteArray(len);
            protobuf.UpdateConfigResponse updateConfigResponseProto = protobuf.UpdateConfigResponse.Parser.ParseFrom(byteArray);

            eWrapper.updateConfigResponseProtoBuf(updateConfigResponseProto);
        }

        public double ReadDouble()
        {
            var doubleAsstring = ReadString();
            if (string.IsNullOrEmpty(doubleAsstring) || doubleAsstring == "0") return 0;
            return double.Parse(doubleAsstring, System.Globalization.NumberFormatInfo.InvariantInfo);
        }

        public double ReadDoubleMax()
        {
            var str = ReadString();
            return string.IsNullOrEmpty(str) ? double.MaxValue : str == Constants.INFINITY_STR ? double.PositiveInfinity : double.Parse(str, System.Globalization.NumberFormatInfo.InvariantInfo);
        }

        public decimal ReadDecimal()
        {
            var str = ReadString();
            return Util.StringToDecimal(str);
        }

        public long ReadLong()
        {
            var longAsstring = ReadString();
            if (string.IsNullOrEmpty(longAsstring) || longAsstring == "0") return 0;
            return long.Parse(longAsstring);
        }

        public int ReadInt()
        {
            var intAsstring = ReadString();
            if (string.IsNullOrEmpty(intAsstring) ||
                intAsstring == "0")
            {
                return 0;
            }
            return int.Parse(intAsstring);
        }

        public int ReadIntMax()
        {
            var str = ReadString();
            return string.IsNullOrEmpty(str) ? int.MaxValue : int.Parse(str);
        }

        public bool ReadBoolFromInt()
        {
            var str = ReadString();
            return str != null && int.Parse(str) != 0;
        }

        public char ReadChar()
        {
            var str = ReadString();
            return str == null ? '\0' : str[0];
        }

        public string ReadString()
        {
            var b = dataReader.ReadByte();

            nDecodedLen++;

            if (b == 0) return null;
            var strBuilder = new StringBuilder();
            strBuilder.Append((char)b);
            while (true)
            {
                b = dataReader.ReadByte();
                if (b == 0) break;
                strBuilder.Append((char)b);
            }

            nDecodedLen += strBuilder.Length;

            return strBuilder.ToString();
        }

        public byte[] ReadByteArray(int len)
        {
            byte[] byteArray = dataReader.ReadBytes(len);
            nDecodedLen += len;
            return byteArray;
        }

        public int ReadRawInt()
        {
            byte[] bytes = dataReader.ReadBytes(4);
            Array.Reverse(bytes);
            nDecodedLen += 4;
            return BitConverter.ToInt32(bytes, 0);
        }

        private void readLastTradeDate(ContractDetails contract, bool isBond)
        {
            var lastTradeDateOrContractMonth = ReadString();
            EDecoderUtils.SetLastTradeDate(lastTradeDateOrContractMonth, contract, isBond);
        }

    }
}
