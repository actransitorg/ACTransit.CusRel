using System;
using System.Linq;
using System.Linq.Expressions;

namespace ACTransit.Framework.DataAccess.Interface
{
    public interface IReadOnlyRepository<T> where T : class, new()
    {
        T GetById<TId>(TId id);
        IQueryable<T> Get(params Expression<Func<T, object>>[] includes);
        void Reload(T entity);
    }
}