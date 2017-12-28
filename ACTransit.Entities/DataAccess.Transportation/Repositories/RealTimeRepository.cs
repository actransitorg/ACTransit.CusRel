using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Linq;
using System.Threading;
using ACTransit.Entities.Transportation;
using ACTransit.Framework.DataAccess.Exceptions;

namespace ACTransit.DataAccess.Transportation.Repositories
{
    public class RealTimeRepository : BaseRepository
    {
        public int RetryCount { get; set; } = 3;
        public int RetrySleep { get; set; } = 500;

        private new TransportationEntities Context => (TransportationEntities)base.Context;

        public List<GetVehiclePositions_Result> GetVehiclePositions()
        {
            var retries = RetryCount;
            List<GetVehiclePositions_Result> result;
            do
            {
                result = Context.GetVehiclePositions().ToList();
                if (RetryCount != retries)
                    Thread.Sleep(RetrySleep);
            } while (--retries > 0 && result.Count == 0);
            return result;
        }

        public List<GetVehicleRealTimeData_Result> GetVehicleRealTimeData(string routeName = null, string stopId = null)
        {
            var retries = RetryCount;
            List<GetVehicleRealTimeData_Result> result;
            do
            {
                result = Context.GetVehicleRealTimeData(routeName).ToList();
                if (RetryCount != retries)
                    Thread.Sleep(RetrySleep);
            } while (--retries > 0 && result.Count == 0);
            return result;
        }

        public List<GetVehicleRealTimeDataAPIv1_Result> GetVehicleRealTimeDataAPIv1(string routeName = null, string stopId = null)
        {
            var retries = RetryCount;
            List<GetVehicleRealTimeDataAPIv1_Result> result;
            do
            {
                result = Context.GetVehicleRealTimeDataAPIv1(routeName, stopId).ToList();
                if (RetryCount != retries)
                    Thread.Sleep(RetrySleep);
            } while (--retries > 0 && result.Count == 0);
            return result;
        }

        public List<GetTimeBetweenStops_Result> GetTimeBetweenStops(string routeName, int fromStopId, int toStopId, string scheduleType = null)
        {
            try
            {
                return Context.GetTimeBetweenStops(routeName, fromStopId, toStopId, scheduleType).ToList();
            }
            catch (EntityCommandExecutionException ex)
            {
                throw new DataAccessException("Unable to retrieve trip estimates for the given parameters.", ex);
            }
        }
    }
}