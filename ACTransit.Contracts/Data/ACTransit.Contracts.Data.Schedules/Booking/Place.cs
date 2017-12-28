using System;
using System.Runtime.Serialization;

namespace ACTransit.Contracts.Data.Schedules.Booking
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class Place
    {
        [DataMember]
        public string PlaceId { get; set; }
        [DataMember]
        public string PlaceReferenceId { get; set; }
        [DataMember]
        public string PlaceShortDescription { get; set; }
        [DataMember]
        public string PlaceDescription { get; set; }
        [DataMember]
        public Nullable<int> DistrictId { get; set; }
        [DataMember]
        public Nullable<decimal> Longitude { get; set; }
        [DataMember]
        public Nullable<decimal> Latitude { get; set; }
        [DataMember]
        public Nullable<System.DateTime> ValidToDate { get; set; }
    }
}
