using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ACTransit.Contracts.Data.Schedules.PublicSite
{
    /// <summary>
    ///     A location for travels to enter or exit a vehicle.
    /// </summary>
    [DataContract]
    public class Stop
    {
        /// <summary>
        ///     Key (internal value) and associated Value (for Display purposes)
        /// </summary>
        [DataMember]
        public KeyValuePair<int, string> KeyValue { get; set; }

        /// <summary>
        ///     Coordinate that specifies the north-south position of the stop.
        /// </summary>
        [DataMember]
        public string Latitude { get; set; }

        /// <summary>
        ///     Coordinate that specifies the east-west position of the stop.
        /// </summary>
        [DataMember]
        public string Longitude { get; set; }

        /// <summary>
        ///     Internal short code for stop
        /// </summary>
        [DataMember]
        public string Code { get; set; }

        /// <summary>
        ///     General location of stop
        /// </summary>
        [DataMember]
        public string City { get; set; }

        /// <summary>
        ///     All times a line is scheduled to stop at location.
        /// </summary>
        [DataMember]
        public virtual List<StopTime> StopTimes { get; set; }
    }
}