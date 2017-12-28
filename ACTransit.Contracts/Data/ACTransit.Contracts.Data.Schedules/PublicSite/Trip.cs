using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ACTransit.Contracts.Data.Schedules.PublicSite
{
    /// <summary>
    ///     The path (places and times) a particular vehicle is scheduled to run along.
    /// </summary>
    [DataContract]
    public class Trip
    {
        public Trip()
        {
            Stops = new List<Stop>();
        }

        /// <summary>
        ///     Key (internal value) and associated Value (for Display purposes)
        /// </summary>
        [DataMember]
        public KeyValuePair<int, string> KeyValue { get; set; }

        /// <summary>
        ///     Locations and times for all stops within a scheduled trip.
        /// </summary>
        [DataMember]
        public virtual List<Stop> Stops { get; set; }
    }
}