using ACTransit.Contracts.Data.Common.PublicSite;
using ACTransit.DataAccess.Scheduling;

namespace ACTransit.CusRel.Public.Domain.Repositories.Schedule.DAL
{
    public sealed class Entity
    {
        static Entity() { }

        private Entity() { }

        public static SchedulingEntities Context(RequestState RequestState = null)
        {
            var result = RequestState != null && RequestState.ConnectionStrings != null
                ? new SchedulingEntities(RequestState.ConnectionStrings[typeof(SchedulingEntities).Name].ToString())
                : new SchedulingEntities();
            result.Configuration.ProxyCreationEnabled = false;
            return result;
        }
    }
}
