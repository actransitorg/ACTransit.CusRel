using System;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace ACTransit.Contracts.Data.Schedules.PublicSite
{
    /// <summary>
    ///     Time at stop
    /// </summary>
    [DataContract]
    public class StopTime
    {
        /// <summary>
        ///     Stop time (ignore date)
        /// </summary>
        [DataMember]
        public DateTime? Time { get; set; }

        /// <summary>
        ///     TripId for reference (stop times only occur during trips)
        /// </summary>
        [DataMember]
        public int TripId { get; set; }
    }


    /// <summary>
    ///     Stop time utilities
    /// </summary>
    public static class StopTimeHelper
    {
        public static Regex stopTimeRe = new Regex(@"^(?<h>\d{1,2})(?<m>\d{2})(?<ampm>[a|p])$", RegexOptions.Compiled);

        public static DateTime? ToDateTime(this string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            value = value.Trim();
            var match = stopTimeRe.Match(value);
            if (!match.Success) return null;
            var time = match.Groups[1].Value + ":" + match.Groups[2].Value + " " +
                       (match.Groups[3].Value == "a" ? "AM" : "PM");
            var result = DateTime.MinValue;
            DateTime.TryParse(time, out result);
            return result;
        }
    }
}