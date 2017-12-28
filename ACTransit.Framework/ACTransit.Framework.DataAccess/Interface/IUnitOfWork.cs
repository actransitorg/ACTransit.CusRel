namespace ACTransit.Framework.DataAccess.Interface
{
    public interface IUnitOfWork:IReadOnlyUnitOfWork
    {
        T Create<T>(T entity) where T : class, new();

        T Update<T>(T entity) where T : class, new();

        void Delete<T,TId>(TId id)  where T : class, new();

        void Delete<T>(T entity) where T : class, new();

        void SaveChanges();
    }
}
