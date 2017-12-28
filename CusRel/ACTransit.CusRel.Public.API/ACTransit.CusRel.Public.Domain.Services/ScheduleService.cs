using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ACTransit.Contracts.Data.Common.PublicSite;
using ACTransit.Contracts.Data.Schedules.Booking;
using ACTransit.CusRel.Public.Domain.Repositories.Schedule.DAL;

namespace ACTransit.CusRel.Public.Domain.Services
{
    public class ScheduleService
    {
        private readonly RequestState requestState;

        public ScheduleService(RequestState requestState)
        {
            this.requestState = requestState;
        }

        public async Task<Stop> Stop511(int id)
        {
            return await Repository.Context(requestState).Stop511(id);
        }

        public IEnumerable<Stop> NearestStops(decimal latitude, decimal longitude, int? count = 3)
        {
            return Repository.Context(requestState).NearestStop(latitude, longitude, count ?? 1, true, true, true, null, null, true, true);
        }
    }
}
