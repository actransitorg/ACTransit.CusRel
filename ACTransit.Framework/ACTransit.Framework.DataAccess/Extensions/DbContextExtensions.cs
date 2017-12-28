using System;
using System.Linq;
using System.Collections.Generic;

using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

namespace ACTransit.Framework.DataAccess.Extensions
{
    public static class DbContextExtensions
    {


        public static T AttachToOrGet<T>(this IObjectContextAdapter contextAdapter, T entity) 
            where T : class
        {
            var context = contextAdapter.ObjectContext;
            string entitySetName = context.CreateObjectSet<T>().EntitySet.Name;
            T attachedEntity;
            ObjectStateEntry entry;
            // Track whether we need to perform an attach
            bool attach = false;
            if (context.ObjectStateManager.TryGetObjectStateEntry(context.CreateEntityKey(entitySetName, entity),out entry))
            {                
                // Re-attach if necessary
                attach = entry.State == EntityState.Detached;
                // Get the discovered entity to the ref
                attachedEntity = (T)entry.Entity;
            }
            else
            {
                attachedEntity = entity;
                // Attach for the first time
                attach = true;
            }
            if (attach)
                context.AttachTo(entitySetName, attachedEntity);
            return attachedEntity;
        }

        public static string EntitySetName<T>(this IObjectContextAdapter contextAdapter, T entity)
                   where T : class
        {
            var context = contextAdapter.ObjectContext;
            return context.CreateObjectSet<T>().EntitySet.Name;
        }

        public static EntityKeyMember[] CreateEntityKey<T>(this IObjectContextAdapter contextAdapter, T entity)
           where T : class
        {
            var context = contextAdapter.ObjectContext;
            var entityKey=context.CreateEntityKey(context.EntitySetName(entity), entity);
            if (entityKey==null)
                throw new Exception("No key found!");
            return entityKey.EntityKeyValues;
        }
    }

    public static class MultipleResultSets
    {
        public static MultipleResultSetWrapper MultipleResults(this DbContext db, string storedProcedure, List<SqlParameter> parameters = null)
        {
            return new MultipleResultSetWrapper(db, storedProcedure, parameters);
        }

        public class MultipleResultSetWrapper
        {
            private readonly DbContext _db;
            private readonly string _storedProcedure;
            private readonly List<SqlParameter> _params;

            public List<Func<IObjectContextAdapter, DbDataReader, IEnumerable>> _resultSets;

            public MultipleResultSetWrapper(DbContext db, string storedProcedure, List<SqlParameter> parameters)
            {
                _db = db;
                _storedProcedure = storedProcedure;
                _params = parameters;
                _resultSets = new List<Func<IObjectContextAdapter, DbDataReader, IEnumerable>>();
            }

            public MultipleResultSetWrapper With<TResult>()
            {
                _resultSets.Add((adapter, reader) => adapter
                    .ObjectContext
                    .Translate<TResult>(reader)
                    .ToList());

                return this;
            }

            public List<IEnumerable> Execute()
            {
                var results = new List<IEnumerable>();

                using (var connection = _db.Database.Connection)
                {
                    var command = connection.CreateCommand();
                    command.CommandText = _storedProcedure;
                    command.CommandType = CommandType.StoredProcedure;

                    if (_params != null && _params.Count > 0)
                        command.Parameters.AddRange(_params.ToArray());

                    connection.Open();
               
                    using (var reader = command.ExecuteReader())
                    {
                        var adapter = ((IObjectContextAdapter)_db);
                        foreach (var resultSet in _resultSets)
                        {
                            results.Add(resultSet(adapter, reader));
                            reader.NextResult();
                        }
                    }

                    return results;
                }
            }
        }
    }
}
