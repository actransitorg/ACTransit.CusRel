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
    
    public partial class RouteType
    {
        public RouteType()
        {
            this.Routes = new HashSet<Route>();
        }
    
        public int RouteTypeId { get; set; }
        public string RouteTypeName { get; set; }
        public string AddUserId { get; set; }
        public System.DateTime AddDateTime { get; set; }
        public string UpdUserId { get; set; }
        public Nullable<System.DateTime> UpdDateTime { get; set; }
    
        public virtual ICollection<Route> Routes { get; set; }
    }
}