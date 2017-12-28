using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using ACTransit.Framework.DataAccess.Extensions;
using ACTransit.Framework.DataAccess.Interface;
using ACTransit.Framework.Interfaces;

//using ACTransit.Framework.Interfaces;

namespace ACTransit.Framework.DataAccess
{
    public class UnitOfWorkBase<TContext>:ReadOnlyUnitOfWorkBase<TContext>,IUnitOfWork where TContext : DbContext, new()
    {

        public UnitOfWorkBase() { }

        public UnitOfWorkBase(TContext context) : base(context) { }

        public virtual T Create<T>(T entity)  where T : class, new() 
        {
            Context.Set<T>().Add(entity);
            ApplyPreSaveChanges(entity, true);

            return entity;
        }

        public virtual T Update<T>(T entity) where T : class, new() 
        {
            var attachedEntity=Context.AttachToOrGet(entity);
            Context.Entry(attachedEntity).CurrentValues.SetValues(entity);

            //if attachedEntity is equal to entity, it means there was no previous entity in the context and entity has been attached to the context via AttachToOrGet.
            //In this case, because both entities are equal, to update the changes we have to manually change the state to modified.
            if (attachedEntity.Equals(entity))  
                Context.Entry(attachedEntity).State=EntityState.Modified;

            ApplyPreSaveChanges(attachedEntity, false);
            entity = attachedEntity;
            return entity;
        }
        public virtual T Update<T, TProperty>(T entity, params Expression<Func<T, TProperty>>[] ignores) where T : class, new()
        {
            
            var attachedEntity = Context.AttachToOrGet(entity);
            Context.Entry(attachedEntity).CurrentValues.SetValues(entity);

            //if attachedEntity is equal to entity, it means there was no previous entity in the context and entity has been attached to the context via AttachToOrGet.
            //In this case, because both entities are equal, to update the changes we have to manually change the state to modified.
            if (attachedEntity.Equals(entity))
                Context.Entry(attachedEntity).State = EntityState.Modified;

            if (ignores != null && ignores.Length > 0)
            {
                for (var i = 0; i < ignores.Length; i++)
                    Context.Entry<T>(entity).Property(ignores[i]).IsModified = false;
            }

            ApplyPreSaveChanges(attachedEntity, false);
            entity = attachedEntity;
            return entity;
        }
        
        public virtual void Delete<T, TId>(TId id) where T : class, new()
        {
            var entity = new T();
            var keyValues = Context.CreateEntityKey(entity);
            if (keyValues.Length == 0)
                throw new Exception("No key found, keyValue.length is zero.");
            if (keyValues.Length >1)
                throw new Exception("more than one key found.");
                            
            entity.GetType().GetProperty(keyValues[0].Key).SetValue(entity, id);
            Delete(entity);
        }

        public virtual void Delete<T>(T entity) where T : class, new()
        {
            var attachedEntity = Context.AttachToOrGet(entity);
            entity = attachedEntity;
            Context.Set<T>().Remove(entity);
        }

        public virtual void SaveChanges()
        {            
            Context.ChangeTracker.DetectChanges();
            Context.SaveChanges();
        }
        public bool HasChanges<T>(T entity) where T : class, new()
        {
            var dbEntityEntry = Context.ChangeTracker.Entries<T>().FirstOrDefault(m => m.Entity.Equals(entity));
            var state = dbEntityEntry != null ? (EntityState?) dbEntityEntry.State : (EntityState?) null;

            if (state != null)
            {
                return state != EntityState.Unchanged && state != EntityState.Detached;
            }
            return false;
        }


        public bool HasChanges()
        {
            return Context.ChangeTracker.HasChanges();
        }

        private string _currentUserName;
        protected string CurrentUserName
        {
            get
            {
                if (string.IsNullOrEmpty(_currentUserName))
                        _currentUserName = Environment.UserName;

                return _currentUserName;
            }
            set { _currentUserName = value; }
        }

        protected virtual void ApplyPreSaveChanges<T>(T entity, bool isEntityNew) where T : class, new()
        {
            var modifiedStates = new List<EntityState> { EntityState.Added, EntityState.Modified };

            var auditDate = DateTime.UtcNow;

            foreach (var contextEntry in Context.ChangeTracker.Entries<IAuditableEntity>())
            {
                if (!modifiedStates.Contains(contextEntry.State))
                    continue;

                var auditableEntity = contextEntry.Entity;

                auditableEntity.UpdDateTime = auditDate;
                auditableEntity.UpdUserId = CurrentUserName;

                if (contextEntry.State == EntityState.Added)
                {
                    auditableEntity.AddDateTime = auditDate;
                    auditableEntity.AddUserId = CurrentUserName;
                }
                else
                {
                    Context.Entry(auditableEntity).Property("AddUserId").OriginalValue = string.Empty;
                    Context.Entry(auditableEntity).Property("AddDateTime").OriginalValue = DateTime.Now;
                    Context.Entry(auditableEntity).Property("AddUserId").IsModified = false;
                    Context.Entry(auditableEntity).Property("AddDateTime").IsModified = false;
                }
            }
        }
       
    }
}
