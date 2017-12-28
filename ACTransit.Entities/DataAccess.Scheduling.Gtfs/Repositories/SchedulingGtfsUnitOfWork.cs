using ACTransit.Framework.DataAccess;

namespace ACTransit.DataAccess.Scheduling.Gtfs
{
    public class SchedulingGtfsUnitOfWork : UnitOfWorkBase<SchedulingGtfsEntities>
    {
        public SchedulingGtfsUnitOfWork() : this(new SchedulingGtfsEntities(), null) { }
        public SchedulingGtfsUnitOfWork(SchedulingGtfsEntities context) : this(context, null) { }
        public SchedulingGtfsUnitOfWork(string currentUserName) : this(new SchedulingGtfsEntities(), currentUserName) { }
        public SchedulingGtfsUnitOfWork(SchedulingGtfsEntities context, string currentUserName) : base(context) { CurrentUserName = currentUserName; }
    }

    public class SchedulingGtfsReadOnlyUnitOfWork : ReadOnlyUnitOfWorkBase<SchedulingGtfsEntities>
    {
        public SchedulingGtfsReadOnlyUnitOfWork() : this(new SchedulingGtfsEntities()) { }
        public SchedulingGtfsReadOnlyUnitOfWork(SchedulingGtfsEntities context) : base(context) { }
    }
}
