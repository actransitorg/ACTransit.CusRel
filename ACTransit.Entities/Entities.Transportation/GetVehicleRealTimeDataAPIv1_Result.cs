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
    
    public partial class GetVehicleRealTimeDataAPIv1_Result
    {
        public int TripId { get; set; }
        public Nullable<int> NextTripId { get; set; }
        public string TripScheduleType { get; set; }
        public System.DateTime TripStartTime { get; set; }
        public System.DateTime TripEndTime { get; set; }
        public string TripDirection { get; set; }
        public string TripRouteName { get; set; }
        public short TripPatternId { get; set; }
        public int StopId { get; set; }
        public short StopSequence { get; set; }
        public string StopDescription { get; set; }
        public decimal StopLatitude { get; set; }
        public decimal StopLongitude { get; set; }
        public System.DateTime StopScheduledDepartureTime { get; set; }
        public Nullable<int> ExpectedDelay { get; set; }
        public Nullable<System.DateTime> ExpectedDepartureTime { get; set; }
        public int VehicleId { get; set; }
        public Nullable<System.DateTime> VehiclePositionDateTime { get; set; }
        public Nullable<double> VehicleCurrentLatitude { get; set; }
        public Nullable<double> VehicleCurrentLongitude { get; set; }
        public Nullable<short> VehicleHeading { get; set; }
        public string VehicleLicenseNumber { get; set; }
        public short VehiclePassengerCapacity { get; set; }
        public bool VehicleWheelchairAccessible { get; set; }
    }
}