//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ACTransit.Entities.Transportation
{
    using System;
    
    public partial class GetTimeBetweenStops_Result
    {
        public string RouteAlpha { get; set; }
        public Nullable<int> TripId { get; set; }
        public string ScheduleType { get; set; }
        public string Direction { get; set; }
        public Nullable<System.DateTime> TimeStart { get; set; }
        public Nullable<System.DateTime> TimeEnd { get; set; }
        public Nullable<int> FromStopId { get; set; }
        public Nullable<short> FromStopSequence { get; set; }
        public Nullable<System.DateTime> FromStopPassingTime { get; set; }
        public Nullable<int> ToStopId { get; set; }
        public Nullable<short> ToStopSequence { get; set; }
        public Nullable<System.DateTime> ToStopPassingTime { get; set; }
        public Nullable<int> VehicleId { get; set; }
        public Nullable<short> VehiclePassengerCapacity { get; set; }
        public Nullable<bool> VehicleWheelchairAccessible { get; set; }
        public Nullable<System.DateTime> FromStopExpectedDepartureTime { get; set; }
    }
}
