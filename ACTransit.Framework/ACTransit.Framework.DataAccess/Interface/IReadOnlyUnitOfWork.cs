using System;
using System.Linq;
using System.Linq.Expressions;

namespace ACTransit.Framework.DataAccess.Interface
{
    public interface IReadOnlyUnitOfWork
    {
        T GetById<T, TId>(TId id) where T : class, new();
        IQueryable<T> Get<T>(params Expression<Func<T, object>>[] includes) where T : class, new();
        void Reload<T>(T entity) where T : class, new();
    }
}
