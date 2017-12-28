using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Runtime.InteropServices;
using System.Security.Principal;
using ACTransit.Framework.DataAccess.Interface;
using ACTransit.Framework.DataAccess.Extensions;
using ACTransit.Framework.Interfaces;

//using ACTransit.Framework.Interfaces;

namespace ACTransit.Framework.DataAccess
{
    /// <summary>
    /// Exposes a set of default methods for creating, updating and deleting entities.
    /// </summary>
    /// <typeparam name="T">Type of entity</typeparam>
    /// <typeparam name="TContext">The DbContext to query</typeparam>
    public class RepositoryBase<T, TContext> : ReadOnlyRepositoryBase<T, TContext>, IRepository<T>
        where T : class, new()
        where TContext : DbContext, new()
    {

        public RepositoryBase() { }

        public RepositoryBase(TContext context) : base(context) { }


        public virtual T Create(T entity)
        {
            try
            {
                Context.Set<T>().Add(entity);
                ApplyPreSaveChanges(entity, true);

                return entity;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error creating entity", ex.GetBaseException());
            }
        }

        public virtual T Update(T entity)
        {
            try
            {
                var attachedEntity = Context.AttachToOrGet(entity);
                Context.Entry(attachedEntity).CurrentValues.SetValues(entity);

                //if attachedEntity is equal to entity, it means there was no previous entity in the context and entity has been attached to the context via AttachToOrGet.
                //In this case, because both entities are equal, to update the changes we have to manually change the state to modified.
                if (attachedEntity.Equals(entity))
                    Context.Entry(attachedEntity).State = EntityState.Modified;

                ApplyPreSaveChanges(attachedEntity, false);
                entity = attachedEntity;
                return entity;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error updating entity", ex.GetBaseException());
            }
        }

        public virtual void Delete<TId>(TId id)
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

        public virtual void Delete(T entity)
        {
            try
            {
                var attachedEntity = Context.AttachToOrGet(entity);
                entity = attachedEntity;
                Context.Set<T>().Remove(entity);

            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error deleting entity", ex.GetBaseException());
            }
        }

        public void SaveChanges()
        {
            Context.ChangeTracker.DetectChanges();
            Context.SaveChanges();
        }

        /// <summary>
        /// Last place to apply changes to entities before they are saved to the data store
        /// </summary>
        /// <param name="context"></param>
        /// <param name="entity">The entity being saved</param>
        /// <param name="isEntityNew">Determines whether or not the entity is being created.</param>
        private string _currentUserName;
        protected string CurrentUserName
        {
            get
            {
                if (string.IsNullOrEmpty(_currentUserName))
                {
                    var identity = WindowsIdentity.GetCurrent();
                    if (identity != null)
                        _currentUserName = identity.Name;
                }

                return _currentUserName;
            }
            set { _currentUserName = value; }
        }

        protected void ApplyPreSaveChanges(T entity, bool isEntityNew)
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