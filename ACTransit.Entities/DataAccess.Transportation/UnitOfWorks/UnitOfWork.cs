using System;
using System.Data;
using ACTransit.Framework.DataAccess;
using ACTransit.Framework.DataAccess.Extensions;

namespace ACTransit.DataAccess.Transportation.UnitOfWorks
{
    public class UnitOfWork : UnitOfWorkBase<TransportationEntities>
    {
        public UnitOfWork() : this(new TransportationEntities(), null) { }
        public UnitOfWork(TransportationEntities context) : this(context, null) { }
        public UnitOfWork(string currentUserName) : this(new TransportationEntities(), currentUserName) { }
        public UnitOfWork(TransportationEntities context, string currentUserName) : base(context) { CurrentUserName = currentUserName; }

        public object GetEntityKeyValue<T>(T entity) where T:class, new()
        {
            var keyvalues = Context.CreateEntityKey(entity);
            if (keyvalues==null || keyvalues.Length!=1)
                throw new MissingPrimaryKeyException();
            if (keyvalues.Length>1) 
                throw new Exception("more than one Key found.");

            return keyvalues[0].Value;
        }
   


    }
}
