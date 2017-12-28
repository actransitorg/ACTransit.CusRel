using System;
using System.Data.Entity;

namespace ACTransit.DataAccess.Employee.Repositories
{
    public abstract class BaseRepository:IDisposable
    {
        private bool _disposed;
        protected DbContext Context { get; private set; }

        protected BaseRepository()
        {
            Context = new EmployeeEntities();
        }

        protected BaseRepository(DbContext context)
        {
            Context = context;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                if (Context != null)
                {
                    Context.Dispose();
                }                
            }
            _disposed = true;
        }

        public void DisableProxy()
        {
            Context.Configuration.ProxyCreationEnabled = false;
        }
    }
}
