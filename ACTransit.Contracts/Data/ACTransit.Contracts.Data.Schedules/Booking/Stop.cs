using System;
using System.Runtime.Serialization;

namespace ACTransit.Contracts.Data.Schedules.Booking
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]

    public class Stop
    {
        [DataMember]
        public string StopId { get; set; }
        [DataMember]
        public string StopDescription { get; set; }
        [DataMember]
        public string PlaceId { get; set; }
        [DataMember]
        public Nullable<int> DistrictId { get; set; }
        [DataMember]
        public int Id511 { get; set; }
        [DataMember]
        public bool IsPublic { get; set; }
        [DataMember]
        public decimal Longitude { get; set; }
        [DataMember]
        public decimal Latitude { get; set; }
        [DataMember]
        public bool AllowAlighting { get; set; }
        [DataMember]
        public bool AllowBoarding { get; set; }
        [DataMember]
        public string Corner { get; set; }
        [DataMember]
        public bool IsBart { get; set; }
        [DataMember]
        public bool IsTransitCenter { get; set; }
        [DataMember]
        public bool IsInService { get; set; }
        [DataMember]
        public bool IsGPSValidated { get; set; }
        [DataMember]
        public string Site { get; set; }
        [DataMember]
        public bool AvaStatus { get; set; }
        [DataMember]
        public string AvaDescription { get; set; }
        [DataMember]
        public string Comment { get; set; }
        [DataMember]
        public System.DateTime LastSchedulerUpdate { get; set; }
        [DataMember]
        public string FlagRoute { get; set; }
        [DataMember]
        public Nullable<System.DateTime> ValidToDate { get; set; }
        [DataMember]
        public double? Distance { get; set; }

        [DataMember]
        public virtual District District { get; set; }
        [DataMember]
        public virtual Place Place { get; set; }

    }
}
