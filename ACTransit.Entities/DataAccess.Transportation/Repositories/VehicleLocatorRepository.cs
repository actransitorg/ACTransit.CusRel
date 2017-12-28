using System.Collections.Generic;
using System.Linq;
using ACTransit.Entities.Transportation;

namespace ACTransit.DataAccess.Transportation.Repositories
{

    public class VehicleLocatorRepository : BaseRepository
    {
        public List<GetVehicleLocation_Result> GetVehicleLocation(string ids, string delimiter, string vehicleType, string assignedTo, string routeLines)
        {
            return ((TransportationEntities)Context).GetVehicleLocation(ids, delimiter, vehicleType, assignedTo, routeLines).ToList();
        }

        public List<GetRouteTypes_Result> GetRouteTypes()
        {
            return ((TransportationEntities)Context).GetRouteTypes().ToList();
        }

        public List<string> GetRouteLines(string routeTypeIds)
        {
            return ((TransportationEntities)Context).GetRouteLines(routeTypeIds).ToList();
        }
        public List<string> GetCommonRouteLines()
        {
            return ((TransportationEntities)Context).GetCommonRouteLines().ToList();
        }
    }
}
