using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;

namespace ACTransit.Contracts.Data.Schedules.PublicSite
{
    /// <summary>
    ///     Route of transit line with optional routes, stops and daily trips
    /// </summary>
    [DataContract]
    public class Route
    {
        /// <summary>
        ///     Key (internal value) and associated Value (for Display purposes)
        /// </summary>
        [DataMember]
        public KeyValuePair<int, string> KeyValue { get; set; }

        /// <summary>
        ///     Direction of scheduled route or trip.
        /// </summary>
        [DataMember]
        public DirectionCode DirectionCode { get; set; }

        /// <summary>
        ///     Day(s) of week of schedule route or trip.
        /// </summary>
        [DataMember]
        public DayCode DayCode { get; set; }

        /// <summary>
        ///     All the stops on a line.
        /// </summary>
        [DataMember]
        public virtual List<Stop> Stops { get; set; }

        /// <summary>
        ///     All the scheduled stops for all the vehicles of a transit line route.
        /// </summary>
        [DataMember]
        public virtual List<Trip> Trips { get; set; }

        /// <summary>
        ///     Order to display
        /// </summary>
        [DataMember]
        public int SortOrder { get; set; }

        /// <summary>
        ///     Get Route's Schedule (rows = individual vehicle trip, columns = stop location, cells = stop time)
        /// </summary>
        /// <returns>Rows and columns of strings</returns>
        [DataMember]
        public DataTable Schedule { get; set; }
    }
}