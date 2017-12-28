using ACTransit.Framework.DataAccess;

namespace ACTransit.DataAccess.Scheduling.Repositories
{
    public class SchedulingRepository<T> : RepositoryBase<T, SchedulingEntities>
        where T : class, new()
    {
    }

    public class SchedulingReadOnlyRepository<T> : ReadOnlyRepositoryBase<T, SchedulingEntities>
        where T : class, new()
    {
    }
}