//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ACTransit.Entities.Scheduling
{
    using System;
    using System.Collections.Generic;
    
    public partial class VehicleDisplayCode
    {
        public string BookingID { get; set; }
        public long SysRecNo { get; set; }
        public string VDCId { get; set; }
        public string VDCMessage1 { get; set; }
        public string VDCMessage2 { get; set; }
        public string VDCMessage3 { get; set; }
        public string VDCMessage4 { get; set; }
        public Nullable<System.DateTime> ValidToDate { get; set; }
        public string AddUserId { get; set; }
        public System.DateTime AddDateTime { get; set; }
        public string UpdUserId { get; set; }
        public System.DateTime UpdDateTime { get; set; }
        public string BookingId { get; set; }
    
        public virtual Booking Booking { get; set; }
        public virtual Booking Booking1 { get; set; }
    }
}