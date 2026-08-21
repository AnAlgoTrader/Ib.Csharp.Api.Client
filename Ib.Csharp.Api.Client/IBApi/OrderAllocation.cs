/* Copyright (C) 2024 Interactive Brokers LLC. All rights reserved. This code is subject to the terms
 * and conditions of the IB API Non-Commercial License or the IB API Commercial License, as applicable. */

using System;

namespace IBApi
{
    /**
     * @class OrderAllocation
     * @brief allocation of order
     * @sa OrderState
     */
    public class OrderAllocation
    {
        /**
         * @brief allocation account
         */
        public string Account { get; set; }

        /**
         * @brief position
         */
        public decimal Position { get; set; }

        /**
         * @brief desired position
         */
        public decimal PositionDesired { get; set; }

        /**
         * @brief position after
         */
        public decimal PositionAfter { get; set; }

        /**
         * @brief desired allocation quantity
         */
        public decimal DesiredAllocQty { get; set; }

        /**
         * @brief allowed allocation quantity
         */
        public decimal AllowedAllocQty { get; set; }

        /**
         * @brief is monetary
         */
        public bool IsMonetary { get; set; }

        public OrderAllocation()
        {
            Account = "";
            Position = decimal.MaxValue;
            PositionDesired = decimal.MaxValue;
            PositionAfter = decimal.MaxValue;
            DesiredAllocQty = decimal.MaxValue;
            AllowedAllocQty = decimal.MaxValue;
            IsMonetary = false;
        }

        public override bool Equals(object other)
        {
            if (!(other is OrderAllocation theOther))
            {
                return false;
            }

            if (this == other)
            {
                return true;
            }

            return Account == theOther.Account;
        }

        public override int GetHashCode() => -814345894 + Account.GetHashCode();
    }
}
