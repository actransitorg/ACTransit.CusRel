using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ACTransit.Framework.DataAccess.Interface;

namespace ACTransit.Framework.DataAccess
{
    public abstract class ReadOnlyUnitOfWorkBase<TContext>:IReadOnlyUnitOfWork,IDisposable   
        where TContext : DbContext, new()
    {
        protected bool Disposed;
        private TContext _context;
        protected TContext Context
        {
            get { return _context; }
        }

        public ReadOnlyUnitOfWorkBase()
        {
            _context = new TContext();
        }

        public ReadOnlyUnitOfWorkBase(TContext context)
        {
            _context = context;
        }

        public virtual T GetById<T, TId>(TId id) where T : class, new()
        {
            return Context.Set<T>().Find(id);
        }

        public virtual IQueryable<T> Get<T>(params System.Linq.Expressions.Expression<Func<T, object>>[] includes) where T : class, new()
        {
            return GetIQueryable(Context.Set<T>().AsQueryable(), includes);
        }

        public virtual void Reload<T>(T entity) where T : class, new()
        {
            Context.Entry(entity).Reload();
        }

        protected IQueryable<T> GetIQueryable<T, TProperty>(IQueryable<T> value, params Expression<Func<T, TProperty>>[] includes)
        {
            if (includes != null)
            {
                value = includes.Aggregate(value, (current, include) => current.Include(include));
            }

            return value;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (Disposed)
                return;

            if (disposing)
            {
                if (_context != null)
                {
                    _context.Dispose();
                    _context = null;
                }
            }

            Disposed = true;
        }

    }
}
