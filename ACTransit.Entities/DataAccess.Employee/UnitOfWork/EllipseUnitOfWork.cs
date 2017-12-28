using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using ACTransit.Entities.Employee;
using ACTransit.Framework.DataAccess;

namespace ACTransit.DataAccess.Employee.UnitOfWork
{
    public class EllipseUnitOfWork : UnitOfWorkBase<EmployeeEntities>
    {
        public EllipseUnitOfWork() : this(new EmployeeEntities(), null) { }
        public EllipseUnitOfWork(EmployeeEntities context) : this(context, null) { }
        public EllipseUnitOfWork(string currentUserName) : this(new EmployeeEntities(), currentUserName) { }
        public EllipseUnitOfWork(EmployeeEntities context, string currentUserName) : base(context) { CurrentUserName = currentUserName; }

        public IEnumerable<EllipseEmployeeData> GetEllipseEmployeeDelta(string clientId, DateTime? lastRunDate = null, int? skip = null, int? count = null, DateTime? windowEndDate = null)
        {
            var list = Context.Database.SqlQuery<EllipseEmployeeData>("GetEllipseEmployeeDelta @LastRunDate, @ClientId",
                new SqlParameter("@LastRunDate", 
                    lastRunDate ?? (object)DBNull.Value)
                    {
                        IsNullable = true, SqlDbType = SqlDbType.DateTime
                    },
                new SqlParameter("@ClientId",
                    !string.IsNullOrEmpty(clientId) ? clientId : (object)DBNull.Value)
                    {
                        IsNullable = false,
                        SqlDbType = SqlDbType.VarChar
                    },
                new SqlParameter("@WindowEndDate",
                    windowEndDate ?? (object)DBNull.Value)
                    {
                        IsNullable = true, SqlDbType = SqlDbType.DateTime
                    }).ToList();
            var skipList = (skip.HasValue ? list.Skip(skip.Value) : list);
            var takeList = (count.HasValue ? skipList.Take(count.Value) : skipList);
            return takeList;
        }
    }


}
