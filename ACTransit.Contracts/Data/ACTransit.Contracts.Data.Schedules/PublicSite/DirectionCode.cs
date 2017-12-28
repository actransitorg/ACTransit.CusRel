using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ACTransit.Contracts.Data.Schedules.PublicSite
{
    /// <summary>
    ///     Active directions of scheduled line(s).
    /// </summary>
    [DataContract]
    public class DirectionCode
    {
        /// <summary>
        ///     Key (internal code) and associated Value (for Display purposes). If Value is empty, display Description instead.
        /// </summary>
        [DataMember]
        public KeyValuePair<string, string> KeyValue { get; set; }

        /// <summary>
        ///     Longer description
        /// </summary>
        [DataMember]
        public string Description { get; set; }
    }
}