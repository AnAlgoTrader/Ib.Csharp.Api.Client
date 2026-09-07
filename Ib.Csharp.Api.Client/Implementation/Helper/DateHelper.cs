using System;
using System.Linq;
using System.Threading.Tasks;
using IBApi;

namespace Ib.Csharp.Api.Client.Implementation.Helper;

public static class DateHelper
{
    private const string IbkrDateFormat = "yyyyMMdd";

    public static DateTime GetNextWednesday(DateTime fromDate)
    {
        var daysToAdd = ((int)DayOfWeek.Wednesday - (int)fromDate.DayOfWeek + 7) % 7;
        if (daysToAdd == 0) daysToAdd = 7; // Skip today if it's Wednesday
        return fromDate.AddDays(daysToAdd);
    }

    public static string GetNextTradingDay(int daysInFuture)
    {
        var today = DateTime.Today;
        var target = today.AddDays(daysInFuture);
        while (IsWeekendOrHoliday(target))
            target = target.AddDays(1);
        return target.ToString(IbkrDateFormat);
    }

    public static string GetPreviousTradingDay()
    {
        var date = DateTime.Today.AddDays(-1);
        while (IsWeekendOrHoliday(date))
            date = date.AddDays(-1);
        return date.ToString(IbkrDateFormat);
    }

    /// <summary>
    /// Calculates the number of business days between two dates (excluding weekends and US market holidays).
    /// </summary>
    /// <param name="startDate">The start date (inclusive)</param>
    /// <param name="endDate">The end date (exclusive)</param>
    /// <returns>Number of business days</returns>
    public static int GetBusinessDaysBetween(DateTime startDate, DateTime endDate)
    {
        if (endDate <= startDate)
            return 0;

        int businessDays = 0;
        var current = startDate.Date;

        while (current < endDate.Date)
        {
            if (!IsWeekendOrHoliday(current))
            {
                businessDays++;
            }

            current = current.AddDays(1);
        }

        return businessDays;
    }

    /// <summary>
    /// Checks if a given date is a weekend or a US financial market holiday
    /// </summary>
    /// <param name="date">The date to check</param>
    /// <returns>True if the date is a weekend or a US financial market holiday, false otherwise</returns>
    private static bool IsWeekendOrHoliday(DateTime date)
    {
        // Check for weekends (Saturday and Sunday)
        if ((int)date.DayOfWeek == 6 || (int)date.DayOfWeek == 0) // 6 = Saturday, 0 = Sunday
        {
            return true;
        }

        // Check for US financial market holidays
        return IsUsFinancialHoliday(date);
    }

    /// <summary>
    /// Checks if a given date is a US financial market holiday
    /// </summary>
    /// <param name="date">The date to check</param>
    /// <returns>True if the date is a US financial market holiday, false otherwise</returns>
    private static bool IsUsFinancialHoliday(DateTime date)
    {
        int year = date.Year;
        int month = date.Month;
        int day = date.Day;

        // New Year's Day (might fall on weekend and be observed on Monday)
        if (month == 1 && day == 1) return true;
        if (month == 1 && day == 2 && date.DayOfWeek == DayOfWeek.Monday) return true;
        if (month == 12 && day == 31 && date.DayOfWeek == DayOfWeek.Friday) return true;

        // Martin Luther King Jr. Day (third Monday in January)
        if (month == 1)
        {
            var mlkDay = GetNthOccurrenceOfMonth(year, 1, DayOfWeek.Monday, 3);
            if (date.Date == mlkDay.Date) return true;
        }

        // Presidents' Day (third Monday in February)
        if (month == 2)
        {
            var presidentsDay = GetNthOccurrenceOfMonth(year, 2, DayOfWeek.Monday, 3);
            if (date.Date == presidentsDay.Date) return true;
        }

        // Memorial Day (last Monday in May)
        if (month == 5)
        {
            var memorialDay = GetLastOccurrenceOfMonth(year, 5, DayOfWeek.Monday);
            if (date.Date == memorialDay.Date) return true;
        }

        // Juneteenth National Independence Day (June 19, added as federal holiday in 2022)
        if (month == 6 && day == 19) return true;
        if (month == 6 && day == 20 && date.DayOfWeek == DayOfWeek.Monday) return true; // Observed on Monday if June 19 falls on weekend
        if (month == 6 && day == 18 && date.DayOfWeek == DayOfWeek.Friday) return true; // Observed on Friday if June 19 falls on Saturday

        // Independence Day (July 4)
        if (month == 7 && day == 4) return true;
        if (month == 7 && day == 5 && date.DayOfWeek == DayOfWeek.Monday) return true; // Observed on Monday if July 4 falls on weekend
        if (month == 7 && day == 3 && date.DayOfWeek == DayOfWeek.Friday) return true; // Observed on Friday if July 4 falls on Saturday

        // Labor Day (first Monday in September)
        if (month == 9)
        {
            var laborDay = GetNthOccurrenceOfMonth(year, 9, DayOfWeek.Monday, 1);
            if (date.Date == laborDay.Date) return true;
        }

        // Thanksgiving Day (fourth Thursday in November)
        if (month == 11)
        {
            var thanksgiving = GetNthOccurrenceOfMonth(year, 11, DayOfWeek.Thursday, 4);
            if (date.Date == thanksgiving.Date) return true;
        }

        // Christmas Day (December 25)
        if (month == 12 && day == 25) return true;
        if (month == 12 && day == 26 && date.DayOfWeek == DayOfWeek.Monday) return true; // Observed on Monday if Christmas falls on weekend
        if (month == 12 && day == 24 && date.DayOfWeek == DayOfWeek.Friday) return true; // Observed on Friday if Christmas falls on Saturday

        return false;
    }

    /// <summary>
    /// Gets the nth occurrence of a specific day of week in a given month and year
    /// </summary>
    /// <param name="year">The year</param>
    /// <param name="month">The month (1-12)</param>
    /// <param name="dayOfWeek">The day of week to find</param>
    /// <param name="occurrence">Which occurrence (1st, 2nd, 3rd, etc.)</param>
    /// <returns>The date of the nth occurrence</returns>
    private static DateTime GetNthOccurrenceOfMonth(int year, int month, DayOfWeek dayOfWeek, int occurrence)
    {
        var firstDay = new DateTime(year, month, 1);

        // Find the first occurrence of the specified day of week in the month
        int daysToAdd = ((int)dayOfWeek - (int)firstDay.DayOfWeek + 7) % 7;
        var firstOccurrence = firstDay.AddDays(daysToAdd);

        // Calculate the nth occurrence
        return firstOccurrence.AddDays((occurrence - 1) * 7);
    }

    /// <summary>
    /// Gets the last occurrence of a specific day of week in a given month and year
    /// </summary>
    /// <param name="year">The year</param>
    /// <param name="month">The month (1-12)</param>
    /// <param name="dayOfWeek">The day of week to find</param>
    /// <returns>The date of the last occurrence</returns>
    private static DateTime GetLastOccurrenceOfMonth(int year, int month, DayOfWeek dayOfWeek)
    {
        var lastDay = new DateTime(year, month, 1).AddMonths(1).AddDays(-1); // Last day of the month

        // Move backwards to find the last occurrence of the specified day of week
        while (lastDay.DayOfWeek != dayOfWeek)
        {
            lastDay = lastDay.AddDays(-1);
        }

        return lastDay;
    }

    public static DateTime IbkrTimeToLocalTime(long ibkrTime)
    {
        var utcTime = DateTimeOffset.FromUnixTimeSeconds(ibkrTime);
        return utcTime.LocalDateTime;
    }

    public static string DateToIbkrDataFormat(DateTime dateTime)
    {
        return dateTime.ToString("yyyyMMdd-HH:mm:ss");
    }
    
    public static long ToTradingViewTime(DateTime dateTime)
    {
        // 1. Ensure the DateTime is treated as UTC to prevent local time zone offsets from skewing the timestamp
        var utcDateTime = dateTime.Kind == DateTimeKind.Utc 
            ? dateTime 
            : dateTime.ToUniversalTime();

        // 2. Convert to DateTimeOffset and extract total milliseconds since 1970-01-01
        var dto = new DateTimeOffset(utcDateTime);
        return dto.ToUnixTimeMilliseconds();
    }
}