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
    
    public partial class sch_lines
    {
        public int line_id { get; set; }
        public Nullable<int> category_id { get; set; }
        public Nullable<int> version_id { get; set; }
        public string line_name { get; set; }
        public string description { get; set; }
        public string directions_file { get; set; }
        public string stop_lists_file { get; set; }
        public string map_file { get; set; }
        public string last_update { get; set; }
        public string cmyk_value { get; set; }
        public string rgb_value { get; set; }
        public Nullable<int> is_draft { get; set; }
        public Nullable<int> is_processing { get; set; }
        public Nullable<int> line_pdb_updated { get; set; }
        public string line_update_by { get; set; }
        public Nullable<System.DateTime> line_update_timestamp { get; set; }
    }
}
