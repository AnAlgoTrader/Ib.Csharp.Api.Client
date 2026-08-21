/* Copyright (C) 2025 Interactive Brokers LLC. All rights reserved. This code is subject to the terms
 * and conditions of the IB API Non-Commercial License or the IB API Commercial License, as applicable. */

using System;
using System.Collections.Generic;
using System.Linq;

namespace IBApi
{
    internal class EClientUtils
    {
	    public static protobuf.ExecutionRequest createExecutionRequestProto(int reqId, ExecutionFilter filter)
        {
            protobuf.ExecutionFilter executionFilterProto = new protobuf.ExecutionFilter();
            if (Util.IsValidValue(filter.ClientId)) executionFilterProto.ClientId = filter.ClientId;
            if (!Util.StringIsEmpty(filter.AcctCode)) executionFilterProto.AcctCode = filter.AcctCode;
            if (!Util.StringIsEmpty(filter.Time)) executionFilterProto.Time = filter.Time;
            if (!Util.StringIsEmpty(filter.Symbol)) executionFilterProto.Symbol = filter.Symbol;
            if (!Util.StringIsEmpty(filter.SecType)) executionFilterProto.SecType = filter.SecType;
            if (!Util.StringIsEmpty(filter.Exchange)) executionFilterProto.Exchange = filter.Exchange;
            if (!Util.StringIsEmpty(filter.Side)) executionFilterProto.Side = filter.Side;
            if (Util.IsValidValue(filter.LastNDays)) executionFilterProto.LastNDays = filter.LastNDays;
            if (filter.SpecificDates != null && filter.SpecificDates.Any()) executionFilterProto.SpecificDates.AddRange(filter.SpecificDates);

            protobuf.ExecutionRequest executionRequestProto = new protobuf.ExecutionRequest();
            if (Util.IsValidValue(reqId)) executionRequestProto.ReqId = reqId;
            executionRequestProto.ExecutionFilter = executionFilterProto;
                 
            return executionRequestProto;
        }

        public static protobuf.PlaceOrderRequest createPlaceOrderRequestProto(int orderId, Contract contract, Order order) 
        {
            protobuf.PlaceOrderRequest placeOrderRequestProto = new protobuf.PlaceOrderRequest();
            if (Util.IsValidValue(orderId)) placeOrderRequestProto.OrderId = orderId;

            protobuf.Contract contractProto = createContractProto(contract, order);
            if (contractProto != null) placeOrderRequestProto.Contract = contractProto;

            protobuf.Order orderProto = createOrderProto(order);
            if (orderProto != null) placeOrderRequestProto.Order = orderProto;

            protobuf.AttachedOrders attachedOrdersProto = createAttachedOrdersProto(order);
            if (attachedOrdersProto != null) placeOrderRequestProto.AttachedOrders = attachedOrdersProto;

            return placeOrderRequestProto;
        }

        public static protobuf.AttachedOrders createAttachedOrdersProto(Order order)
        {
            protobuf.AttachedOrders attachedOrdersProto = new protobuf.AttachedOrders();
            if (Util.IsValidValue(order.SlOrderId)) attachedOrdersProto.SlOrderId = order.SlOrderId;
            if (!Util.StringIsEmpty(order.SlOrderType)) attachedOrdersProto.SlOrderType = order.SlOrderType;
            if (Util.IsValidValue(order.PtOrderId)) attachedOrdersProto.PtOrderId = order.PtOrderId;
            if (!Util.StringIsEmpty(order.PtOrderType)) attachedOrdersProto.PtOrderType = order.PtOrderType;
            return attachedOrdersProto;
        }

        public static protobuf.Order createOrderProto(Order order) 
        {
            protobuf.Order orderProto = new protobuf.Order();
            if (Util.IsValidValue(order.ClientId)) orderProto.ClientId = order.ClientId;
            if (Util.IsValidValue(order.PermId)) orderProto.PermId = order.PermId;
            if (Util.IsValidValue(order.ParentId)) orderProto.ParentId = order.ParentId;
            if (!Util.StringIsEmpty(order.Action)) orderProto.Action = order.Action;
            if (Util.IsValidValue(order.TotalQuantity)) orderProto.TotalQuantity = order.TotalQuantity.ToString();
            if (Util.IsValidValue(order.DisplaySize)) orderProto.DisplaySize = order.DisplaySize;
            if (!Util.StringIsEmpty(order.OrderType)) orderProto.OrderType = order.OrderType;
            if (Util.IsValidValue(order.LmtPrice)) orderProto.LmtPrice = order.LmtPrice;
            if (Util.IsValidValue(order.AuxPrice)) orderProto.AuxPrice = order.AuxPrice;
            if (!Util.StringIsEmpty(order.Tif)) orderProto.Tif = order.Tif;
            if (!Util.StringIsEmpty(order.Account)) orderProto.Account = order.Account;
            if (!Util.StringIsEmpty(order.SettlingFirm)) orderProto.SettlingFirm = order.SettlingFirm;
            if (!Util.StringIsEmpty(order.ClearingAccount)) orderProto.ClearingAccount = order.ClearingAccount;
            if (!Util.StringIsEmpty(order.ClearingIntent)) orderProto.ClearingIntent = order.ClearingIntent;
            if (order.AllOrNone) orderProto.AllOrNone = order.AllOrNone;
            if (order.BlockOrder) orderProto.BlockOrder = order.BlockOrder;
            if (order.Hidden) orderProto.Hidden = order.Hidden;
            if (order.OutsideRth) orderProto.OutsideRth = order.OutsideRth;
            if (order.SweepToFill) orderProto.SweepToFill = order.SweepToFill;
            if (Util.IsValidValue(order.PercentOffset)) orderProto.PercentOffset = order.PercentOffset;
            if (Util.IsValidValue(order.TrailingPercent)) orderProto.TrailingPercent = order.TrailingPercent;
            if (Util.IsValidValue(order.TrailStopPrice)) orderProto.TrailStopPrice = order.TrailStopPrice;
            if (Util.IsValidValue(order.MinQty)) orderProto.MinQty = order.MinQty;
            if (!Util.StringIsEmpty(order.GoodAfterTime)) orderProto.GoodAfterTime = order.GoodAfterTime;
            if (!Util.StringIsEmpty(order.GoodTillDate)) orderProto.GoodTillDate = order.GoodTillDate;
            if (!Util.StringIsEmpty(order.OcaGroup)) orderProto.OcaGroup = order.OcaGroup;
            if (!Util.StringIsEmpty(order.OrderRef)) orderProto.OrderRef = order.OrderRef;
            if (!Util.StringIsEmpty(order.Rule80A)) orderProto.Rule80A = order.Rule80A;
            if (Util.IsValidValue(order.OcaType)) orderProto.OcaType = order.OcaType;
            if (Util.IsValidValue(order.TriggerMethod)) orderProto.TriggerMethod = order.TriggerMethod;
            if (!Util.StringIsEmpty(order.ActiveStartTime)) orderProto.ActiveStartTime = order.ActiveStartTime;
            if (!Util.StringIsEmpty(order.ActiveStopTime)) orderProto.ActiveStopTime = order.ActiveStopTime;
            if (!Util.StringIsEmpty(order.FaGroup)) orderProto.FaGroup = order.FaGroup;
            if (!Util.StringIsEmpty(order.FaMethod)) orderProto.FaMethod = order.FaMethod;
            if (!Util.StringIsEmpty(order.FaPercentage)) orderProto.FaPercentage = order.FaPercentage;
            if (Util.IsValidValue(order.Volatility)) orderProto.Volatility = order.Volatility;
            if (Util.IsValidValue(order.VolatilityType)) orderProto.VolatilityType = order.VolatilityType;
            if (Util.IsValidValue(order.ContinuousUpdate)) orderProto.ContinuousUpdate = order.ContinuousUpdate == 1;
            if (Util.IsValidValue(order.ReferencePriceType)) orderProto.ReferencePriceType = order.ReferencePriceType;
            if (!Util.StringIsEmpty(order.DeltaNeutralOrderType)) orderProto.DeltaNeutralOrderType = order.DeltaNeutralOrderType;
            if (Util.IsValidValue(order.DeltaNeutralAuxPrice)) orderProto.DeltaNeutralAuxPrice = order.DeltaNeutralAuxPrice;
            if (Util.IsValidValue(order.DeltaNeutralConId)) orderProto.DeltaNeutralConId = order.DeltaNeutralConId;
            if (!Util.StringIsEmpty(order.DeltaNeutralOpenClose)) orderProto.DeltaNeutralOpenClose = order.DeltaNeutralOpenClose;
            if (order.DeltaNeutralShortSale) orderProto.DeltaNeutralShortSale = order.DeltaNeutralShortSale;
            if (Util.IsValidValue(order.DeltaNeutralShortSaleSlot)) orderProto.DeltaNeutralShortSaleSlot = order.DeltaNeutralShortSaleSlot;
            if (!Util.StringIsEmpty(order.DeltaNeutralDesignatedLocation)) orderProto.DeltaNeutralDesignatedLocation = order.DeltaNeutralDesignatedLocation;
            if (Util.IsValidValue(order.ScaleInitLevelSize)) orderProto.ScaleInitLevelSize = order.ScaleInitLevelSize;
            if (Util.IsValidValue(order.ScaleSubsLevelSize)) orderProto.ScaleSubsLevelSize = order.ScaleSubsLevelSize;
            if (Util.IsValidValue(order.ScalePriceIncrement)) orderProto.ScalePriceIncrement = order.ScalePriceIncrement;
            if (Util.IsValidValue(order.ScalePriceAdjustValue)) orderProto.ScalePriceAdjustValue = order.ScalePriceAdjustValue;
            if (Util.IsValidValue(order.ScalePriceAdjustInterval)) orderProto.ScalePriceAdjustInterval = order.ScalePriceAdjustInterval;
            if (Util.IsValidValue(order.ScaleProfitOffset)) orderProto.ScaleProfitOffset = order.ScaleProfitOffset;
            if (order.ScaleAutoReset) orderProto.ScaleAutoReset = order.ScaleAutoReset;
            if (Util.IsValidValue(order.ScaleInitPosition)) orderProto.ScaleInitPosition = order.ScaleInitPosition;
            if (Util.IsValidValue(order.ScaleInitFillQty)) orderProto.ScaleInitFillQty = order.ScaleInitFillQty;
            if (order.ScaleRandomPercent) orderProto.ScaleRandomPercent = order.ScaleRandomPercent;
            if (!Util.StringIsEmpty(order.ScaleTable)) orderProto.ScaleTable = order.ScaleTable;
            if (!Util.StringIsEmpty(order.HedgeType)) orderProto.HedgeType = order.HedgeType;
            if (!Util.StringIsEmpty(order.HedgeParam)) orderProto.HedgeParam = order.HedgeParam;
            if (Util.IsValidValue(order.HedgeMaxSize)) orderProto.HedgeMaxSize = order.HedgeMaxSize;

	        if (!Util.StringIsEmpty(order.AlgoStrategy)) orderProto.AlgoStrategy = order.AlgoStrategy;
            if (order.AlgoParams != null && order.AlgoParams.Any())
            {
                Dictionary<string, string> algoParams = order.AlgoParams.ToDictionary(tagValue => tagValue.Tag, tagValue => tagValue.Value);
                orderProto.AlgoParams.Add(algoParams);
            }
            if (!Util.StringIsEmpty(order.AlgoId)) orderProto.AlgoId = order.AlgoId;

            if (order.SmartComboRoutingParams != null && order.SmartComboRoutingParams.Any())
            {
                Dictionary<string, string> smartComboRoutingParams = order.SmartComboRoutingParams.ToDictionary(tagValue => tagValue.Tag, tagValue => tagValue.Value);
                orderProto.SmartComboRoutingParams.Add(smartComboRoutingParams);
            }

            if (order.WhatIf) orderProto.WhatIf = order.WhatIf;
            if (order.Transmit) orderProto.Transmit = order.Transmit;
            if (order.OverridePercentageConstraints) orderProto.OverridePercentageConstraints = order.OverridePercentageConstraints;
            if (!Util.StringIsEmpty(order.OpenClose)) orderProto.OpenClose = order.OpenClose;
            if (Util.IsValidValue(order.Origin)) orderProto.Origin = order.Origin;
            if (Util.IsValidValue(order.ShortSaleSlot)) orderProto.ShortSaleSlot = order.ShortSaleSlot;
            if (!Util.StringIsEmpty(order.DesignatedLocation)) orderProto.DesignatedLocation = order.DesignatedLocation;
            if (Util.IsValidValue(order.ExemptCode)) orderProto.ExemptCode = order.ExemptCode;
            if (!Util.StringIsEmpty(order.DeltaNeutralSettlingFirm)) orderProto.DeltaNeutralSettlingFirm = order.DeltaNeutralSettlingFirm;
            if (!Util.StringIsEmpty(order.DeltaNeutralClearingAccount)) orderProto.DeltaNeutralClearingAccount = order.DeltaNeutralClearingAccount;
            if (!Util.StringIsEmpty(order.DeltaNeutralClearingIntent)) orderProto.DeltaNeutralClearingIntent = order.DeltaNeutralClearingIntent;
            if (Util.IsValidValue(order.DiscretionaryAmt)) orderProto.DiscretionaryAmt = order.DiscretionaryAmt;
            if (order.OptOutSmartRouting) orderProto.OptOutSmartRouting = order.OptOutSmartRouting;
            if (Util.IsValidValue(order.ExemptCode)) orderProto.ExemptCode = order.ExemptCode;
            if (Util.IsValidValue(order.StartingPrice)) orderProto.StartingPrice = order.StartingPrice;
            if (Util.IsValidValue(order.StockRefPrice)) orderProto.StockRefPrice = order.StockRefPrice;
            if (Util.IsValidValue(order.Delta)) orderProto.Delta = order.Delta;
            if (Util.IsValidValue(order.StockRangeLower)) orderProto.StockRangeLower = order.StockRangeLower;
            if (Util.IsValidValue(order.StockRangeUpper)) orderProto.StockRangeUpper = order.StockRangeUpper;
            if (order.NotHeld) orderProto.NotHeld = order.NotHeld;

            if (order.OrderMiscOptions != null && order.OrderMiscOptions.Any())
            {
                Dictionary<string, string> orderMiscOptions = order.OrderMiscOptions.ToDictionary(tagValue => tagValue.Tag, tagValue => tagValue.Value);
                orderProto.OrderMiscOptions.Add(orderMiscOptions);
            }

            if (order.Solicited) orderProto.Solicited = order.Solicited;
            if (order.RandomizeSize) orderProto.RandomizeSize = order.RandomizeSize;
            if (order.RandomizePrice) orderProto.RandomizePrice = order.RandomizePrice;
            if (Util.IsValidValue(order.ReferenceContractId)) orderProto.ReferenceContractId = order.ReferenceContractId;
            if (Util.IsValidValue(order.PeggedChangeAmount)) orderProto.PeggedChangeAmount = order.PeggedChangeAmount;
            if (order.IsPeggedChangeAmountDecrease) orderProto.IsPeggedChangeAmountDecrease = order.IsPeggedChangeAmountDecrease;
            if (Util.IsValidValue(order.ReferenceChangeAmount)) orderProto.ReferenceChangeAmount = order.ReferenceChangeAmount;
            if (!Util.StringIsEmpty(order.ReferenceExchange)) orderProto.ReferenceExchangeId = order.ReferenceExchange;

            if (order.AdjustedOrderType != null && !Util.StringIsEmpty(order.AdjustedOrderType)) orderProto.AdjustedOrderType = order.AdjustedOrderType;
            if (Util.IsValidValue(order.TriggerPrice)) orderProto.TriggerPrice = order.TriggerPrice;
            if (Util.IsValidValue(order.AdjustedStopPrice)) orderProto.AdjustedStopPrice = order.AdjustedStopPrice;
            if (Util.IsValidValue(order.AdjustedStopLimitPrice)) orderProto.AdjustedStopLimitPrice = order.AdjustedStopLimitPrice;
            if (Util.IsValidValue(order.AdjustedTrailingAmount)) orderProto.AdjustedTrailingAmount = order.AdjustedTrailingAmount;
            if (Util.IsValidValue(order.AdjustableTrailingUnit)) orderProto.AdjustableTrailingUnit = order.AdjustableTrailingUnit;
            if (Util.IsValidValue(order.LmtPriceOffset)) orderProto.LmtPriceOffset = order.LmtPriceOffset;

            List<protobuf.OrderCondition> orderConditionList = createConditionsProto(order);
            if (orderConditionList != null && orderConditionList.Any()) orderProto.Conditions.Add(orderConditionList);
            if (order.ConditionsCancelOrder) orderProto.ConditionsCancelOrder = order.ConditionsCancelOrder;
            if (order.ConditionsIgnoreRth) orderProto.ConditionsIgnoreRth = order.ConditionsIgnoreRth;

            if (!Util.StringIsEmpty(order.ModelCode)) orderProto.ModelCode = order.ModelCode;
            if (!Util.StringIsEmpty(order.ExtOperator)) orderProto.ExtOperator = order.ExtOperator;

    	    protobuf.SoftDollarTier softDollarTier = createSoftDollarTierProto(order);
            if (softDollarTier != null) orderProto.SoftDollarTier = softDollarTier;

            if (Util.IsValidValue(order.CashQty)) orderProto.CashQty = order.CashQty;
            if (!Util.StringIsEmpty(order.Mifid2DecisionMaker)) orderProto.Mifid2DecisionMaker = order.Mifid2DecisionMaker;
            if (!Util.StringIsEmpty(order.Mifid2DecisionAlgo)) orderProto.Mifid2DecisionAlgo = order.Mifid2DecisionAlgo;
            if (!Util.StringIsEmpty(order.Mifid2ExecutionTrader)) orderProto.Mifid2ExecutionTrader = order.Mifid2ExecutionTrader;
            if (!Util.StringIsEmpty(order.Mifid2ExecutionAlgo)) orderProto.Mifid2ExecutionAlgo = order.Mifid2ExecutionAlgo;
            if (order.DontUseAutoPriceForHedge) orderProto.DontUseAutoPriceForHedge = order.DontUseAutoPriceForHedge;
            if (order.IsOmsContainer) orderProto.IsOmsContainer = order.IsOmsContainer;
            if (order.DiscretionaryUpToLimitPrice) orderProto.DiscretionaryUpToLimitPrice = order.DiscretionaryUpToLimitPrice;
            if (order.UsePriceMgmtAlgo.HasValue) orderProto.UsePriceMgmtAlgo = order.UsePriceMgmtAlgo.Value ? 1 : 0;
            if (Util.IsValidValue(order.Duration)) orderProto.Duration = order.Duration;
            if (Util.IsValidValue(order.PostToAts)) orderProto.PostToAts = order.PostToAts;
            if (!Util.StringIsEmpty(order.AdvancedErrorOverride)) orderProto.AdvancedErrorOverride = order.AdvancedErrorOverride;
            if (!Util.StringIsEmpty(order.ManualOrderTime)) orderProto.ManualOrderTime = order.ManualOrderTime;
            if (Util.IsValidValue(order.MinTradeQty)) orderProto.MinTradeQty = order.MinTradeQty;
            if (Util.IsValidValue(order.MinCompeteSize)) orderProto.MinCompeteSize = order.MinCompeteSize;
            if (Util.IsValidValue(order.CompeteAgainstBestOffset)) orderProto.CompeteAgainstBestOffset = order.CompeteAgainstBestOffset;
            if (Util.IsValidValue(order.MidOffsetAtWhole)) orderProto.MidOffsetAtWhole = order.MidOffsetAtWhole;
            if (Util.IsValidValue(order.MidOffsetAtHalf)) orderProto.MidOffsetAtHalf = order.MidOffsetAtHalf;
            if (!Util.StringIsEmpty(order.CustomerAccount)) orderProto.CustomerAccount = order.CustomerAccount;
            if (order.ProfessionalCustomer) orderProto.ProfessionalCustomer = order.ProfessionalCustomer;
            if (!Util.StringIsEmpty(order.BondAccruedInterest)) orderProto.BondAccruedInterest = order.BondAccruedInterest;
            if (order.IncludeOvernight) orderProto.IncludeOvernight = order.IncludeOvernight;
            if (Util.IsValidValue(order.ManualOrderIndicator)) orderProto.ManualOrderIndicator = order.ManualOrderIndicator;
            if (!Util.StringIsEmpty(order.Submitter)) orderProto.Submitter = order.Submitter;
            if (order.AutoCancelParent) orderProto.AutoCancelParent = order.AutoCancelParent;
            if (order.ImbalanceOnly) orderProto.ImbalanceOnly = order.ImbalanceOnly;
            if (order.PostOnly) orderProto.PostOnly = order.PostOnly;
            if (order.AllowPreOpen) orderProto.AllowPreOpen = order.AllowPreOpen;
            if (order.IgnoreOpenAuction) orderProto.IgnoreOpenAuction = order.IgnoreOpenAuction;
            if (order.Deactivate) orderProto.Deactivate = order.Deactivate;
            if (order.SeekPriceImprovement.HasValue) orderProto.SeekPriceImprovement = order.SeekPriceImprovement.Value ? 1 : 0;
            if (Util.IsValidValue(order.WhatIfType)) orderProto.WhatIfType = order.WhatIfType;
            if (order.RouteMarketableToBbo.HasValue) orderProto.RouteMarketableToBbo = order.RouteMarketableToBbo.Value ? 1 : 0;

            return orderProto;
        }

        public static List<protobuf.OrderCondition> createConditionsProto(Order order) 
        {
            List<protobuf.OrderCondition> orderConditionList = new List<protobuf.OrderCondition>();
            try
            {
                if (order.Conditions != null && order.Conditions.Count > 0) {
                    foreach (OrderCondition condition in order.Conditions) {
                        OrderConditionType type = condition.Type;
                        protobuf.OrderCondition orderConditionProto = null;
                        switch (type) {
                            case OrderConditionType.Price:
                                orderConditionProto = createPriceConditionProto(condition);
                                break;
                            case OrderConditionType.Time:
                                orderConditionProto = createTimeConditionProto(condition);
                                break;
                            case OrderConditionType.Margin:
                                orderConditionProto = createMarginConditionProto(condition);
                                break;
                            case OrderConditionType.Execution:
                                orderConditionProto = createExecutionConditionProto(condition);
                                break;
                            case OrderConditionType.Volume:
                                orderConditionProto = createVolumeConditionProto(condition);
                                break;
                            case OrderConditionType.PercentChange:
                                orderConditionProto = createPercentChangeConditionProto(condition);
                                break;
                        }
                        if (orderConditionProto != null) {
                            orderConditionList.Add(orderConditionProto);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw new EClientException(EClientErrors.ERROR_ENCODING_PROTOBUF, "Error encoding conditions");
            }

            return orderConditionList;
        }

        private static protobuf.OrderCondition createOrderConditionProto(OrderCondition condition) 
        {
            int type = (int)condition.Type;
            bool isConjunctionConnection = condition.IsConjunctionConnection;
            protobuf.OrderCondition orderConditionProto = new protobuf.OrderCondition();
            if (Util.IsValidValue(type)) orderConditionProto.Type = type;
            orderConditionProto.IsConjunctionConnection = isConjunctionConnection;
            return orderConditionProto;
        }

        private static protobuf.OrderCondition createOperatorConditionProto(OrderCondition condition)
        {
            protobuf.OrderCondition orderConditionProto = createOrderConditionProto(condition);
            OperatorCondition operatorCondition = (OperatorCondition)condition; 
            bool isMore = operatorCondition.IsMore;
            protobuf.OrderCondition operatorConditionProto = new protobuf.OrderCondition();
            operatorConditionProto.MergeFrom(orderConditionProto);
            operatorConditionProto.IsMore = isMore;
            return operatorConditionProto;
        }

        private static protobuf.OrderCondition createContractConditionProto(OrderCondition condition) 
        {
            protobuf.OrderCondition orderConditionProto = createOperatorConditionProto(condition);
            ContractCondition contractCondition = (ContractCondition)condition; 
            int conId = contractCondition.ConId;
            string exchange = contractCondition.Exchange;
            protobuf.OrderCondition contractConditionProto = new protobuf.OrderCondition();
            contractConditionProto.MergeFrom(orderConditionProto);
            if (Util.IsValidValue(conId)) contractConditionProto.ConId = conId;
            if (!Util.StringIsEmpty(exchange)) contractConditionProto.Exchange = exchange;
            return contractConditionProto;
        }

        private static protobuf.OrderCondition createPriceConditionProto(OrderCondition condition) 
        {
            protobuf.OrderCondition orderConditionProto = createContractConditionProto(condition);
            PriceCondition priceCondition = (PriceCondition)condition; 
            double price = priceCondition.Price;
            int triggerMethod = (int)priceCondition.TriggerMethod;
            protobuf.OrderCondition priceConditionProto = new protobuf.OrderCondition();
            priceConditionProto.MergeFrom(orderConditionProto);
            if (Util.IsValidValue(price)) priceConditionProto.Price = price;
            if (Util.IsValidValue(triggerMethod)) priceConditionProto.TriggerMethod = triggerMethod;
            return priceConditionProto;
        }

        private static protobuf.OrderCondition createTimeConditionProto(OrderCondition condition) 
        {
            protobuf.OrderCondition operatorConditionProto = createOperatorConditionProto(condition);
            TimeCondition timeCondition = (TimeCondition)condition; 
            string time = timeCondition.Time;
            protobuf.OrderCondition timeConditionProto = new protobuf.OrderCondition();
            timeConditionProto.MergeFrom(operatorConditionProto);
            if (!Util.StringIsEmpty(time)) timeConditionProto.Time = time;
            return timeConditionProto;
        }

        private static protobuf.OrderCondition createMarginConditionProto(OrderCondition condition)
        {
            protobuf.OrderCondition operatorConditionProto = createOperatorConditionProto(condition);
            MarginCondition marginCondition = (MarginCondition)condition; 
            int percent = marginCondition.Percent;
            protobuf.OrderCondition marginConditionProto = new protobuf.OrderCondition();
            marginConditionProto.MergeFrom(operatorConditionProto);
            if (Util.IsValidValue(percent)) marginConditionProto.Percent = percent;
            return marginConditionProto;
        }

        private static protobuf.OrderCondition createExecutionConditionProto(OrderCondition condition) 
        {
            protobuf.OrderCondition orderConditionProto = createOrderConditionProto(condition);
            ExecutionCondition executionCondition = (ExecutionCondition)condition; 
            string secType = executionCondition.SecType;
            string exchange = executionCondition.Exchange;
            string symbol = executionCondition.Symbol;
            protobuf.OrderCondition executionConditionProto = new protobuf.OrderCondition();
            executionConditionProto.MergeFrom(orderConditionProto);
            if (!Util.StringIsEmpty(secType)) executionConditionProto.SecType = secType;
            if (!Util.StringIsEmpty(exchange)) executionConditionProto.Exchange = exchange;
            if (!Util.StringIsEmpty(symbol)) executionConditionProto.Symbol = symbol;
            return executionConditionProto;
        }

        private static protobuf.OrderCondition createVolumeConditionProto(OrderCondition condition) 
        {
            protobuf.OrderCondition orderConditionProto = createContractConditionProto(condition);
            VolumeCondition volumeCondition = (VolumeCondition)condition; 
            int volume = volumeCondition.Volume;
            protobuf.OrderCondition volumeConditionProto = new protobuf.OrderCondition();
            volumeConditionProto.MergeFrom(orderConditionProto);
            if (Util.IsValidValue(volume)) volumeConditionProto.Volume = volume;
            return volumeConditionProto;
        }

        private static protobuf.OrderCondition createPercentChangeConditionProto(OrderCondition condition) 
        {
            protobuf.OrderCondition orderConditionProto = createContractConditionProto(condition);
            PercentChangeCondition percentChangeCondition = (PercentChangeCondition)condition; 
            double changePercent = percentChangeCondition.ChangePercent;
            protobuf.OrderCondition percentChangeConditionProto = new protobuf.OrderCondition();
            percentChangeConditionProto.MergeFrom(orderConditionProto);
            if (Util.IsValidValue(changePercent)) percentChangeConditionProto.ChangePercent = changePercent;
            return percentChangeConditionProto;
        }

        public static protobuf.SoftDollarTier createSoftDollarTierProto(Order order) 
        {
            SoftDollarTier tier = order.Tier;
            if (tier == null) {
                return null;
            }

            protobuf.SoftDollarTier softDollarTierProto = new protobuf.SoftDollarTier();
            if (!Util.StringIsEmpty(tier.Name)) softDollarTierProto.Name = tier.Name;
            if (!Util.StringIsEmpty(tier.Value)) softDollarTierProto.Value = tier.Value;
            if (!Util.StringIsEmpty(tier.DisplayName)) softDollarTierProto.DisplayName = tier.DisplayName;
            return softDollarTierProto;
        }

        public static protobuf.Contract createContractProto(Contract contract, Order order) 
        {
            protobuf.Contract contractProto = new protobuf.Contract();
            if (Util.IsValidValue(contract.ConId)) contractProto.ConId = contract.ConId;
            if (!Util.StringIsEmpty(contract.Symbol)) contractProto.Symbol = contract.Symbol;
            if (!Util.StringIsEmpty(contract.SecType)) contractProto.SecType = contract.SecType;
            if (!Util.StringIsEmpty(contract.LastTradeDateOrContractMonth)) contractProto.LastTradeDateOrContractMonth = contract.LastTradeDateOrContractMonth;
            if (Util.IsValidValue(contract.Strike)) contractProto.Strike = contract.Strike;
            if (!Util.StringIsEmpty(contract.Right)) contractProto.Right = contract.Right;
            if (!Util.StringIsEmpty(contract.Multiplier)) contractProto.Multiplier = Util.StringToDoubleMax(contract.Multiplier);
            if (!Util.StringIsEmpty(contract.Exchange)) contractProto.Exchange = contract.Exchange;
            if (!Util.StringIsEmpty(contract.PrimaryExch)) contractProto.PrimaryExch = contract.PrimaryExch;
            if (!Util.StringIsEmpty(contract.Currency)) contractProto.Currency = contract.Currency;
            if (!Util.StringIsEmpty(contract.LocalSymbol)) contractProto.LocalSymbol = contract.LocalSymbol;
            if (!Util.StringIsEmpty(contract.TradingClass)) contractProto.TradingClass = contract.TradingClass;
            if (!Util.StringIsEmpty(contract.SecIdType)) contractProto.SecIdType = contract.SecIdType;
            if (!Util.StringIsEmpty(contract.SecId)) contractProto.SecId = contract.SecId;
            if (contract.IncludeExpired) contractProto.IncludeExpired = contract.IncludeExpired;
            if (!Util.StringIsEmpty(contract.ComboLegsDescription)) contractProto.ComboLegsDescrip = contract.ComboLegsDescription;
            if (!Util.StringIsEmpty(contract.Description)) contractProto.Description = contract.Description;
            if (!Util.StringIsEmpty(contract.IssuerId)) contractProto.IssuerId = contract.IssuerId;

            List<protobuf.ComboLeg> comboLegProtoList = createComboLegProtoList(contract, order);
            if (comboLegProtoList != null) {
                contractProto.ComboLegs.AddRange(comboLegProtoList);
            }
            protobuf.DeltaNeutralContract deltaNeutralContractProto = createDeltaNeutralContractProto(contract);
            if (deltaNeutralContractProto != null) {
               contractProto.DeltaNeutralContract = deltaNeutralContractProto;
            }
            return contractProto;
        }

        public static protobuf.DeltaNeutralContract createDeltaNeutralContractProto(Contract contract) 
        {
            if (contract.DeltaNeutralContract == null) {
                return null;
            }
            DeltaNeutralContract deltaNeutralContract = contract.DeltaNeutralContract;
            protobuf.DeltaNeutralContract deltaNeutralContractProto = new protobuf.DeltaNeutralContract();
            if (Util.IsValidValue(deltaNeutralContract.ConId)) deltaNeutralContractProto.ConId = deltaNeutralContract.ConId;
            if (Util.IsValidValue(deltaNeutralContract.Delta)) deltaNeutralContractProto.Delta = deltaNeutralContract.Delta;
            if (Util.IsValidValue(deltaNeutralContract.Price)) deltaNeutralContractProto.Price = deltaNeutralContract.Price;
            return deltaNeutralContractProto;
        }

        public static List<protobuf.ComboLeg> createComboLegProtoList(Contract contract, Order order) 
        {
            List<ComboLeg> comboLegs = contract.ComboLegs;
            if (comboLegs == null || !comboLegs.Any()) {
                return null;
            }
            List<protobuf.ComboLeg> comboLegProtoList = new List<protobuf.ComboLeg>();
            for(int i = 0; i < comboLegs.Count; i++) {
                ComboLeg comboLeg = comboLegs.ElementAt(i);
                double perLegPrice = double.MaxValue;
                if (order != null && i < order.OrderComboLegs.Count) {
                    perLegPrice = order.OrderComboLegs.ElementAt(i).Price;
                }
                protobuf.ComboLeg comboLegProto = createComboLegProto(comboLeg, perLegPrice);
                comboLegProtoList.Add(comboLegProto);
            }
            return comboLegProtoList;
        }

        public static protobuf.ComboLeg createComboLegProto(ComboLeg comboLeg, double perLegPrice) 
        {
            protobuf.ComboLeg comboLegProto = new protobuf.ComboLeg();
            if (Util.IsValidValue(comboLeg.ConId)) comboLegProto.ConId = comboLeg.ConId;
            if (Util.IsValidValue(comboLeg.Ratio)) comboLegProto.Ratio = comboLeg.Ratio;
            if (!Util.StringIsEmpty(comboLeg.Action)) comboLegProto.Action = comboLeg.Action;
            if (!Util.StringIsEmpty(comboLeg.Exchange)) comboLegProto.Exchange = comboLeg.Exchange;
            if (Util.IsValidValue(comboLeg.OpenClose)) comboLegProto.OpenClose = comboLeg.OpenClose;
            if (Util.IsValidValue(comboLeg.ShortSaleSlot)) comboLegProto.ShortSalesSlot = comboLeg.ShortSaleSlot;
            if (!Util.StringIsEmpty(comboLeg.DesignatedLocation)) comboLegProto.DesignatedLocation = comboLeg.DesignatedLocation;
            if (Util.IsValidValue(comboLeg.ExemptCode)) comboLegProto.ExemptCode = comboLeg.ExemptCode;
            if (Util.IsValidValue(perLegPrice)) comboLegProto.PerLegPrice = perLegPrice;
            return comboLegProto;
        }

        public static protobuf.CancelOrderRequest createCancelOrderRequestProto(int id, OrderCancel orderCancel) 
        {
            protobuf.CancelOrderRequest cancelOrderRequestProto = new protobuf.CancelOrderRequest();
            if (Util.IsValidValue(id)) cancelOrderRequestProto.OrderId = id;
            protobuf.OrderCancel orderCancelProto = createOrderCancelProto(orderCancel);
            if (orderCancelProto != null) cancelOrderRequestProto.OrderCancel = orderCancelProto;
            return cancelOrderRequestProto;
        }

        public static protobuf.GlobalCancelRequest createGlobalCancelRequestProto(OrderCancel orderCancel) 
        {
            protobuf.GlobalCancelRequest globalCancelRequestProto = new protobuf.GlobalCancelRequest();
            protobuf.OrderCancel orderCancelProto = createOrderCancelProto(orderCancel);
            if (orderCancelProto != null) globalCancelRequestProto.OrderCancel = orderCancelProto;
            return globalCancelRequestProto;
        }

        public static protobuf.OrderCancel createOrderCancelProto(OrderCancel orderCancel) 
        {
            if (orderCancel == null)
            {
                return null;
            }
            protobuf.OrderCancel orderCancelProto = new protobuf.OrderCancel();
            if (!Util.StringIsEmpty(orderCancel.ManualOrderCancelTime)) orderCancelProto.ManualOrderCancelTime = orderCancel.ManualOrderCancelTime;
            if (!Util.StringIsEmpty(orderCancel.ExtOperator)) orderCancelProto.ExtOperator = orderCancel.ExtOperator;
            if (Util.IsValidValue(orderCancel.ManualOrderIndicator)) orderCancelProto.ManualOrderIndicator = orderCancel.ManualOrderIndicator;
            return orderCancelProto;
        }

        public static protobuf.AllOpenOrdersRequest createAllOpenOrdersRequestProto()
        {
            protobuf.AllOpenOrdersRequest allOpenOrdersRequestProto = new protobuf.AllOpenOrdersRequest();
            return allOpenOrdersRequestProto;
        }

        public static protobuf.AutoOpenOrdersRequest createAutoOpenOrdersRequestProto(bool autoBind)
        {
            protobuf.AutoOpenOrdersRequest autoOpenOrdersRequestProto = new protobuf.AutoOpenOrdersRequest();
            if (autoBind) autoOpenOrdersRequestProto.AutoBind = autoBind;
            return autoOpenOrdersRequestProto;
        }

        public static protobuf.OpenOrdersRequest createOpenOrdersRequestProto()
        {
            protobuf.OpenOrdersRequest openOrdersRequestProto = new protobuf.OpenOrdersRequest();
            return openOrdersRequestProto;
        }

        public static protobuf.CompletedOrdersRequest createCompletedOrdersRequestProto(bool apiOnly)
        {
            protobuf.CompletedOrdersRequest completedOrdersRequestProto = new protobuf.CompletedOrdersRequest();
            if (apiOnly) completedOrdersRequestProto.ApiOnly = apiOnly;
            return completedOrdersRequestProto;
        }

        public static protobuf.ContractDataRequest createContractDataRequestProto(int reqId, Contract contract)
        {
            protobuf.ContractDataRequest contractDataRequestProto = new protobuf.ContractDataRequest();
            if (Util.IsValidValue(reqId)) contractDataRequestProto.ReqId = reqId;
            protobuf.Contract contractProto = createContractProto(contract, null);
            if (contractProto != null) contractDataRequestProto.Contract = contractProto;
            return contractDataRequestProto;
        }

        public static protobuf.MarketDataRequest createMarketDataRequestProto(int reqId, Contract contract, string genericTickList, bool snapshot, bool regulatorySnapshot, List<TagValue> marketDataOptionsList)
        {
            protobuf.MarketDataRequest marketDataRequestProto = new protobuf.MarketDataRequest();
            if (Util.IsValidValue(reqId)) marketDataRequestProto.ReqId = reqId;
            protobuf.Contract contractProto = createContractProto(contract, null);
            if (contractProto != null) marketDataRequestProto.Contract = contractProto;
            if (!Util.StringIsEmpty(genericTickList)) marketDataRequestProto.GenericTickList = genericTickList;
            if (snapshot) marketDataRequestProto.Snapshot = snapshot;
            if (regulatorySnapshot) marketDataRequestProto.RegulatorySnapshot = regulatorySnapshot;

            if (marketDataOptionsList != null && marketDataOptionsList.Any())
            {
                Dictionary<string, string> marketDataOptions = marketDataOptionsList.ToDictionary(tagValue => tagValue.Tag, tagValue => tagValue.Value);
                marketDataRequestProto.MarketDataOptions.Add(marketDataOptions);
            }
            return marketDataRequestProto;
        }

        public static protobuf.MarketDepthRequest createMarketDepthRequestProto(int reqId, Contract contract, int numRows, bool isSmartDepth, List<TagValue> marketDepthOptionsList)
        {
            protobuf.MarketDepthRequest marketDepthRequestProto = new protobuf.MarketDepthRequest();
            if (Util.IsValidValue(reqId)) marketDepthRequestProto.ReqId = reqId;
            protobuf.Contract contractProto = createContractProto(contract, null);
            if (contractProto != null) marketDepthRequestProto.Contract = contractProto;
            if (Util.IsValidValue(numRows)) marketDepthRequestProto.NumRows = numRows;
            if (isSmartDepth) marketDepthRequestProto.IsSmartDepth = isSmartDepth;

            if (marketDepthOptionsList != null && marketDepthOptionsList.Any())
            {
                Dictionary<string, string> marketDepthOptions = marketDepthOptionsList.ToDictionary(tagValue => tagValue.Tag, tagValue => tagValue.Value);
                marketDepthRequestProto.MarketDepthOptions.Add(marketDepthOptions);
            }
            return marketDepthRequestProto;
        }

        public static protobuf.MarketDataTypeRequest createMarketDataTypeRequestProto(int marketDataType)
        {
            protobuf.MarketDataTypeRequest marketDataTypeRequestProto = new protobuf.MarketDataTypeRequest();
            if (Util.IsValidValue(marketDataType)) marketDataTypeRequestProto.MarketDataType = marketDataType;
            return marketDataTypeRequestProto;
        }

        public static protobuf.CancelMarketData createCancelMarketDataProto(int reqId)
        {
            protobuf.CancelMarketData cancelMarketDataProto = new protobuf.CancelMarketData();
            if (Util.IsValidValue(reqId)) cancelMarketDataProto.ReqId = reqId;
            return cancelMarketDataProto;
        }

        public static protobuf.CancelMarketDepth createCancelMarketDepthProto(int reqId, bool isSmartDepth)
        {
            protobuf.CancelMarketDepth cancelMarketDepthProto = new protobuf.CancelMarketDepth();
            if (Util.IsValidValue(reqId)) cancelMarketDepthProto.ReqId = reqId;
            if (isSmartDepth) cancelMarketDepthProto.IsSmartDepth = isSmartDepth;
            return cancelMarketDepthProto;
        }

        public static protobuf.AccountDataRequest createAccountDataRequestProto(bool subscribe, string acctCode)
        {
            protobuf.AccountDataRequest accountDataRequestProto = new protobuf.AccountDataRequest();
            if (subscribe) accountDataRequestProto.Subscribe = subscribe;
            if (!Util.StringIsEmpty(acctCode)) accountDataRequestProto.AcctCode = acctCode;
            return accountDataRequestProto;
        }

        public static protobuf.ManagedAccountsRequest createManagedAccountsRequestProto()
        {
            protobuf.ManagedAccountsRequest managedAccountsRequestProto = new protobuf.ManagedAccountsRequest();
            return managedAccountsRequestProto;
        }

        public static protobuf.PositionsRequest createPositionsRequestProto()
        {
            protobuf.PositionsRequest positionsRequestProto = new protobuf.PositionsRequest();
            return positionsRequestProto;
        }

        public static protobuf.CancelPositions createCancelPositionsRequestProto()
        {
            protobuf.CancelPositions cancelPositionsProto = new protobuf.CancelPositions();
            return cancelPositionsProto;
        }

        public static protobuf.AccountSummaryRequest createAccountSummaryRequestProto(int reqId, string group, string tags)
        {
            protobuf.AccountSummaryRequest accountSummaryRequestProto = new protobuf.AccountSummaryRequest();
            if (Util.IsValidValue(reqId)) accountSummaryRequestProto.ReqId = reqId;
            if (!Util.StringIsEmpty(group)) accountSummaryRequestProto.Group = group;
            if (!Util.StringIsEmpty(tags)) accountSummaryRequestProto.Tags = tags;
            return accountSummaryRequestProto;
        }

        public static protobuf.CancelAccountSummary createCancelAccountSummaryRequestProto(int reqId)
        {
            protobuf.CancelAccountSummary cancelAccountSummaryProto = new protobuf.CancelAccountSummary();
            if (Util.IsValidValue(reqId)) cancelAccountSummaryProto.ReqId = reqId;
            return cancelAccountSummaryProto;
        }

        public static protobuf.PositionsMultiRequest createPositionsMultiRequestProto(int reqId, string account, string modelCode)
        {
            protobuf.PositionsMultiRequest positionsMultiRequestProto = new protobuf.PositionsMultiRequest();
            if (Util.IsValidValue(reqId)) positionsMultiRequestProto.ReqId = reqId;
            if (!Util.StringIsEmpty(account)) positionsMultiRequestProto.Account = account;
            if (!Util.StringIsEmpty(modelCode)) positionsMultiRequestProto.ModelCode = modelCode;
            return positionsMultiRequestProto;
        }

        public static protobuf.CancelPositionsMulti createCancelPositionsMultiRequestProto(int reqId)
        {
            protobuf.CancelPositionsMulti cancelPositionsMultiProto = new protobuf.CancelPositionsMulti();
            if (Util.IsValidValue(reqId)) cancelPositionsMultiProto.ReqId = reqId;
            return cancelPositionsMultiProto;
        }

        public static protobuf.AccountUpdatesMultiRequest createAccountUpdatesMultiRequestProto(int reqId, string account, string modelCode, bool ledgerAndNLV)
        {
            protobuf.AccountUpdatesMultiRequest accountUpdatesMultiRequestProto = new protobuf.AccountUpdatesMultiRequest();
            if (Util.IsValidValue(reqId)) accountUpdatesMultiRequestProto.ReqId = reqId;
            if (!Util.StringIsEmpty(account)) accountUpdatesMultiRequestProto.Account = account;
            if (!Util.StringIsEmpty(modelCode)) accountUpdatesMultiRequestProto.ModelCode = modelCode;
            if (ledgerAndNLV) accountUpdatesMultiRequestProto.LedgerAndNLV = ledgerAndNLV;
            return accountUpdatesMultiRequestProto;
        }

        public static protobuf.CancelAccountUpdatesMulti createCancelAccountUpdatesMultiRequestProto(int reqId)
        {
            protobuf.CancelAccountUpdatesMulti cancelAccountUpdatesMultiProto = new protobuf.CancelAccountUpdatesMulti();
            if (Util.IsValidValue(reqId)) cancelAccountUpdatesMultiProto.ReqId = reqId;
            return cancelAccountUpdatesMultiProto;
        }

        public static protobuf.HistoricalDataRequest createHistoricalDataRequestProto(int reqId, Contract contract, string endDateTime,
            string duration, string barSizeSetting, string whatToShow, bool useRTH, int formatDate, bool keepUpToDate, List<TagValue> chartOptionsList)
        {
            protobuf.HistoricalDataRequest historicalDataRequestProto = new protobuf.HistoricalDataRequest();
            if (Util.IsValidValue(reqId)) historicalDataRequestProto.ReqId = reqId;
            protobuf.Contract contractProto = createContractProto(contract, null);
            if (contractProto != null) historicalDataRequestProto.Contract = contractProto;

            if (!Util.StringIsEmpty(endDateTime)) historicalDataRequestProto.EndDateTime = endDateTime;
            if (!Util.StringIsEmpty(duration)) historicalDataRequestProto.Duration = duration;
            if (!Util.StringIsEmpty(barSizeSetting)) historicalDataRequestProto.BarSizeSetting = barSizeSetting;
            if (!Util.StringIsEmpty(whatToShow)) historicalDataRequestProto.WhatToShow = whatToShow;
            if (useRTH) historicalDataRequestProto.UseRTH = useRTH;
            if (Util.IsValidValue(formatDate)) historicalDataRequestProto.FormatDate = formatDate;
            if (keepUpToDate) historicalDataRequestProto.KeepUpToDate = keepUpToDate;

            if (chartOptionsList != null && chartOptionsList.Any())
            {
                Dictionary<string, string> chartOptions = chartOptionsList.ToDictionary(tagValue => tagValue.Tag, tagValue => tagValue.Value);
                historicalDataRequestProto.ChartOptions.Add(chartOptions);
            }

            return historicalDataRequestProto;
        }

        public static protobuf.RealTimeBarsRequest createRealTimeBarsRequestProto(int reqId, Contract contract, int barSize, string whatToShow, bool useRTH, List<TagValue> realTimeBarsOptionsList)
        {
            protobuf.RealTimeBarsRequest realTimeBarsRequestProto = new protobuf.RealTimeBarsRequest();
            if (Util.IsValidValue(reqId)) realTimeBarsRequestProto.ReqId = reqId;
            protobuf.Contract contractProto = createContractProto(contract, null);
            if (contractProto != null) realTimeBarsRequestProto.Contract = contractProto;

            if (Util.IsValidValue(barSize)) realTimeBarsRequestProto.BarSize = barSize;
            if (!Util.StringIsEmpty(whatToShow)) realTimeBarsRequestProto.WhatToShow = whatToShow;
            if (useRTH) realTimeBarsRequestProto.UseRTH = useRTH;

            if (realTimeBarsOptionsList != null && realTimeBarsOptionsList.Any())
            {
                Dictionary<string, string> realTimeBarsOptions = realTimeBarsOptionsList.ToDictionary(tagValue => tagValue.Tag, tagValue => tagValue.Value);
                realTimeBarsRequestProto.RealTimeBarsOptions.Add(realTimeBarsOptions);
            }

            return realTimeBarsRequestProto;
        }

        public static protobuf.HeadTimestampRequest createHeadTimestampRequestProto(int reqId, Contract contract, string whatToShow, bool useRTH, int formatDate)
        {
            protobuf.HeadTimestampRequest headTimestampRequestProto = new protobuf.HeadTimestampRequest();
            if (Util.IsValidValue(reqId)) headTimestampRequestProto.ReqId = reqId;
            protobuf.Contract contractProto = createContractProto(contract, null);
            if (contractProto != null) headTimestampRequestProto.Contract = contractProto;
            if (!Util.StringIsEmpty(whatToShow)) headTimestampRequestProto.WhatToShow = whatToShow;
            if (useRTH) headTimestampRequestProto.UseRTH = useRTH;
            if (Util.IsValidValue(formatDate)) headTimestampRequestProto.FormatDate = formatDate;
            return headTimestampRequestProto;
        }

        public static protobuf.HistogramDataRequest createHistogramDataRequestProto(int reqId, Contract contract, bool useRTH, string timePeriod)
        {
            protobuf.HistogramDataRequest histogramDataRequestProto = new protobuf.HistogramDataRequest();
            if (Util.IsValidValue(reqId)) histogramDataRequestProto.ReqId = reqId;
            protobuf.Contract contractProto = createContractProto(contract, null);
            if (contractProto != null) histogramDataRequestProto.Contract = contractProto;
            histogramDataRequestProto.UseRTH = useRTH;
            if (!Util.StringIsEmpty(timePeriod)) histogramDataRequestProto.TimePeriod = timePeriod;
            return histogramDataRequestProto;
        }

        public static protobuf.HistoricalTicksRequest createHistoricalTicksRequestProto(int reqId, Contract contract, string startDateTime, string endDateTime, 
            int numberOfTicks, string whatToShow, bool useRTH, bool ignoreSize, List<TagValue> miscOptionsList)
        {
            protobuf.HistoricalTicksRequest historicalTicksRequestProto = new protobuf.HistoricalTicksRequest();
            if (Util.IsValidValue(reqId)) historicalTicksRequestProto.ReqId = reqId;
            protobuf.Contract contractProto = createContractProto(contract, null);
            if (contractProto != null) historicalTicksRequestProto.Contract = contractProto;
            if (!Util.StringIsEmpty(startDateTime)) historicalTicksRequestProto.StartDateTime = startDateTime;
            if (!Util.StringIsEmpty(endDateTime)) historicalTicksRequestProto.EndDateTime = endDateTime;
            if (Util.IsValidValue(numberOfTicks)) historicalTicksRequestProto.NumberOfTicks = numberOfTicks;
            if (!Util.StringIsEmpty(whatToShow)) historicalTicksRequestProto.WhatToShow = whatToShow;
            if (useRTH) historicalTicksRequestProto.UseRTH = useRTH;
            if (ignoreSize) historicalTicksRequestProto.IgnoreSize = ignoreSize;

            if (miscOptionsList != null && miscOptionsList.Any())
            {
                Dictionary<string, string> miscOptions = miscOptionsList.ToDictionary(tagValue => tagValue.Tag, tagValue => tagValue.Value);
                historicalTicksRequestProto.MiscOptions.Add(miscOptions);
            }

            return historicalTicksRequestProto;
        }

        public static protobuf.TickByTickRequest createTickByTickRequestProto(int reqId, Contract contract, string tickType, int numberOfTicks, bool ignoreSize)
        {
            protobuf.TickByTickRequest tickByTickRequestProto = new protobuf.TickByTickRequest();
            if (Util.IsValidValue(reqId)) tickByTickRequestProto.ReqId = reqId;
            protobuf.Contract contractProto = createContractProto(contract, null);
            if (contractProto != null) tickByTickRequestProto.Contract = contractProto;
            if (!Util.StringIsEmpty(tickType)) tickByTickRequestProto.TickType = tickType;
            if (Util.IsValidValue(numberOfTicks)) tickByTickRequestProto.NumberOfTicks = numberOfTicks;
            if (ignoreSize) tickByTickRequestProto.IgnoreSize = ignoreSize;
            return tickByTickRequestProto;
        }

        public static protobuf.CancelHistoricalData createCancelHistoricalDataProto(int reqId)
        {
            protobuf.CancelHistoricalData cancelHistoricalDataProto = new protobuf.CancelHistoricalData();
            if (Util.IsValidValue(reqId)) cancelHistoricalDataProto.ReqId = reqId;
            return cancelHistoricalDataProto;
        }

        public static protobuf.CancelRealTimeBars createCancelRealTimeBarsProto(int reqId)
        {
            protobuf.CancelRealTimeBars cancelRealTimeBarsProto = new protobuf.CancelRealTimeBars();
            if (Util.IsValidValue(reqId)) cancelRealTimeBarsProto.ReqId = reqId;
            return cancelRealTimeBarsProto;
        }

        public static protobuf.CancelHeadTimestamp createCancelHeadTimestampProto(int reqId)
        {
            protobuf.CancelHeadTimestamp cancelHeadTimestampProto = new protobuf.CancelHeadTimestamp();
            if (Util.IsValidValue(reqId)) cancelHeadTimestampProto.ReqId = reqId;
            return cancelHeadTimestampProto;
        }

        public static protobuf.CancelHistogramData createCancelHistogramDataProto(int reqId)
        {
            protobuf.CancelHistogramData cancelHistogramDataProto = new protobuf.CancelHistogramData();
            if (Util.IsValidValue(reqId)) cancelHistogramDataProto.ReqId = reqId;
            return cancelHistogramDataProto;
        }

        public static protobuf.CancelTickByTick createCancelTickByTickProto(int reqId)
        {
            protobuf.CancelTickByTick cancelTickByTickProto = new protobuf.CancelTickByTick();
            if (Util.IsValidValue(reqId)) cancelTickByTickProto.ReqId = reqId;
            return cancelTickByTickProto;
        }

        public static protobuf.NewsBulletinsRequest createNewsBulletinsRequestProto(bool allMessages)
        {
            protobuf.NewsBulletinsRequest newsBulletinsRequestProto = new protobuf.NewsBulletinsRequest();
            if (allMessages) newsBulletinsRequestProto.AllMessages = allMessages;
            return newsBulletinsRequestProto;
        }

        public static protobuf.CancelNewsBulletins createCancelNewsBulletinsProto()
        {
            protobuf.CancelNewsBulletins cancelNewsBulletinsProto = new protobuf.CancelNewsBulletins();
            return cancelNewsBulletinsProto;
        }

        public static protobuf.NewsArticleRequest createNewsArticleRequestProto(int reqId, string providerCode, string articleId, List<TagValue> newsArticleOptionsList)
        {
            protobuf.NewsArticleRequest newsArticleRequestProto = new protobuf.NewsArticleRequest();
            if (Util.IsValidValue(reqId)) newsArticleRequestProto.ReqId = reqId;
            if (!Util.StringIsEmpty(providerCode)) newsArticleRequestProto.ProviderCode = providerCode;
            if (!Util.StringIsEmpty(articleId)) newsArticleRequestProto.ArticleId = articleId;

            if (newsArticleOptionsList != null && newsArticleOptionsList.Any())
            {
                Dictionary<string, string> newsArticleOptions = newsArticleOptionsList.ToDictionary(tagValue => tagValue.Tag, tagValue => tagValue.Value);
                newsArticleRequestProto.NewsArticleOptions.Add(newsArticleOptions);
            }

            return newsArticleRequestProto;
        }

        public static protobuf.NewsProvidersRequest createNewsProvidersRequestProto()
        {
            protobuf.NewsProvidersRequest newsProvidersRequestProto = new protobuf.NewsProvidersRequest();
            return newsProvidersRequestProto;
        }

        public static protobuf.HistoricalNewsRequest createHistoricalNewsRequestProto(int reqId, int conId, string providerCodes, string startDateTime, string endDateTime, int totalResults, List<TagValue> historicalNewsOptionsList)
        {
            protobuf.HistoricalNewsRequest historicalNewsRequestProto = new protobuf.HistoricalNewsRequest();
            if (Util.IsValidValue(reqId)) historicalNewsRequestProto.ReqId = reqId;
            if (Util.IsValidValue(conId)) historicalNewsRequestProto.ConId = conId;
            if (!Util.StringIsEmpty(providerCodes)) historicalNewsRequestProto.ProviderCodes = providerCodes;
            if (!Util.StringIsEmpty(startDateTime)) historicalNewsRequestProto.StartDateTime = startDateTime;
            if (!Util.StringIsEmpty(endDateTime)) historicalNewsRequestProto.EndDateTime = endDateTime;
            if (Util.IsValidValue(totalResults)) historicalNewsRequestProto.TotalResults = totalResults;

            if (historicalNewsOptionsList != null && historicalNewsOptionsList.Any())
            {
                Dictionary<string, string> historicalNewsOptions = historicalNewsOptionsList.ToDictionary(tagValue => tagValue.Tag, tagValue => tagValue.Value);
                historicalNewsRequestProto.HistoricalNewsOptions.Add(historicalNewsOptions);
            }

            return historicalNewsRequestProto;
        }

        public static protobuf.WshMetaDataRequest createWshMetaDataRequestProto(int reqId)
        {
            protobuf.WshMetaDataRequest wshMetaDataRequestProto = new protobuf.WshMetaDataRequest();
            if (Util.IsValidValue(reqId)) wshMetaDataRequestProto.ReqId = reqId;
            return wshMetaDataRequestProto;
        }

        public static protobuf.CancelWshMetaData createCancelWshMetaDataProto(int reqId)
        {
            protobuf.CancelWshMetaData cancelWshMetaDataProto = new protobuf.CancelWshMetaData();
            if (Util.IsValidValue(reqId)) cancelWshMetaDataProto.ReqId = reqId;
            return cancelWshMetaDataProto;
        }

        public static protobuf.WshEventDataRequest createWshEventDataRequestProto(int reqId, WshEventData wshEventData)
        {
            protobuf.WshEventDataRequest wshEventDataRequestProto = new protobuf.WshEventDataRequest();
            if (Util.IsValidValue(reqId)) wshEventDataRequestProto.ReqId = reqId;

            if (wshEventData != null)
            {
                if (Util.IsValidValue(wshEventData.ConId)) wshEventDataRequestProto.ConId = wshEventData.ConId;

                if (!Util.StringIsEmpty(wshEventData.Filter)) wshEventDataRequestProto.Filter = wshEventData.Filter;
                if (wshEventData.FillWatchlist) wshEventDataRequestProto.FillWatchlist = wshEventData.FillWatchlist;
                if (wshEventData.FillPortfolio) wshEventDataRequestProto.FillPortfolio = wshEventData.FillPortfolio;
                if (wshEventData.FillCompetitors) wshEventDataRequestProto.FillCompetitors = wshEventData.FillCompetitors;
                if (!Util.StringIsEmpty(wshEventData.StartDate)) wshEventDataRequestProto.StartDate = wshEventData.StartDate;
                if (!Util.StringIsEmpty(wshEventData.EndDate)) wshEventDataRequestProto.EndDate = wshEventData.EndDate;
                if (Util.IsValidValue(wshEventData.TotalLimit)) wshEventDataRequestProto.TotalLimit = wshEventData.TotalLimit;
            }

            return wshEventDataRequestProto;
        }

        public static protobuf.CancelWshEventData createCancelWshEventDataProto(int reqId)
        {
            protobuf.CancelWshEventData cancelWshEventDataProto = new protobuf.CancelWshEventData();
            if (Util.IsValidValue(reqId)) cancelWshEventDataProto.ReqId = reqId;
            return cancelWshEventDataProto;
        }

        public static protobuf.ScannerParametersRequest createScannerParametersRequestProto()
        {
            protobuf.ScannerParametersRequest scannerParametersRequestProto = new protobuf.ScannerParametersRequest();
            return scannerParametersRequestProto;
        }

        public static protobuf.ScannerSubscriptionRequest createScannerSubscriptionRequestProto(int reqId, ScannerSubscription subscription, 
            List<TagValue> scannerSubscriptionOptionsList, List<TagValue> scannerSubscriptionFilterOptionsList)
        {
            protobuf.ScannerSubscriptionRequest scannerSubscriptionRequestProto = new protobuf.ScannerSubscriptionRequest();
            if (Util.IsValidValue(reqId)) scannerSubscriptionRequestProto.ReqId = reqId;
            protobuf.ScannerSubscription scannerSubscriptionProto = createScannerSubscriptionProto(subscription, scannerSubscriptionOptionsList, scannerSubscriptionFilterOptionsList);
            if (scannerSubscriptionProto != null) scannerSubscriptionRequestProto.ScannerSubscription = scannerSubscriptionProto;
            return scannerSubscriptionRequestProto;
        }

        private static protobuf.ScannerSubscription createScannerSubscriptionProto(ScannerSubscription subscription, List<TagValue> scannerSubscriptionOptionsList, List<TagValue> scannerSubscriptionFilterOptionsList)
        {
            if (subscription == null)
            {
                return null;
            }
            protobuf.ScannerSubscription scannerSubscriptionProto = new protobuf.ScannerSubscription();
            if (Util.IsValidValue(subscription.NumberOfRows)) scannerSubscriptionProto.NumberOfRows = subscription.NumberOfRows;
            if (!Util.StringIsEmpty(subscription.Instrument)) scannerSubscriptionProto.Instrument = subscription.Instrument;
            if (!Util.StringIsEmpty(subscription.LocationCode)) scannerSubscriptionProto.LocationCode = subscription.LocationCode;
            if (!Util.StringIsEmpty(subscription.ScanCode)) scannerSubscriptionProto.ScanCode = subscription.ScanCode;
            if (Util.IsValidValue(subscription.AbovePrice)) scannerSubscriptionProto.AbovePrice = subscription.AbovePrice;
            if (Util.IsValidValue(subscription.BelowPrice)) scannerSubscriptionProto.BelowPrice = subscription.BelowPrice;
            if (Util.IsValidValue(subscription.AboveVolume)) scannerSubscriptionProto.AboveVolume = subscription.AboveVolume;
            if (Util.IsValidValue(subscription.AverageOptionVolumeAbove)) scannerSubscriptionProto.AverageOptionVolumeAbove = subscription.AverageOptionVolumeAbove;
            if (Util.IsValidValue(subscription.MarketCapAbove)) scannerSubscriptionProto.MarketCapAbove = subscription.MarketCapAbove;
            if (Util.IsValidValue(subscription.MarketCapBelow)) scannerSubscriptionProto.MarketCapBelow = subscription.MarketCapBelow;
            if (!Util.StringIsEmpty(subscription.MoodyRatingAbove)) scannerSubscriptionProto.MoodyRatingAbove = subscription.MoodyRatingAbove;
            if (!Util.StringIsEmpty(subscription.MoodyRatingBelow)) scannerSubscriptionProto.MoodyRatingBelow = subscription.MoodyRatingBelow;
            if (!Util.StringIsEmpty(subscription.SpRatingAbove)) scannerSubscriptionProto.SpRatingAbove = subscription.SpRatingAbove;
            if (!Util.StringIsEmpty(subscription.SpRatingBelow)) scannerSubscriptionProto.SpRatingBelow = subscription.SpRatingBelow;
            if (!Util.StringIsEmpty(subscription.MaturityDateAbove)) scannerSubscriptionProto.MaturityDateAbove = subscription.MaturityDateAbove;
            if (!Util.StringIsEmpty(subscription.MaturityDateBelow)) scannerSubscriptionProto.MaturityDateBelow = subscription.MaturityDateBelow;
            if (Util.IsValidValue(subscription.CouponRateAbove)) scannerSubscriptionProto.CouponRateAbove = subscription.CouponRateAbove;
            if (Util.IsValidValue(subscription.CouponRateBelow)) scannerSubscriptionProto.CouponRateBelow = subscription.CouponRateBelow;
            if (subscription.ExcludeConvertible) scannerSubscriptionProto.ExcludeConvertible = subscription.ExcludeConvertible;
            if (!Util.StringIsEmpty(subscription.ScannerSettingPairs)) scannerSubscriptionProto.ScannerSettingPairs = subscription.ScannerSettingPairs;
            if (!Util.StringIsEmpty(subscription.StockTypeFilter)) scannerSubscriptionProto.StockTypeFilter = subscription.StockTypeFilter;

            if (scannerSubscriptionOptionsList != null && scannerSubscriptionOptionsList.Any())
            {
                Dictionary<string, string> scannerSubscriptionOptions = scannerSubscriptionOptionsList.ToDictionary(tagValue => tagValue.Tag, tagValue => tagValue.Value);
                scannerSubscriptionProto.ScannerSubscriptionOptions.Add(scannerSubscriptionOptions);
            }

            if (scannerSubscriptionFilterOptionsList != null && scannerSubscriptionFilterOptionsList.Any())
            {
                Dictionary<string, string> scannerSubscriptionFilterOptions = scannerSubscriptionFilterOptionsList.ToDictionary(tagValue => tagValue.Tag, tagValue => tagValue.Value);
                scannerSubscriptionProto.ScannerSubscriptionFilterOptions.Add(scannerSubscriptionFilterOptions);
            }

            return scannerSubscriptionProto;
        }

        public static protobuf.FundamentalsDataRequest createFundamentalsDataRequestProto(int reqId, Contract contract, string reportType, List<TagValue> fundamentalsDataOptionsList)
        {
            protobuf.FundamentalsDataRequest fundamentalsDataRequestProto = new protobuf.FundamentalsDataRequest();
            if (Util.IsValidValue(reqId)) fundamentalsDataRequestProto.ReqId = reqId;
            protobuf.Contract contractProto = createContractProto(contract, null);
            if (contractProto != null) fundamentalsDataRequestProto.Contract = contractProto;
            if (!Util.StringIsEmpty(reportType)) fundamentalsDataRequestProto.ReportType = reportType;
            if (fundamentalsDataOptionsList != null && fundamentalsDataOptionsList.Any())
            {
                Dictionary<string, string> fundamentalsDataOptions = fundamentalsDataOptionsList.ToDictionary(tagValue => tagValue.Tag, tagValue => tagValue.Value);
                fundamentalsDataRequestProto.FundamentalsDataOptions.Add(fundamentalsDataOptions);
            }
            return fundamentalsDataRequestProto;
        }

        public static protobuf.PnLRequest createPnLRequestProto(int reqId, string account, string modelCode)
        {
            protobuf.PnLRequest pnlRequestProto = new protobuf.PnLRequest();
            if (Util.IsValidValue(reqId)) pnlRequestProto.ReqId = reqId;
            if (!Util.StringIsEmpty(account)) pnlRequestProto.Account = account;
            if (!Util.StringIsEmpty(modelCode)) pnlRequestProto.ModelCode = modelCode;
            return pnlRequestProto;
        }

        public static protobuf.PnLSingleRequest createPnLSingleRequestProto(int reqId, string account, string modelCode, int conId)
        {
            protobuf.PnLSingleRequest pnlSingleRequestProto = new protobuf.PnLSingleRequest();
            if (Util.IsValidValue(reqId)) pnlSingleRequestProto.ReqId = reqId;
            if (!Util.StringIsEmpty(account)) pnlSingleRequestProto.Account = account;
            if (!Util.StringIsEmpty(modelCode)) pnlSingleRequestProto.ModelCode = modelCode;
            if (Util.IsValidValue(conId)) pnlSingleRequestProto.ConId = conId;
            return pnlSingleRequestProto;
        }

        public static protobuf.CancelScannerSubscription createCancelScannerSubscriptionProto(int reqId)
        {
            protobuf.CancelScannerSubscription cancelScannerSubscriptionProto = new protobuf.CancelScannerSubscription();
            if (Util.IsValidValue(reqId)) cancelScannerSubscriptionProto.ReqId = reqId;
            return cancelScannerSubscriptionProto;
        }

        public static protobuf.CancelFundamentalsData createCancelFundamentalsDataProto(int reqId)
        {
            protobuf.CancelFundamentalsData cancelFundamentalsDataProto = new protobuf.CancelFundamentalsData();
            if (Util.IsValidValue(reqId)) cancelFundamentalsDataProto.ReqId = reqId;
            return cancelFundamentalsDataProto;
        }

        public static protobuf.CancelPnL createCancelPnLProto(int reqId)
        {
            protobuf.CancelPnL cancelPnLProto = new protobuf.CancelPnL();
            if (Util.IsValidValue(reqId)) cancelPnLProto.ReqId = reqId;
            return cancelPnLProto;
        }

        public static protobuf.CancelPnLSingle createCancelPnLSingleProto(int reqId)
        {
            protobuf.CancelPnLSingle cancelPnLSingleProto = new protobuf.CancelPnLSingle();
            if (Util.IsValidValue(reqId)) cancelPnLSingleProto.ReqId = reqId;
            return cancelPnLSingleProto;
        }

        public static protobuf.FARequest createFARequestProto(int faDataType)
        {
            protobuf.FARequest faRequestProto = new protobuf.FARequest();
            if (Util.IsValidValue(faDataType)) faRequestProto.FaDataType = faDataType;
            return faRequestProto;
        }

        public static protobuf.FAReplace createFAReplaceProto(int reqId, int faDataType, string xml)
        {
            protobuf.FAReplace faReplaceProto = new protobuf.FAReplace();
            if (Util.IsValidValue(reqId)) faReplaceProto.ReqId = reqId;
            if (Util.IsValidValue(faDataType)) faReplaceProto.FaDataType = faDataType;
            if (!Util.StringIsEmpty(xml)) faReplaceProto.Xml = xml;
            return faReplaceProto;
        }

        public static protobuf.ExerciseOptionsRequest createExerciseOptionsRequestProto(int orderId, Contract contract, int exerciseAction, int exerciseQuantity, string account, bool ovrd, string manualOrderTime, string customerAccount, bool professionalCustomer)
        {
            protobuf.ExerciseOptionsRequest exerciseOptionsRequestProto = new protobuf.ExerciseOptionsRequest();
            if (Util.IsValidValue(orderId)) exerciseOptionsRequestProto.OrderId = orderId;
            protobuf.Contract contractProto = createContractProto(contract, null);
            if (contractProto != null) exerciseOptionsRequestProto.Contract = contractProto;
            if (Util.IsValidValue(exerciseAction)) exerciseOptionsRequestProto.ExerciseAction = exerciseAction;
            if (Util.IsValidValue(exerciseQuantity)) exerciseOptionsRequestProto.ExerciseQuantity = exerciseQuantity;
            if (!Util.StringIsEmpty(account)) exerciseOptionsRequestProto.Account = account;
            if (ovrd) exerciseOptionsRequestProto.Override = ovrd;
            if (!Util.StringIsEmpty(manualOrderTime)) exerciseOptionsRequestProto.ManualOrderTime = manualOrderTime;
            if (!Util.StringIsEmpty(customerAccount)) exerciseOptionsRequestProto.CustomerAccount = customerAccount;
            if (professionalCustomer) exerciseOptionsRequestProto.ProfessionalCustomer = professionalCustomer;
            return exerciseOptionsRequestProto;
        }

        public static protobuf.CalculateImpliedVolatilityRequest createCalculateImpliedVolatilityRequestProto(int reqId, Contract contract, double optionPrice, double underPrice, List<TagValue> implVolOptionsList)
        {
            protobuf.CalculateImpliedVolatilityRequest calculateImpliedVolatilityRequestProto = new protobuf.CalculateImpliedVolatilityRequest();
            if (Util.IsValidValue(reqId)) calculateImpliedVolatilityRequestProto.ReqId = reqId;
            protobuf.Contract contractProto = createContractProto(contract, null);
            if (contractProto != null) calculateImpliedVolatilityRequestProto.Contract = contractProto;
            if (Util.IsValidValue(optionPrice)) calculateImpliedVolatilityRequestProto.OptionPrice = optionPrice;
            if (Util.IsValidValue(underPrice)) calculateImpliedVolatilityRequestProto.UnderPrice = underPrice;
            if (implVolOptionsList != null && implVolOptionsList.Any())
            {
                Dictionary<string, string> implVolOptions = implVolOptionsList.ToDictionary(tagValue => tagValue.Tag, tagValue => tagValue.Value);
                calculateImpliedVolatilityRequestProto.ImpliedVolatilityOptions.Add(implVolOptions);
            }
            return calculateImpliedVolatilityRequestProto;
        }

        public static protobuf.CancelCalculateImpliedVolatility createCancelCalculateImpliedVolatilityProto(int reqId)
        {
            protobuf.CancelCalculateImpliedVolatility cancelCalculateImpliedVolatilityProto = new protobuf.CancelCalculateImpliedVolatility();
            if (Util.IsValidValue(reqId)) cancelCalculateImpliedVolatilityProto.ReqId = reqId;
            return cancelCalculateImpliedVolatilityProto;
        }

        public static protobuf.CalculateOptionPriceRequest createCalculateOptionPriceRequestProto(int reqId, Contract contract, double volatility, double underPrice, List<TagValue> optPrcOptionsList)
        {
            protobuf.CalculateOptionPriceRequest calculateOptionPriceRequestProto = new protobuf.CalculateOptionPriceRequest();
            if (Util.IsValidValue(reqId)) calculateOptionPriceRequestProto.ReqId = reqId;
            protobuf.Contract contractProto = createContractProto(contract, null);
            if (contractProto != null) calculateOptionPriceRequestProto.Contract = contractProto;
            if (Util.IsValidValue(volatility)) calculateOptionPriceRequestProto.Volatility = volatility;
            if (Util.IsValidValue(underPrice)) calculateOptionPriceRequestProto.UnderPrice = underPrice;
            if (optPrcOptionsList != null && optPrcOptionsList.Any())
            {
                Dictionary<string, string> optPrcOptions = optPrcOptionsList.ToDictionary(tagValue => tagValue.Tag, tagValue => tagValue.Value);
                calculateOptionPriceRequestProto.OptionPriceOptions.Add(optPrcOptions);
            }
            return calculateOptionPriceRequestProto;
        }

        public static protobuf.CancelCalculateOptionPrice createCancelCalculateOptionPriceProto(int reqId)
        {
            protobuf.CancelCalculateOptionPrice cancelCalculateOptionPriceProto = new protobuf.CancelCalculateOptionPrice();
            if (Util.IsValidValue(reqId)) cancelCalculateOptionPriceProto.ReqId = reqId;
            return cancelCalculateOptionPriceProto;
        }

        public static protobuf.SecDefOptParamsRequest createSecDefOptParamsRequestProto(int reqId, string underlyingSymbol, string futFopExchange, string underlyingSecType, int underlyingConId)
        {
            protobuf.SecDefOptParamsRequest secDefOptParamsRequestProto = new protobuf.SecDefOptParamsRequest();
            if (Util.IsValidValue(reqId)) secDefOptParamsRequestProto.ReqId = reqId;
            if (!Util.StringIsEmpty(underlyingSymbol)) secDefOptParamsRequestProto.UnderlyingSymbol = underlyingSymbol;
            if (!Util.StringIsEmpty(futFopExchange)) secDefOptParamsRequestProto.FutFopExchange = futFopExchange;
            if (!Util.StringIsEmpty(underlyingSecType)) secDefOptParamsRequestProto.UnderlyingSecType = underlyingSecType;
            if (Util.IsValidValue(underlyingConId)) secDefOptParamsRequestProto.UnderlyingConId = underlyingConId;
            return secDefOptParamsRequestProto;
        }

        public static protobuf.SoftDollarTiersRequest createSoftDollarTiersRequestProto(int reqId)
        {
            protobuf.SoftDollarTiersRequest softDollarTiersRequestProto = new protobuf.SoftDollarTiersRequest();
            if (Util.IsValidValue(reqId)) softDollarTiersRequestProto.ReqId = reqId;
            return softDollarTiersRequestProto;
        }

        public static protobuf.FamilyCodesRequest createFamilyCodesRequestProto()
        {
            protobuf.FamilyCodesRequest familyCodesRequestProto = new protobuf.FamilyCodesRequest();
            return familyCodesRequestProto;
        }

        public static protobuf.MatchingSymbolsRequest createMatchingSymbolsRequestProto(int reqId, string pattern)
        {
            protobuf.MatchingSymbolsRequest matchingSymbolsRequestProto = new protobuf.MatchingSymbolsRequest();
            if (Util.IsValidValue(reqId)) matchingSymbolsRequestProto.ReqId = reqId;
            if (!Util.StringIsEmpty(pattern)) matchingSymbolsRequestProto.Pattern = pattern;
            return matchingSymbolsRequestProto;
        }

        public static protobuf.SmartComponentsRequest createSmartComponentsRequestProto(int reqId, string bboExchange)
        {
            protobuf.SmartComponentsRequest smartComponentsRequestProto = new protobuf.SmartComponentsRequest();
            if (Util.IsValidValue(reqId)) smartComponentsRequestProto.ReqId = reqId;
            if (!Util.StringIsEmpty(bboExchange)) smartComponentsRequestProto.BboExchange = bboExchange;
            return smartComponentsRequestProto;
        }

        public static protobuf.MarketRuleRequest createMarketRuleRequestProto(int marketRuleId)
        {
            protobuf.MarketRuleRequest marketRuleRequestProto = new protobuf.MarketRuleRequest();
            if (Util.IsValidValue(marketRuleId)) marketRuleRequestProto.MarketRuleId = marketRuleId;
            return marketRuleRequestProto;
        }

        public static protobuf.UserInfoRequest createUserInfoRequestProto(int reqId)
        {
            protobuf.UserInfoRequest userInfoRequestProto = new protobuf.UserInfoRequest();
            if (Util.IsValidValue(reqId)) userInfoRequestProto.ReqId = reqId;
            return userInfoRequestProto;
        }

        public static protobuf.IdsRequest createIdsRequestProto(int numIds)
        {
            protobuf.IdsRequest idsRequestProto = new protobuf.IdsRequest();
            if (Util.IsValidValue(numIds)) idsRequestProto.NumIds = numIds;
            return idsRequestProto;
        }

        public static protobuf.CurrentTimeRequest createCurrentTimeRequestProto()
        {
            protobuf.CurrentTimeRequest currentTimeRequestProto = new protobuf.CurrentTimeRequest();
            return currentTimeRequestProto;
        }

        public static protobuf.CurrentTimeInMillisRequest createCurrentTimeInMillisRequestProto()
        {
            protobuf.CurrentTimeInMillisRequest currentTimeInMillisRequestProto = new protobuf.CurrentTimeInMillisRequest();
            return currentTimeInMillisRequestProto;
        }

        public static protobuf.StartApiRequest createStartApiRequestProto(int clientId, string optionalCapabilities)
        {
            protobuf.StartApiRequest startApiRequestProto = new protobuf.StartApiRequest();
            if (Util.IsValidValue(clientId)) startApiRequestProto.ClientId = clientId;
            if (!Util.StringIsEmpty(optionalCapabilities)) startApiRequestProto.OptionalCapabilities = optionalCapabilities;
            return startApiRequestProto;
        }

        public static protobuf.SetServerLogLevelRequest createSetServerLogLevelRequestProto(int logLevel)
        {
            protobuf.SetServerLogLevelRequest setServerLogLevelRequestProto = new protobuf.SetServerLogLevelRequest();
            if (Util.IsValidValue(logLevel)) setServerLogLevelRequestProto.LogLevel = logLevel;
            return setServerLogLevelRequestProto;
        }

        public static protobuf.VerifyRequest createVerifyRequestProto(string apiName, string apiVersion)
        {
            protobuf.VerifyRequest verifyRequestProto = new protobuf.VerifyRequest();
            if (!Util.StringIsEmpty(apiName)) verifyRequestProto.ApiName = apiName;
            if (!Util.StringIsEmpty(apiVersion)) verifyRequestProto.ApiVersion = apiVersion;
            return verifyRequestProto;
        }

        public static protobuf.VerifyMessageRequest createVerifyMessageRequestProto(string apiData)
        {
            protobuf.VerifyMessageRequest verifyMessageRequestProto = new protobuf.VerifyMessageRequest();
            if (!Util.StringIsEmpty(apiData)) verifyMessageRequestProto.ApiData = apiData;
            return verifyMessageRequestProto;
        }

        public static protobuf.QueryDisplayGroupsRequest createQueryDisplayGroupsRequestProto(int reqId)
        {
            protobuf.QueryDisplayGroupsRequest queryDisplayGroupsRequestProto = new protobuf.QueryDisplayGroupsRequest();
            if (Util.IsValidValue(reqId)) queryDisplayGroupsRequestProto.ReqId = reqId;
            return queryDisplayGroupsRequestProto;
        }

        public static protobuf.SubscribeToGroupEventsRequest createSubscribeToGroupEventsRequestProto(int reqId, int groupId)
        {
            protobuf.SubscribeToGroupEventsRequest subscribeToGroupEventsRequestProto = new protobuf.SubscribeToGroupEventsRequest();
            if (Util.IsValidValue(reqId)) subscribeToGroupEventsRequestProto.ReqId = reqId;
            if (Util.IsValidValue(groupId)) subscribeToGroupEventsRequestProto.GroupId = groupId;
            return subscribeToGroupEventsRequestProto;
        }

        public static protobuf.UpdateDisplayGroupRequest createUpdateDisplayGroupRequestProto(int reqId, string contractInfo)
        {
            protobuf.UpdateDisplayGroupRequest updateDisplayGroupRequestProto = new protobuf.UpdateDisplayGroupRequest();
            if (Util.IsValidValue(reqId)) updateDisplayGroupRequestProto.ReqId = reqId;
            if (!Util.StringIsEmpty(contractInfo)) updateDisplayGroupRequestProto.ContractInfo = contractInfo;
            return updateDisplayGroupRequestProto;
        }

        public static protobuf.UnsubscribeFromGroupEventsRequest createUnsubscribeFromGroupEventsRequestProto(int reqId)
        {
            protobuf.UnsubscribeFromGroupEventsRequest unsubscribeFromGroupEventsRequestProto = new protobuf.UnsubscribeFromGroupEventsRequest();
            if (Util.IsValidValue(reqId)) unsubscribeFromGroupEventsRequestProto.ReqId = reqId;
            return unsubscribeFromGroupEventsRequestProto;
        }

        public static protobuf.MarketDepthExchangesRequest createMarketDepthExchangesRequestProto()
        {
            protobuf.MarketDepthExchangesRequest marketDepthExchangesRequestProto = new protobuf.MarketDepthExchangesRequest();
            return marketDepthExchangesRequestProto;
        }

        public static protobuf.CancelContractData createCancelContractDataProto(int reqId)
        {
            protobuf.CancelContractData cancelContractDataProto = new protobuf.CancelContractData();
            if (Util.IsValidValue(reqId)) cancelContractDataProto.ReqId = reqId;
            return cancelContractDataProto;
        }

        public static protobuf.CancelHistoricalTicks createCancelHistoricalTicksProto(int reqId)
        {
            protobuf.CancelHistoricalTicks cancelHistoricalTicksProto = new protobuf.CancelHistoricalTicks();
            if (Util.IsValidValue(reqId)) cancelHistoricalTicksProto.ReqId = reqId;
            return cancelHistoricalTicksProto;
        }
    }
}
