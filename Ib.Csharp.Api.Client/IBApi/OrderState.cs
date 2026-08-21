/* Copyright (C) 2024 Interactive Brokers LLC. All rights reserved. This code is subject to the terms
 * and conditions of the IB API Non-Commercial License or the IB API Commercial License, as applicable. */

using System;
using System.Collections.Generic;
using System.Data;

namespace IBApi
{
    /**
     * @class OrderState
     * @brief Provides an active order's current state
     * @sa Order
     */
    public class OrderState
    {
        /**
         * @brief The order's current status
         */
        public string Status { get; set; }

        /**
         * @brief The account's current initial margin.
         */
        public string InitMarginBefore { get; set; }

        /**
        * @brief The account's current maintenance margin
        */
        public string MaintMarginBefore { get; set; }

        /**
        * @brief The account's current equity with loan
        */
        public string EquityWithLoanBefore { get; set; }

        /**
         * @brief The change of the account's initial margin.
         */
        public string InitMarginChange { get; set; }

        /**
        * @brief The change of the account's maintenance margin
        */
        public string MaintMarginChange { get; set; }

        /**
        * @brief The change of the account's equity with loan
        */
        public string EquityWithLoanChange { get; set; }

        /**
         * @brief The order's impact on the account's initial margin.
         */
        public string InitMarginAfter { get; set; }

        /**
        * @brief The order's impact on the account's maintenance margin
        */
        public string MaintMarginAfter { get; set; }

        /**
        * @brief Shows the impact the order would have on the account's equity with loan
        */
        public string EquityWithLoanAfter { get; set; }

        /**
          * @brief The order's generated commission and fees.
          */
        public double CommissionAndFees { get; set; }

        /**
        * @brief The execution's minimum commission and fees.
        */
        public double MinCommissionAndFees { get; set; }

        /**
        * @brief The executions maximum commission and fees.
        */
        public double MaxCommissionAndFees { get; set; }

        /**
         * @brief The generated commission and fees currency
         * @sa CommissionAndFeesReport
         */
        public string CommissionAndFeesCurrency { get; set; }

        /**
         * @brief Margin currency
         */
        public string MarginCurrency { get; set; }

        /**
         * @brief The account's current initial margin outside RTH
         */
        public double InitMarginBeforeOutsideRTH { get; set; }

        /**
        * @brief The account's current maintenance margin outside RTH
        */
        public double MaintMarginBeforeOutsideRTH { get; set; }

        /**
        * @brief The account's current equity with loan outside RTH
        */
        public double EquityWithLoanBeforeOutsideRTH { get; set; }

        /**
         * @brief The change of the account's initial margin outside RTH
         */
        public double InitMarginChangeOutsideRTH { get; set; }

        /**
        * @brief The change of the account's maintenance margin outside RTH
        */
        public double MaintMarginChangeOutsideRTH { get; set; }

        /**
        * @brief The change of the account's equity with loan outside RTH
        */
        public double EquityWithLoanChangeOutsideRTH { get; set; }

        /**
         * @brief The order's impact on the account's initial margin outside RTH
         */
        public double InitMarginAfterOutsideRTH { get; set; }

        /**
        * @brief The order's impact on the account's maintenance margin outside RTH
        */
        public double MaintMarginAfterOutsideRTH { get; set; }

        /**
        * @brief Shows the impact the order would have on the account's equity with loan outside RTH
        */
        public double EquityWithLoanAfterOutsideRTH { get; set; }

        /**
        * @brief Suggested size
        */
        public decimal SuggestedSize { get; set; }

        /**
        * @brief Reject reason
        */
        public string RejectReason { get; set; }

        /**
        * @brief Order allocations
        */
        public List<OrderAllocation> OrderAllocations { get; set; } = new List<OrderAllocation>();

        /**
         * @brief If the order is warranted, a descriptive message will be provided.
         */
        public string WarningText { get; set; }

        public string CompletedTime { get; set; }

        public string CompletedStatus { get; set; }

        public OrderState()
        {
            Status = null;
            InitMarginBefore = null;
            MaintMarginBefore = null;
            EquityWithLoanBefore = null;
            InitMarginChange = null;
            MaintMarginChange = null;
            EquityWithLoanChange = null;
            InitMarginAfter = null;
            MaintMarginAfter = null;
            EquityWithLoanAfter = null;
            CommissionAndFees = 0.0;
            MinCommissionAndFees = 0.0;
            MaxCommissionAndFees = 0.0;
            CommissionAndFeesCurrency = null;
            MarginCurrency = "";
            InitMarginBeforeOutsideRTH = double.MaxValue;
            MaintMarginBeforeOutsideRTH = double.MaxValue;
            EquityWithLoanBeforeOutsideRTH = double.MaxValue;
            InitMarginChangeOutsideRTH = double.MaxValue;
            MaintMarginChangeOutsideRTH = double.MaxValue;
            EquityWithLoanChangeOutsideRTH = double.MaxValue;
            InitMarginAfterOutsideRTH = double.MaxValue;
            MaintMarginAfterOutsideRTH = double.MaxValue;
            EquityWithLoanAfterOutsideRTH = double.MaxValue;
            SuggestedSize = decimal.MaxValue;
            RejectReason = "";
            WarningText = null;
            CompletedTime = null;
            CompletedStatus = null;
        }

        public override bool Equals(object other)
        {
            if (this == other)
                return true;


            if (!(other is OrderState state))
                return false;

            if (CommissionAndFees != state.CommissionAndFees ||
                MinCommissionAndFees != state.MinCommissionAndFees ||
                MaxCommissionAndFees != state.MaxCommissionAndFees ||
                InitMarginBeforeOutsideRTH != state.InitMarginBeforeOutsideRTH ||
                MaintMarginBeforeOutsideRTH != state.MaintMarginBeforeOutsideRTH ||
                EquityWithLoanBeforeOutsideRTH != state.EquityWithLoanBeforeOutsideRTH ||
                InitMarginChangeOutsideRTH != state.InitMarginChangeOutsideRTH ||
                MaintMarginChangeOutsideRTH != state.MaintMarginChangeOutsideRTH ||
                EquityWithLoanChangeOutsideRTH != state.EquityWithLoanChangeOutsideRTH ||
                InitMarginAfterOutsideRTH != state.InitMarginAfterOutsideRTH ||
                MaintMarginAfterOutsideRTH != state.MaintMarginAfterOutsideRTH ||
                EquityWithLoanAfterOutsideRTH != state.EquityWithLoanAfterOutsideRTH ||
                SuggestedSize != state.SuggestedSize)
            {
                return false;
            }

            if (Util.StringCompare(Status, state.Status) != 0 ||
                Util.StringCompare(InitMarginBefore, state.InitMarginBefore) != 0 ||
                Util.StringCompare(MaintMarginBefore, state.MaintMarginBefore) != 0 ||
                Util.StringCompare(EquityWithLoanBefore, state.EquityWithLoanBefore) != 0 ||
                Util.StringCompare(InitMarginChange, state.InitMarginChange) != 0 ||
                Util.StringCompare(MaintMarginChange, state.MaintMarginChange) != 0 ||
                Util.StringCompare(EquityWithLoanChange, state.EquityWithLoanChange) != 0 ||
                Util.StringCompare(InitMarginAfter, state.InitMarginAfter) != 0 ||
                Util.StringCompare(MaintMarginAfter, state.MaintMarginAfter) != 0 ||
                Util.StringCompare(EquityWithLoanAfter, state.EquityWithLoanAfter) != 0 ||
                Util.StringCompare(CommissionAndFeesCurrency, state.CommissionAndFeesCurrency) != 0 ||
                Util.StringCompare(MarginCurrency, state.MarginCurrency) != 0 ||
                Util.StringCompare(RejectReason, state.RejectReason) != 0 ||
                Util.StringCompare(CompletedTime, state.CompletedTime) != 0 ||
                Util.StringCompare(CompletedStatus, state.CompletedStatus) != 0)
            {
                return false;
            }

            if (!Util.VectorEqualsUnordered(OrderAllocations, state.OrderAllocations))
            {
                return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 1754944475;
            hashCode *= -1521134295 + EqualityComparer<string>.Default.GetHashCode(Status);
            hashCode *= -1521134295 + EqualityComparer<string>.Default.GetHashCode(InitMarginBefore);
            hashCode *= -1521134295 + EqualityComparer<string>.Default.GetHashCode(MaintMarginBefore);
            hashCode *= -1521134295 + EqualityComparer<string>.Default.GetHashCode(EquityWithLoanBefore);
            hashCode *= -1521134295 + EqualityComparer<string>.Default.GetHashCode(InitMarginChange);
            hashCode *= -1521134295 + EqualityComparer<string>.Default.GetHashCode(MaintMarginChange);
            hashCode *= -1521134295 + EqualityComparer<string>.Default.GetHashCode(EquityWithLoanChange);
            hashCode *= -1521134295 + EqualityComparer<string>.Default.GetHashCode(InitMarginAfter);
            hashCode *= -1521134295 + EqualityComparer<string>.Default.GetHashCode(MaintMarginAfter);
            hashCode *= -1521134295 + EqualityComparer<string>.Default.GetHashCode(EquityWithLoanAfter);
            hashCode *= -1521134295 + CommissionAndFees.GetHashCode();
            hashCode *= -1521134295 + MinCommissionAndFees.GetHashCode();
            hashCode *= -1521134295 + MaxCommissionAndFees.GetHashCode();
            hashCode *= -1521134295 + EqualityComparer<string>.Default.GetHashCode(CommissionAndFeesCurrency);
            hashCode *= -1521134295 + EqualityComparer<string>.Default.GetHashCode(MarginCurrency);
            hashCode *= -1521134295 + InitMarginBeforeOutsideRTH.GetHashCode();
            hashCode *= -1521134295 + MaintMarginBeforeOutsideRTH.GetHashCode();
            hashCode *= -1521134295 + EquityWithLoanBeforeOutsideRTH.GetHashCode();
            hashCode *= -1521134295 + InitMarginChangeOutsideRTH.GetHashCode();
            hashCode *= -1521134295 + MaintMarginChangeOutsideRTH.GetHashCode();
            hashCode *= -1521134295 + EquityWithLoanChangeOutsideRTH.GetHashCode();
            hashCode *= -1521134295 + InitMarginAfterOutsideRTH.GetHashCode();
            hashCode *= -1521134295 + MaintMarginAfterOutsideRTH.GetHashCode();
            hashCode *= -1521134295 + EquityWithLoanAfterOutsideRTH.GetHashCode();
            hashCode *= -1521134295 + SuggestedSize.GetHashCode();
            hashCode *= -1521134295 + EqualityComparer<string>.Default.GetHashCode(RejectReason);
            hashCode *= -1521134295 + EqualityComparer<string>.Default.GetHashCode(WarningText);
            hashCode *= -1521134295 + EqualityComparer<string>.Default.GetHashCode(CompletedTime);
            hashCode *= -1521134295 + EqualityComparer<string>.Default.GetHashCode(CompletedStatus);
            return hashCode;
        }
    }
}
