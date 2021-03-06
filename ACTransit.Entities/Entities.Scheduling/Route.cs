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
    
    public partial class Route
    {
        public Route()
        {
            this.Trips = new HashSet<Trip>();
        }
    
        public string BookingId { get; set; }
        public string RouteAlpha { get; set; }
        public string RouteDescription { get; set; }
        public int RouteTypeId { get; set; }
        public string Color { get; set; }
        public int SortOrder { get; set; }
        public string AddUserId { get; set; }
        public System.DateTime AddDateTime { get; set; }
        public string UpdUserId { get; set; }
        public Nullable<System.DateTime> UpdDateTime { get; set; }
        public long SysRecNo { get; set; }
    
        public virtual RouteList RouteList { get; set; }
        public virtual RouteType RouteType { get; set; }
        public virtual ICollection<Trip> Trips { get; set; }
    }
}
