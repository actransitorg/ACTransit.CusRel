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
    
    public partial class Bus_pull_out
    {
        public string Date { get; set; }
        public string Time_on { get; set; }
        public string Time_off { get; set; }
        public string Bus { get; set; }
        public string Route { get; set; }
        public string Block { get; set; }
        public string Run { get; set; }
        public string Badge { get; set; }
        public string Division { get; set; }
        public string Location_on { get; set; }
        public string Location_off { get; set; }
        public string Type { get; set; }
        public Nullable<System.DateTime> DT_Time_On { get; set; }
        public Nullable<System.DateTime> DT_Time_Off { get; set; }
        public int BusPullOutId { get; set; }
    }
}
