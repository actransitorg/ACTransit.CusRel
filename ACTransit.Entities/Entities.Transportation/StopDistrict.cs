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
    using System.Collections.Generic;
    
    public partial class StopDistrict
    {
        public string identifier { get; set; }
        public int id_511 { get; set; }
        public string district { get; set; }
        public string description { get; set; }
        public string place { get; set; }
        public bool is_public { get; set; }
        public decimal longitude { get; set; }
        public decimal latitude { get; set; }
        public bool allow_alighting { get; set; }
        public bool allow_boarding { get; set; }
        public string corner { get; set; }
        public bool is_bart { get; set; }
        public bool is_transit_center { get; set; }
        public bool is_in_service { get; set; }
        public System.DateTime UpdDateTime { get; set; }
        public string UpdUserId { get; set; }
        public System.DateTime AddDateTime { get; set; }
        public string AddUserId { get; set; }
        public string flag_route { get; set; }
        public System.DateTime last_scheduler_update { get; set; }
        public string comment { get; set; }
        public string ava_description { get; set; }
        public bool ava_status { get; set; }
        public string site { get; set; }
        public bool is_GPS_validated { get; set; }
    }
}
