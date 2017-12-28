using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ACTransit.Contracts.Data.Schedules.PublicSite
{
    /// <summary>
    ///     Common category for grouping transit lines.
    /// </summary>
    [DataContract]
    public class LineCategory
    {
        /// <summary>
        ///     Key (internal value) and associated Value (for Display purposes)
        /// </summary>
        [DataMember]
        public KeyValuePair<int, string> KeyValue { get; set; }

        /// <summary>
        ///     Is category enabled?  Do not display if not
        /// </summary>
        [DataMember]
        public bool IsEnabled { get; set; }

        /// <summary>
        ///     Lines within given category.
        /// </summary>
        [DataMember]
        public virtual List<Line> Lines { get; set; }
    }

    /// <summary>
    ///     All categories for grouping transit lines
    /// </summary>
    [DataContract]
    public class LineCategories : List<LineCategory>
    {
        public LineCategories()
        {
            Add(new LineCategory {KeyValue = new KeyValuePair<int, string>(0, "Select one..."), IsEnabled = true});
            //Add(new LineCategory { KeyValue = new KeyValuePair<int, string>(1, "By City"), IsEnabled = false });
            Add(new LineCategory {KeyValue = new KeyValuePair<int, string>(2, "Local"), IsEnabled = true});
            Add(new LineCategory {KeyValue = new KeyValuePair<int, string>(3, "Transbay"), IsEnabled = true});
            Add(new LineCategory {KeyValue = new KeyValuePair<int, string>(4, "All Nighter"), IsEnabled = true});
            Add(new LineCategory {KeyValue = new KeyValuePair<int, string>(5, "School"), IsEnabled = true});
            Add(new LineCategory {KeyValue = new KeyValuePair<int, string>(6, "All Lines"), IsEnabled = true});
        }
    }
}