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
    
    public partial class RouteList
    {
        public RouteList()
        {
            this.Routes = new HashSet<Route>();
            this.TripPatterns = new HashSet<TripPattern>();
        }
    
        public string RouteAlpha { get; set; }
    
        public virtual ICollection<Route> Routes { get; set; }
        public virtual ICollection<TripPattern> TripPatterns { get; set; }
    }
}
