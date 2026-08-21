/* Copyright (C) 2025 Interactive Brokers LLC. All rights reserved. This code is subject to the terms
 * and conditions of the IB API Non-Commercial License or the IB API Commercial License, as applicable. */

using System.Collections.Generic;

namespace IBApi
{
    /**
     * @class ExecutionFilter
     * @brief when requesting executions, a filter can be specified to receive only a subset of them
     * @sa Contract, Execution, CommissionAndFeesReport
     */
    public class ExecutionFilter
    {
        /**
         * @brief The API client which placed the order
         */
        public int ClientId { get; set; }

        /**
        * @brief The account to which the order was allocated to
        */
        public string AcctCode { get; set; }

        /**
         * @brief Time from which the executions will be returned yyyymmdd hh:mm:ss
         * Only those executions reported after the specified time will be returned.
         */
        public string Time { get; set; }

        /**
        * @brief The instrument's symbol
        */
        public string Symbol { get; set; }

        /**
         * @brief The Contract's security's type (i.e. STK, OPT...)
         */
        public string SecType { get; set; }

        /**
         * @brief The exchange at which the execution was produced
         */
        public string Exchange { get; set; }

        /**
        * @brief The Contract's side (BUY or SELL)
        */
        public string Side { get; set; }

        /**
         * @brief Last N days for which the request is sent
         */
        public int LastNDays { get; set; }

        /**
         * @brief List of specific dates for which the request is sent
         */
        public List<int> SpecificDates { get; set; }

        public ExecutionFilter()
        {
            ClientId = 0;
            LastNDays = int.MaxValue;
        }

        public ExecutionFilter(int clientId, string acctCode, string time, string symbol, string secType, string exchange, string side, int lastNDays, List<int> specificDates)
        {
            ClientId = clientId;
            AcctCode = acctCode;
            Time = time;
            Symbol = symbol;
            SecType = secType;
            Exchange = exchange;
            Side = side;
            LastNDays = lastNDays;
            SpecificDates = specificDates;
        }

        public override bool Equals(object other)
        {
            bool l_bRetVal;
            if (!(other is ExecutionFilter l_theOther))
            {
                l_bRetVal = false;
            }
            else if (this == other)
            {
                l_bRetVal = true;
            }
            else
            {
                if (!Util.VectorEqualsUnordered(SpecificDates, l_theOther.SpecificDates))
                {
                    return false;
                }

                l_bRetVal = ClientId == l_theOther.ClientId &&
                            string.Equals(AcctCode, l_theOther.AcctCode, System.StringComparison.OrdinalIgnoreCase) &&
                            string.Equals(Time, l_theOther.Time, System.StringComparison.OrdinalIgnoreCase) &&
                            string.Equals(Symbol, l_theOther.Symbol, System.StringComparison.OrdinalIgnoreCase) &&
                            string.Equals(SecType, l_theOther.SecType, System.StringComparison.OrdinalIgnoreCase) &&
                            string.Equals(Exchange, l_theOther.Exchange, System.StringComparison.OrdinalIgnoreCase) &&
                            string.Equals(Side, l_theOther.Side, System.StringComparison.OrdinalIgnoreCase) &&
                            LastNDays == l_theOther.LastNDays;
            }
            return l_bRetVal;
        }

        public override int GetHashCode()
        {
            var hashCode = 82934527;
            hashCode *= -1521134295 + ClientId.GetHashCode();
            hashCode *= -1521134295 + EqualityComparer<string>.Default.GetHashCode(AcctCode);
            hashCode *= -1521134295 + EqualityComparer<string>.Default.GetHashCode(Time);
            hashCode *= -1521134295 + EqualityComparer<string>.Default.GetHashCode(Symbol);
            hashCode *= -1521134295 + EqualityComparer<string>.Default.GetHashCode(SecType);
            hashCode *= -1521134295 + EqualityComparer<string>.Default.GetHashCode(Exchange);
            hashCode *= -1521134295 + EqualityComparer<string>.Default.GetHashCode(Side);
            hashCode *= -1521134295 + LastNDays.GetHashCode();
            return hashCode;
        }
    }
}
