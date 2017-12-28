using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ACTransit.Framework.Algorithm.Sort;

namespace ACTransit.Contracts.Data.Schedules.PublicSite
{
    /// <summary>
    ///     Individual transit line
    /// </summary>
    [DataContract]
    public class Line : IComparable<Line>
    {
        /// <summary>
        ///     Key (internal value) and associated Value (for Display purposes)
        /// </summary>
        [DataMember]
        public KeyValuePair<int, string> KeyValue { get; set; }

        /// <summary>
        ///     Routes and schedule information.
        /// </summary>
        [DataMember]
        public virtual List<Route> Routes { get; set; }

        /// <summary>
        ///     Line category
        /// </summary>
        [DataMember]
        public int? CategoryKey { get; set; }


        /// <summary>
        ///     Comparison for sorting
        /// </summary>
        /// <param name="otherLine"></param>
        /// <returns>-1 (less than), 0 (equals), +1 (greater than)</returns>
        public int CompareTo(Line otherLine)
        {
            return FastAlphaNumericComparer.CompareFast(KeyValue.Value, otherLine.KeyValue.Value);
        }
    }
}