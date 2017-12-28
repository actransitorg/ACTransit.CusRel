using ACTransit.Framework.DataAccess;

namespace ACTransit.DataAccess.Scheduling.Gtfs
{
    public class SchedulingGtfsRepository<T> : RepositoryBase<T, SchedulingGtfsEntities>
        where T : class, new()
    {
    }

    public class SchedulingGtfsReadOnlyRepository<T> : ReadOnlyRepositoryBase<T, SchedulingGtfsEntities>
        where T : class, new()
    {
    }
}