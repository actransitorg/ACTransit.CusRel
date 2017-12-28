using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ACTransit.Contracts.Data.CusRel.LookupContract;
using ACTransit.Contracts.Data.CusRel.LookupContract.Result;
using ACTransit.Contracts.Data.Schedules.PublicSite;
using ACTransit.DataAccess.MapsSchedules.Repositories;
using ACTransit.DataAccess.Scheduling;
using ACTransit.DataAccess.Transportation;

namespace ACTransit.CusRel.Repositories
{
    public class MapsScheduleRepository: IDisposable
    {
        private readonly Contracts.Data.Common.PublicSite.RequestState MapsSchedulesRequestState = null;
        private TicketEntitiesRepository ticketEntitiesRepository;
        private TransportationEntities transportationContext;
        private SchedulingEntities schedulingContext;
        private SettingsRepository settingsRepository;

        /// <summary>
        /// DetachGet is useful for special case: updating/inserting a TicketEntities object graph, itself created from a get operation (not from a client request).  
        /// This is a side effect from the population of TicketEntities.
        /// </summary>
        public bool DetachGet { get; set; }

        #region Constructors / Initialization

        public MapsScheduleRepository(Contracts.Data.Common.PublicSite.RequestState MapsSchedulesRequestState = null)
        {
            this.MapsSchedulesRequestState = MapsSchedulesRequestState;
            init();
        }

        private void init()
        {
            if (ticketEntitiesRepository != null)
                ticketEntitiesRepository.DetachGet = DetachGet;

            if (settingsRepository == null)
                settingsRepository = new SettingsRepository();
        }

        private void InitTransportationContext()
        {
            if (transportationContext == null)
                transportationContext = new TransportationEntities();
            init();
        }

        private void InitSchedulingContext()
        {
            if (schedulingContext == null)
                schedulingContext = new SchedulingEntities();
            init();
        }

        #endregion

        // =============================================================

        #region Dispose

        private bool disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed && disposing)
            {
                if (transportationContext != null)
                    transportationContext.Dispose();
            }
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
       
        // =============================================================

        #region Public Methods and Delegates

        public Line GetLine(string LineValue, DateTime date)
        {
            var result = Task.Run(() => RepositoryFactory.Create(MapsSchedulesRequestState).LineRoute(LineValue, date));
            return result.Result;
        }

        public List<Line> GetLines(DateTime date)
        {
            var result = Task.Run(() => RepositoryFactory.Create(MapsSchedulesRequestState).Lines(date));
            return result.Result;
        }

        public List<Line> GetAllLines()
        {
            var result = Task.Run(() => RepositoryFactory.Create(MapsSchedulesRequestState).AllLines());
            return result.Result;
        }

        public RouteInfoResult GetRouteInfo(string route, DateTime? date)
        {
            try
            {
                if (route != null && Regex.Replace(route, @"[\s\r\n/]+", "").ToLower() == "na")
                    route = null;

                InitTransportationContext();

                if (date.HasValue)
                {
                    InitSchedulingContext();

                    //For old tickets for which bookings do not exist force latest schedule, even if inaccurate, to prevent date out of range error.
                    if (date < schedulingContext.Bookings.Min(b => b.StartDate))
                        date = DateTime.Now;

                    var line = route != null ? GetLine(route, date.Value) : null;

                    var dateToCompare = date.Value.Date;
                    var bookingId = (
                        from b in schedulingContext.Bookings
                        where b.StartDate <= date.Value && b.EndDate >= dateToCompare
                        select b.BookingId
                    ).FirstOrDefault();

                    if (bookingId != null)
                    {
                        var divisionQuery = (
                            from r in schedulingContext.Routes
                            where r.BookingId == bookingId && r.RouteAlpha == route
                            join t in schedulingContext.Trips
                            on new { p1 = r.RouteAlpha, p2 = bookingId } equals new { p1 = t.RouteAlpha, p2 = t.BookingId }
                            where t.ValidToDate == null || t.ValidToDate > date //DateTime.Now
                        join b in schedulingContext.Blocks
                            on new { p1 = t.BlockNumber, p2 = bookingId } equals new { p1 = b.BlockNumber, p2 = b.BookingId }
                            where b.ValidToDate == null || b.ValidToDate > date //DateTime.Now
                        select new { b.Garage, BlockScheduleType = b.ScheduleType, TripScheduleType = t.ScheduleType }
                        ).Distinct().ToList();

                        //Only one division should be assigned per route. If resultset returns more than one division then filter by the schedule type
                        if (divisionQuery.Count() > 1)
                        {
                            var scheduleType = date.Value.DayOfWeek == DayOfWeek.Saturday
                                ? "Saturday"
                                : date.Value.DayOfWeek == DayOfWeek.Sunday
                                    ? "Sunday"
                                    : "Weekday";

                            divisionQuery = divisionQuery.Where(d => d.BlockScheduleType == scheduleType).ToList();

                            //Additional filtering if the query still returns more than one division
                            if (divisionQuery.Select(d => d.Garage).Distinct().Count() > 1)
                            {
                                divisionQuery = divisionQuery.Where(d => d.TripScheduleType == scheduleType).ToList();
                            }
                        }

                        var divisions = divisionQuery.Select(d => d.Garage).Distinct().OrderBy(d => d).ToList();

                        var directions = line != null
                            ? (from r in line.Routes
                               where GetDayCodes(date.Value).Contains(r.DayCode.KeyValue.Key)
                                 && r.DirectionCode.Description != "To "
                               select r.DirectionCode.Description.Replace("To ", "")).OrderBy(d => d).ToList()
                            : null;

                        return new RouteInfoResult
                        {
                            RouteInfo = new RouteInfo
                            {
                                Routes = GetLines(date.Value)
                                         .Select(r => r.KeyValue.Value)
                                         .Union(settingsRepository.GetSettings("FlexRoute").Settings.First().Value.Split(new char[] { ',' }).Select(r => r.Trim()))
                                         .OrderBy(r => r)
                                         .ToList(),
                                Route = route,
                                Directions = (!string.IsNullOrEmpty(route) && route.Contains("FLEX") ? settingsRepository.ParseSettings("FlexRouteDirection", ',') : directions),
                                Divisions = (!string.IsNullOrEmpty(route) && route.Contains("FLEX") ? settingsRepository.ParseSettings("FlexRouteDivision", ',') : divisions)
                            }
                        };
                    }
                }

                return new RouteInfoResult
                {
                    RouteInfo = new RouteInfo
                    {
                        Routes = GetAllLines().Select(r => r.KeyValue.Value).ToList(),
                        Route = null,
                        Directions = null,
                        Divisions = transportationContext.Divisions.Select(d => d.division_sname).Distinct().OrderBy(d => d).ToList()
                    }
                };
            }
            catch (Exception e)
            {
                var result = new RouteInfoResult();
                result.SetFail(e);
                return result;
            }
        }

        public Func<DateTime, IEnumerable<string>> GetDayCodes;

        public List<DateTime> GetHolidayList()
        {
            InitTransportationContext();
            return transportationContext.HolidayLists.Select(h=>h.HolidayDate).ToList();
        }

        public Tuple<decimal, decimal> GetStopLatLong(int id)
        {
            InitSchedulingContext();
            var result = schedulingContext.Stops.FirstOrDefault(s => s.Id511 == id);
            return result != null ? new Tuple<decimal, decimal>(result.Latitude, result.Longitude) : null;
        }

        public string GetStopStreetIntersection(int id)
        {
            InitSchedulingContext();
            var result = schedulingContext.Stops.FirstOrDefault(s => s.Id511 == id);
            return result != null ? result.StopDescription.Replace(":", " and ") : null;
        }

        #endregion

        // =============================================================


    }
}
