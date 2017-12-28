using ACTransit.Contracts.Data.Common.PublicSite;

namespace ACTransit.CusRel.Public.Domain.Repositories.Schedule.DAL
{
    public sealed class Repository
    {
        static Repository() { }

        private Repository() { }

        public static ScheduleRepository Context(RequestState requestState)
        {
            return new ScheduleRepository(requestState, Entity.Context(requestState));
        }
    }
}