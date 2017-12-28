namespace ACTransit.Framework.DataAccess.Interface
{
    public interface IRepository<T> : IReadOnlyRepository<T> where T : class, new()
    {
        T Create(T entity);

        T Update(T entity);

        void Delete<TId>(TId id);

        void Delete(T entity);

        void SaveChanges();
    }
}