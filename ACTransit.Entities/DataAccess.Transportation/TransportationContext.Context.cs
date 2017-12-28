﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ACTransit.DataAccess.Transportation
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    using ACTransit.Entities.Transportation;
    
    public partial class TransportationEntities : DbContext
    {
        public TransportationEntities()
            : base("name=TransportationEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<RouteAttrib> RouteAttribs { get; set; }
        public virtual DbSet<Booking> Bookings { get; set; }
        public virtual DbSet<Direction_codes> Direction_codes { get; set; }
        public virtual DbSet<Division> Divisions { get; set; }
        public virtual DbSet<EmployeeInterface> EmployeeInterfaces { get; set; }
        public virtual DbSet<HolidayList> HolidayLists { get; set; }
        public virtual DbSet<incident_log> incident_log { get; set; }
        public virtual DbSet<Route> Routes { get; set; }
        public virtual DbSet<VehicleRealTime> VehicleRealTimes { get; set; }
        public virtual DbSet<Bus_pull_out> Bus_pull_out { get; set; }
        public virtual DbSet<Bus_pull_out_History> Bus_pull_out_History { get; set; }
        public virtual DbSet<VehicleInfo> VehicleInfoes { get; set; }
        public virtual DbSet<StopDistrict> StopDistricts { get; set; }
    
        public virtual ObjectResult<GetTimeBetweenStops_Result> GetTimeBetweenStops(string routeName, Nullable<int> fromStopId, Nullable<int> toStopId, string scheduleType)
        {
            var routeNameParameter = routeName != null ?
                new ObjectParameter("RouteName", routeName) :
                new ObjectParameter("RouteName", typeof(string));
    
            var fromStopIdParameter = fromStopId.HasValue ?
                new ObjectParameter("FromStopId", fromStopId) :
                new ObjectParameter("FromStopId", typeof(int));
    
            var toStopIdParameter = toStopId.HasValue ?
                new ObjectParameter("ToStopId", toStopId) :
                new ObjectParameter("ToStopId", typeof(int));
    
            var scheduleTypeParameter = scheduleType != null ?
                new ObjectParameter("ScheduleType", scheduleType) :
                new ObjectParameter("ScheduleType", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetTimeBetweenStops_Result>("GetTimeBetweenStops", routeNameParameter, fromStopIdParameter, toStopIdParameter, scheduleTypeParameter);
        }
    
        public virtual ObjectResult<GetVehicleRealTimeDataAPIv1_Result> GetVehicleRealTimeDataAPIv1(string routeName, string stopId)
        {
            var routeNameParameter = routeName != null ?
                new ObjectParameter("RouteName", routeName) :
                new ObjectParameter("RouteName", typeof(string));
    
            var stopIdParameter = stopId != null ?
                new ObjectParameter("StopId", stopId) :
                new ObjectParameter("StopId", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetVehicleRealTimeDataAPIv1_Result>("GetVehicleRealTimeDataAPIv1", routeNameParameter, stopIdParameter);
        }
    
        public virtual ObjectResult<GetVehiclePositions_Result> GetVehiclePositions()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetVehiclePositions_Result>("GetVehiclePositions");
        }
    
        public virtual ObjectResult<GetVehicleRealTimeData_Result> GetVehicleRealTimeData(string routeName)
        {
            var routeNameParameter = routeName != null ?
                new ObjectParameter("RouteName", routeName) :
                new ObjectParameter("RouteName", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetVehicleRealTimeData_Result>("GetVehicleRealTimeData", routeNameParameter);
        }
    
        public virtual ObjectResult<GetBusPullOuts_Result> GetBusPullOuts(string division)
        {
            var divisionParameter = division != null ?
                new ObjectParameter("Division", division) :
                new ObjectParameter("Division", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetBusPullOuts_Result>("GetBusPullOuts", divisionParameter);
        }
    
        public virtual ObjectResult<GetVehicleLocation_Result> GetVehicleLocation(string idList, string context, string vehicleType, string assignedTo, string routeLines)
        {
            var idListParameter = idList != null ?
                new ObjectParameter("IdList", idList) :
                new ObjectParameter("IdList", typeof(string));
    
            var contextParameter = context != null ?
                new ObjectParameter("Context", context) :
                new ObjectParameter("Context", typeof(string));
    
            var vehicleTypeParameter = vehicleType != null ?
                new ObjectParameter("VehicleType", vehicleType) :
                new ObjectParameter("VehicleType", typeof(string));
    
            var assignedToParameter = assignedTo != null ?
                new ObjectParameter("AssignedTo", assignedTo) :
                new ObjectParameter("AssignedTo", typeof(string));
    
            var routeLinesParameter = routeLines != null ?
                new ObjectParameter("RouteLines", routeLines) :
                new ObjectParameter("RouteLines", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetVehicleLocation_Result>("GetVehicleLocation", idListParameter, contextParameter, vehicleTypeParameter, assignedToParameter, routeLinesParameter);
        }
    
        public virtual ObjectResult<string> GetCommonRouteLines()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<string>("GetCommonRouteLines");
        }
    
        public virtual ObjectResult<string> GetRouteLines(string routeTypeIds)
        {
            var routeTypeIdsParameter = routeTypeIds != null ?
                new ObjectParameter("RouteTypeIds", routeTypeIds) :
                new ObjectParameter("RouteTypeIds", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<string>("GetRouteLines", routeTypeIdsParameter);
        }
    
        public virtual ObjectResult<GetRouteTypes_Result> GetRouteTypes()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetRouteTypes_Result>("GetRouteTypes");
        }
    }
}
