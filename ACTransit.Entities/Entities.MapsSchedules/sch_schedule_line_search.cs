//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ACTransit.Entities.MapsSchedules
{
    using System;
    using System.Collections.Generic;
    
    public partial class sch_schedule_line_search
    {
        public int schedule_id { get; set; }
        public Nullable<int> line_id { get; set; }
        public string line_name { get; set; }
        public string notes { get; set; }
        public string direction_replace { get; set; }
        public string direction_names { get; set; }
        public string direction_code { get; set; }
        public string day_code { get; set; }
        public string adjectives { get; set; }
        public Nullable<int> sort_order { get; set; }
    }
}
