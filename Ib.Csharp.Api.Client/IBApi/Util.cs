/* Copyright (C) 2025 Interactive Brokers LLC. All rights reserved. This code is subject to the terms
 * and conditions of the IB API Non-Commercial License or the IB API Commercial License, as applicable. */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Google.Protobuf;

namespace IBApi
{
    public static class Util
    {
        public static bool IsValidValue(double value) => value != double.MaxValue && !Double.IsNaN(value) && !Double.IsInfinity(value);

        public static bool IsValidValue(int value) => value != int.MaxValue;

        public static bool IsValidValue(long value) => value != long.MaxValue;

        public static bool IsValidValue(Decimal value) => value != decimal.MaxValue;

        public static bool StringIsEmpty(string str) => string.IsNullOrEmpty(str);

        public static string NormalizeString(string str) => str ?? string.Empty;

        public static int StringCompare(string lhs, string rhs) => NormalizeString(lhs).CompareTo(NormalizeString(rhs));

        public static int StringCompareIgnCase(string lhs, string rhs)
        {
            var normalisedLhs = NormalizeString(lhs);
            var normalisedRhs = NormalizeString(rhs);
            return string.Compare(normalisedLhs, normalisedRhs, true);
        }

        public static bool VectorEqualsUnordered<T>(List<T> lhs, List<T> rhs)
        {
            if (lhs == rhs)
                return true;

            var lhsCount = lhs?.Count ?? 0;
            var rhsCount = rhs?.Count ?? 0;

            if (lhsCount != rhsCount)
                return false;

            if (lhsCount == 0)
                return true;

            var matchedRhsElems = new bool[rhsCount];

            for (var lhsIdx = 0; lhsIdx < lhsCount; ++lhsIdx)
            {
                object lhsElem = lhs[lhsIdx];
                var rhsIdx = 0;
                for (; rhsIdx < rhsCount; ++rhsIdx)
                {
                    if (matchedRhsElems[rhsIdx])
                    {
                        continue;
                    }
                    if (lhsElem.Equals(rhs[rhsIdx]))
                    {
                        matchedRhsElems[rhsIdx] = true;
                        break;
                    }
                }
                if (rhsIdx >= rhsCount)
                {
                    // no matching elem found
                    return false;
                }
            }

            return true;
        }

        public static string IntMaxString(int value) => value == int.MaxValue ? string.Empty : string.Empty + value;

        public static string LongMaxString(long value) => value == long.MaxValue ? string.Empty : string.Empty + value;

        public static string DoubleMaxString(double value) => DoubleMaxString(value, string.Empty);

        public static string DoubleMaxString(double value, string def) => value != double.MaxValue ? value.ToString(NumberFormatInfo.InvariantInfo) : def;

        public static string DecimalMaxString(decimal value) => value == decimal.MaxValue ? string.Empty : string.Empty + value;

        public static string DecimalMaxStringNoZero(decimal value) => value == decimal.MaxValue || value == 0 ? string.Empty : string.Empty + value;

        public static string UnixSecondsToString(long seconds, string format) => new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(Convert.ToDouble(seconds)).ToLocalTime().ToString(format);

        public static string UnixMilliSecondsToString(long milliSeconds, string format) => new DateTime(1970, 1, 1, 0, 0, 0).AddMilliseconds(Convert.ToDouble(milliSeconds)).ToLocalTime().ToString(format);

        public static string formatDoubleString(string str) => string.IsNullOrEmpty(str) ? string.Empty : DoubleMaxString(double.Parse(str));

        public static string TagValueListToString(List<TagValue> options)
        {
            var tagValuesStr = new StringBuilder();
            var tagValuesCount = options?.Count ?? 0;

            for (var i = 0; i < tagValuesCount; i++)
            {
                var tagValue = options[i];
                tagValuesStr.Append(tagValue.Tag).Append('=').Append(tagValue.Value).Append(';');
            }

            return tagValuesStr.ToString();
        }

        public static List<TagValue> StringToTagValueList(string tagValueString)
        {
            var tagValueList = new List<TagValue>();

            if (string.IsNullOrEmpty(tagValueString))
                return tagValueList;

            var pairs = tagValueString.Split(';');
            foreach (var pair in pairs)
            {
                if (string.IsNullOrEmpty(pair))
                    continue;

                var parts = pair.Split(',');
                if (parts.Length == 2)
                {
                    tagValueList.Add(new TagValue(parts[0], parts[1]));
                }
            }

            return tagValueList;
        }

        public static decimal StringToDecimal(string str) => !string.IsNullOrEmpty(str) && !str.Equals("9223372036854775807") && !str.Equals("2147483647") && !str.Equals("1.7976931348623157E308") && !str.Equals("-9223372036854775808")  ? decimal.Parse(str, NumberFormatInfo.InvariantInfo) : decimal.MaxValue;

        public static decimal GetDecimal(object value) => Convert.ToDecimal(((IEnumerable)value).Cast<object>().ToArray()[0]);

        public static bool IsVolOrder(string orderType) => orderType.Equals("VOL") || orderType.Equals("VOLATILITY") || orderType.Equals("VOLAT");

        public static bool IsPegBenchOrder(string orderType) => orderType.Equals("PEG BENCH") || orderType.Equals("PEGBENCH");

        public static bool IsPegMidOrder(string orderType) => orderType.Equals("PEG MID") || orderType.Equals("PEGMID");

        public static bool IsPegBestOrder(string orderType) => orderType.Equals("PEG BEST") || orderType.Equals("PEGBEST");

        public static long CurrentTimeMillis() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        public static double StringToDoubleMax(string number) => string.IsNullOrEmpty(number) ? double.MaxValue : double.Parse(number);

        public static int StringToIntMax(string number) => string.IsNullOrEmpty(number) ? int.MaxValue : int.Parse(number);

        public static void printProtoSingleLine(String header, IMessage message)
        {
            Console.WriteLine(header + JsonFormatter.Default.Format(message));
        }
    }
}
