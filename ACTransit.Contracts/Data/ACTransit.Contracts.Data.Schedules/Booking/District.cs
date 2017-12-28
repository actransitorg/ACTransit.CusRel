using System;
using System.Runtime.Serialization;

namespace ACTransit.Contracts.Data.Schedules.Booking
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class District
    {
        [DataMember]

        public int DistrictId { get; set; }
        [DataMember]
        public string DistrictDescription { get; set; }
        [DataMember]
        public Nullable<System.DateTime> ValidToDate { get; set; }
    }
}
