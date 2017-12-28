using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Linq.Expressions;
using ACTransit.Entities.Employee;
using ACTransit.Framework.DataAccess;
using ACTransit.Framework.Extensions;

namespace ACTransit.DataAccess.Employee.UnitOfWork
{
    public class EmployeeUnitOfWork: UnitOfWorkBase<EmployeeEntities>
    {
        public EmployeeUnitOfWork() : this(new EmployeeEntities(), null) { }
        public EmployeeUnitOfWork(EmployeeEntities context) : this(context, null) { }
        public EmployeeUnitOfWork(string currentUserName) : this(new EmployeeEntities(), currentUserName) { }
        public EmployeeUnitOfWork(EmployeeEntities context, string currentUserName) : base(context) { CurrentUserName = currentUserName; }

        //        public ObjectResult<GetEmployeesForEllipse_Result> GetEmployeesForEllipse()
        //        {
        //            return Context.GetEmployeesForEllipse();
        //        }
        //        public IEnumerable<GetEmployeesForEllipse_Result> GetEmployeesForEllipse(GetEmployeesForEllipse_Result employeeCriteria)
        //        {
        //            Expression<Func<GetEmployeesForEllipse_Result, bool>> whereClause = null;
        //            var exp = new Expression<Func<GetEmployeesForEllipse_Result, bool>>[14];
        //            if (employeeCriteria != null)
        //            {
        //                if (employeeCriteria.DeptId != null) exp[0] = m => m.DeptId != null && m.DeptId.Contains(employeeCriteria.DeptId);
        //                if (employeeCriteria.EmailAddr != null) exp[1] = m => m.EmailAddr != null && m.EmailAddr.Contains(employeeCriteria.EmailAddr);
        //                if (employeeCriteria.Empl_Status != null) exp[2] = m => m.Empl_Status != null && m.Empl_Status.Contains(employeeCriteria.Empl_Status);
        //                if (employeeCriteria.EMPLOYEE_ID != null) exp[3] = m => m.EMPLOYEE_ID != null && m.EMPLOYEE_ID.Contains(employeeCriteria.EMPLOYEE_ID);
        //                if (employeeCriteria.FirstName != null) exp[4] = m => m.FirstName != null && m.FirstName.Contains(employeeCriteria.FirstName);
        //                if (employeeCriteria.FullName != null) exp[5] = m => m.FullName != null && m.FullName.Contains(employeeCriteria.FullName);
        //                if (employeeCriteria.HireDate != null) exp[6] = m => m.HireDate != null && m.HireDate.Contains(employeeCriteria.HireDate);
        //                if (employeeCriteria.InvStrDate != null) exp[7] = m => m.InvStrDate != null && m.InvStrDate == (employeeCriteria.InvStrDate);
        //                if (employeeCriteria.JobCode != null) exp[8] = m => m.JobCode != null && m.JobCode.Contains(employeeCriteria.JobCode);
        //                if (employeeCriteria.JobTitle != null) exp[9] = m => m.JobTitle != null && m.JobTitle.Contains(employeeCriteria.JobTitle);
        //                if (employeeCriteria.ModDate != null) exp[10] = m => m.ModDate != null && m.ModDate.Contains(employeeCriteria.ModDate);
        //                if (employeeCriteria.ReHireDate != null) exp[11] = m => m.ReHireDate != null && m.ReHireDate.Contains(employeeCriteria.ReHireDate);
        //                if (employeeCriteria.Sex != null) exp[12] = m => m.Sex != null && m.Sex.Contains(employeeCriteria.Sex);
        //                if (employeeCriteria.SurName != null) exp[13] = m => m.SurName != null && m.SurName.Contains(employeeCriteria.SurName);
        //            }
        //            foreach (var t in exp)
        //            {
        //                if (t == null) continue;
        //                whereClause = whereClause == null ? t : whereClause.And(t);
        //            }
        //            if (whereClause==null)
        //                return GetEmployeesForEllipse();
        //            return GetEmployeesForEllipse().Where(whereClause.Compile());
        //        }

        public void DisableProxy()
        {
            Context.Configuration.ProxyCreationEnabled = false;
        }
    }
}
