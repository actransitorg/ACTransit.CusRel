using System;

namespace ACTransit.Framework.Extensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Returns Midnight of the given date.
        /// </summary>
        public static DateTime StartOfDay(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
        }

        /// <summary>
        /// Returns 11:59:59 PM of the given date
        /// </summary>
        public static DateTime EndOfDay(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 23, 59, 59);
        }

        /// <summary>
        /// Returns a time span of the difference from the supplied date until a specific date and time.  Will return 0 if the time has already passed.
        /// </summary>
        public static TimeSpan TimeUntil(this DateTime date, DateTime toDate)
        {
            return date > toDate
                ? new TimeSpan(0, 0, 0, 0)
                : toDate.Subtract(date);
        }

        public static bool IsBetween(this DateTime input, DateTime date1, DateTime date2)
        {
            return (input > date1 && input < date2);
        }
    }
}