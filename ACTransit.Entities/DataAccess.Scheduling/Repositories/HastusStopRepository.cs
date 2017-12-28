using System.Collections.Generic;
using System.Linq;
using ACTransit.Entities.Scheduling;

namespace ACTransit.DataAccess.Scheduling.Repositories
{
    public class HastusStopRepository : SchedulingRepository<Stop>
    {
        public IEnumerable<Stop> GetStopsWithinProximity(decimal latitude, decimal longitude, Stop.DistanceUnit unitOfMeasure, decimal distance, string routeName = null)
        {
            return Context.GetStopsWithinProximity2((double)latitude, (double)longitude, GetDistanceUnitString(unitOfMeasure), (double)distance, routeName);
        }

        private string GetDistanceUnitString(Stop.DistanceUnit unitOfMeasure)
        {
            switch (unitOfMeasure) 
            {
                case Stop.DistanceUnit.Kilometers:
                    return "km";
                case Stop.DistanceUnit.Miles:
                    return "mi";
                default:
                    return "ft";
            }
        }

        public IQueryable<NearestStop_Result> NearestStop(decimal latitude, decimal longitude, int top, 
            bool? isPublic, bool? allowAlighting, bool? allowBoarding, bool? isBart, bool? isTransitCenter, bool? isInService, bool? isGpsValidated, bool? avaStatus, string corner = null)
        {
            return Context.NearestStop(latitude, longitude, top, isPublic, allowAlighting, allowBoarding, corner, isBart, isTransitCenter, isInService, isGpsValidated, avaStatus).AsQueryable();
        }
    }
}