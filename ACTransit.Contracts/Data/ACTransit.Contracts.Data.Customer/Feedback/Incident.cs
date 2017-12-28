using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace ACTransit.Contracts.Data.Customer.Feedback
{
    /// <summary>
    ///     Incident details
    /// </summary>
    [DataContract]
    public class Incident
    {
        /// <summary>
        ///     Date and approximate time of incident
        /// </summary>
        [DataMember]
        public DateTime DateTime { get; set; }

        /// <summary>
        ///     Vehicle Number, located in front of bus, above the driver, or displayed on back of bus
        /// </summary>
        [RegularExpression(@"^\d{1,4}$", ErrorMessage = "Vehicle field must only contain numbers 4 or less in length.")]
        [DataMember]
        public string Vehicle { get; set; }

        /// <summary>
        ///     Line (or route)
        /// </summary>
        [RegularExpression(@"^[a-zA-Z0-9]{1,4}$",
            ErrorMessage = "Line field must only letters and numbers 4 or less in length.")]
        [DataMember]
        public string Line { get; set; }

        /// <summary>
        ///     Destination stop or end of the line
        /// </summary>
        [DataMember]
        public string Destination { get; set; }

        /// <summary>
        ///     Where the incident occurred
        /// </summary>
        [DataMember]
        public string Location { get; set; }
    }
}