using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using ACTransit.Framework.DataAccess.Interface;

namespace ACTransit.Framework.DataAccess
{
    /// <summary>
    /// Exposes a set of default methods for retrieving entities
    /// </summary>
    /// <typeparam name="T">Type of entity</typeparam>
    /// <typeparam name="TContext">The DbContext to query</typeparam>
    public abstract class ReadOnlyRepositoryBase<T, TContext> : IReadOnlyRepository<T>, IDisposable
        where T : class, new()
        where TContext : DbContext, new()
    {
        protected bool Disposed;
        private TContext _context;

        protected TContext Context
        {
            get { return _context; }
        }

        public ReadOnlyRepositoryBase()
        {
            _context = new TContext();
        }

        public ReadOnlyRepositoryBase(TContext context)
        {
            _context = context;
        }

        public virtual T GetById<TId>(TId id)
        {
            try
            {
                return Context.Set<T>().Find(id);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error retrieving entity", ex);
            }
        }

        public virtual IQueryable<T> Get(params Expression<Func<T, object>>[] includes)
        {
            try
            {
                return GetIQueryable(Context.Set<T>().AsQueryable(), includes);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error retrieving entities", ex);
            }
        }

        protected IQueryable<T> GetIQueryable<TProperty>(IQueryable<T> value, params Expression<Func<T, TProperty>>[] includes)
        {
            if (includes != null)
            {
                value = includes.Aggregate(value, (current, include) => current.Include(include));
            }

            return value;
        }

        public virtual void Reload(T entity)
        {
            Context.Entry(entity).Reload();
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