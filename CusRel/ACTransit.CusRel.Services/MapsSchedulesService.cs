using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using ACTransit.Contracts.Data.CusRel.LookupContract.Result;
using ACTransit.CusRel.Repositories;
using ACTransit.CusRel.Services.Extensions;
using Newtonsoft.Json.Linq;

namespace ACTransit.CusRel.Services
{
    public class MapsScheduleService: IDisposable
    {
        private readonly Contracts.Data.Common.PublicSite.RequestState MapsSchedulesRequestState = null;
        private MapsScheduleRepository mapsScheduleRepository;
        private readonly ServicesProxy ServicesProxy;

        /// <summary>
        /// DetachGet is useful for special case: updating/inserting a TicketEntities object graph, itself created from a get operation (not from a client request).  
        /// This is a side effect from the population of TicketEntities.
        /// </summary>
        public bool DetachGet { get; set; }

        #region Constructors / Initialization

        public MapsScheduleService(ServicesProxy ServicesProxy, Contracts.Data.Common.PublicSite.RequestState MapsSchedulesRequestState = null)
        {
            this.ServicesProxy = ServicesProxy;
            this.MapsSchedulesRequestState = MapsSchedulesRequestState;
            init();
        }

        private void init()
        {
            if (mapsScheduleRepository == null)
                mapsScheduleRepository = new MapsScheduleRepository
                {
                    GetDayCodes = GetDayCodes
                };
            if (mapsScheduleRepository != null)
                mapsScheduleRepository.DetachGet = DetachGet;            
        }

        #endregion

        // =============================================================

        #region Dispose

        private bool disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed && disposing)
            {
                if (mapsScheduleRepository != null)
                    mapsScheduleRepository.Dispose();
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

        #region Public Methods


        public RouteInfoResult GetRouteInfo(string Route, DateTime? date)
        {
            return mapsScheduleRepository.GetRouteInfo(Route, date);
        }

        public IEnumerable<string> GetDayCodes(DateTime date)
        {
            if (date == DateTime.MinValue)
                date = DateTime.Now;
            var dtService = new DateTimeExtensions(this);
            var isHoliday = dtService.IsHoliday(date);

            if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                yield return "WE";
            else
                yield return "WD";

            if (date.DayOfWeek == DayOfWeek.Saturday)
                yield return "SA";

            if (date.DayOfWeek == DayOfWeek.Sunday || isHoliday)
                yield return "SU";

            if (date.DayOfWeek == DayOfWeek.Tuesday || date.DayOfWeek == DayOfWeek.Thursday)
                yield return "TT";

            yield return "DA";
        }

        public List<DateTime> GetHolidayList()
        {
            return mapsScheduleRepository.GetHolidayList();
        }

        public string GetStopCity(int id, string urlTemplate = "")
        {
            var latLong = mapsScheduleRepository.GetStopLatLong(id);
            if (latLong == null) return null;
            var wc = new WebClient();
            var response = wc.DownloadString(string.Format(urlTemplate, latLong.Item1, latLong.Item2));
            var o = JObject.Parse(response);
            var address = (string)o["results"][0]["formatted_address"];
            var re = new Regex(@"[^,]+,\s+([^,]+),");
            var city = re.Match(address).Groups[1].Value;
            return city; //address.Substring(0, address.IndexOf(",")).Trim().Replace(":", " and ");
        }

        public string GetStopStreetIntersection(int id)
        {
            return mapsScheduleRepository.GetStopStreetIntersection(id);
        }

        #endregion

        // =============================================================


    }
}
