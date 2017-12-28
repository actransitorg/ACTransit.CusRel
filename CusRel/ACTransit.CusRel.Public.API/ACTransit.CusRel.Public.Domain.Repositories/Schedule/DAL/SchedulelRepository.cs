using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using ACTransit.Contracts.Data.Common.PublicSite;
using ACTransit.Contracts.Data.Schedules.Booking;
using ACTransit.DataAccess.Scheduling;
using ACTransit.DataAccess.Scheduling.Repositories;
using Omu.ValueInjecter;

namespace ACTransit.CusRel.Public.Domain.Repositories.Schedule.DAL
{
    public class ScheduleRepository : IDisposable
    {
        private readonly RequestState requestState;
        private readonly SchedulingEntities context;
        private readonly HastusStopRepository stopContext;

        public ScheduleRepository(RequestState requestState, SchedulingEntities context)
        {
            this.requestState = requestState;
            this.context = context;
        }

        // =============================================================

        #region Bookkeeping

        public void Save()
        {
            context.SaveChanges();
        }

        private bool disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed && disposing)
                context.Dispose();
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        // =============================================================

        #region Lookup Tables and Enums

        #endregion

        // =============================================================

        #region Common 

        #endregion

        // =============================================================

        #region Scheduling

        public async Task<Stop> Stop511(int id)
        {
            var stop = await context.Stops.Where(s => s.Id511 == id)
                    .Include(m => m.Place)
                    .Include(m => m.Place.District)
                    .FirstOrDefaultAsync();
            var result = new Stop
            {
                District = new District(),
                Place = new Place()
            };
            if (stop == null)
                throw new Exception("Stop not found.");
            result.InjectFrom(stop);
            if (stop.Place.District != null)
                result.District.InjectFrom(stop.Place.District);
            if (stop.Place != null)
                result.Place.InjectFrom(stop.Place);
            return result;
        }

        public IEnumerable<Stop> NearestStop(decimal latitude, decimal longitude, int top,
            bool? isPublic = null, bool? allowAlighting = null, bool? allowBoarding = null, bool? isBart = null, bool? isTransitCenter = null,
            bool? isInService = null, bool? isGpsValidated = null, bool? avaStatus = null, string corner = null)
        {
            using (var hastusStopContext = new HastusStopRepository())
            {
                var result = hastusStopContext.NearestStop(latitude, longitude, top,
                    isPublic, allowAlighting, allowBoarding, isBart, isTransitCenter,
                    isInService, isGpsValidated, avaStatus, corner).ToList();
                return result.Select(MapNearestStopResult);
            }
        }

        private static Stop MapNearestStopResult(Entities.Scheduling.NearestStop_Result item)
        {
            var result = new Stop();
            result.InjectFrom(item);
            return result;
        }

        #endregion

        // =============================================================
    }
}
