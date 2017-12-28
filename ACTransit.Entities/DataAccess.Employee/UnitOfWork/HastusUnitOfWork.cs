using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using ACTransit.Entities.Employee;
using ACTransit.Framework.DataAccess;

namespace ACTransit.DataAccess.Employee.UnitOfWork
{
    public class HastusUnitOfWork : UnitOfWorkBase<EmployeeEntities>
    {
        public HastusUnitOfWork() : this(new EmployeeEntities(), null) { }
        public HastusUnitOfWork(EmployeeEntities context) : this(context, null) { }
        public HastusUnitOfWork(string currentUserName) : this(new EmployeeEntities(), currentUserName) { }
        public HastusUnitOfWork(EmployeeEntities context, string currentUserName) : base(context) { CurrentUserName = currentUserName; }

        public IEnumerable<HastusEmployeeData> GetHastusEmployeeDelta(string clientId, DateTime? lastRunDate = null, int? skip = null, int? count = null, DateTime? windowEndDate = null)
        {
            var list = Context.Database.SqlQuery<HastusEmployeeData>("GetHastusEmployeeDelta @LastRunDate, @ClientId",
                new SqlParameter("@LastRunDate", 
                    lastRunDate.HasValue ? lastRunDate.Value : (Object)DBNull.Value)
                    {
                        IsNullable = true, SqlDbType = SqlDbType.DateTime
                    },
                new SqlParameter("@ClientId",
                    !string.IsNullOrEmpty(clientId) ? clientId : (Object)DBNull.Value)
                    {
                        IsNullable = false,
                        SqlDbType = SqlDbType.VarChar
                    },
                new SqlParameter("@WindowEndDate",
                    windowEndDate.HasValue ? windowEndDate.Value : (Object)DBNull.Value)
                    {
                        IsNullable = true, SqlDbType = SqlDbType.DateTime
                    }).ToList();
            var skipList = (skip.HasValue ? list.Skip(skip.Value) : list);
            var takeList = (count.HasValue ? skipList.Take(count.Value) : skipList);
            return takeList;
        }
    }


}
