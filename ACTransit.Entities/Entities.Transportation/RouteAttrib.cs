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
    
    public partial class RouteAttrib
    {
        public long RouteAttribsId { get; set; }
        public string AddUserId { get; set; }
        public System.DateTime AddDateTime { get; set; }
        public string UpdUserId { get; set; }
        public System.DateTime UpdDateTime { get; set; }
        public int route_id { get; set; }
        public int division_id { get; set; }
        public long RouteTypeId { get; set; }
        public System.DateTime BegEffDate { get; set; }
        public System.DateTime EndEffDate { get; set; }
    }
}
