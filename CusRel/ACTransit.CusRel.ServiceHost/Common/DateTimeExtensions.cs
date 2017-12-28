using System;
using System.Globalization;

namespace ACTransit.CusRel.ServiceHost.Common
{
    public static class DateTimeExtensions
    {
        private static readonly string[] Formats = { Config.Instance.DateFormat, Config.Instance.DateFormat + Config.Instance.TimeFormat };

        public static DateTime? AsDateTime(this string dateTime)
        {
            DateTime result;
            return DateTime.TryParseExact(dateTime, Formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out result)
                ? (DateTime?)result
                : null;
        }
    }
}
