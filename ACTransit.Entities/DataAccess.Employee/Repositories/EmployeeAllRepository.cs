using System;
using System.Linq;
using System.Linq.Expressions;
using ACTransit.Entities.Employee;
using ACTransit.Framework.Extensions;

namespace ACTransit.DataAccess.Employee.Repositories
{
    /// <summary>
    /// List of all employee including retired and not current employees.
    /// </summary>
    public class EmployeeAllRepository:BaseRepository
    {
        public EmployeeAll GetEmployeeByBadge(string badge)
        {
            return ((EmployeeEntities)Context).EmployeeAlls.FirstOrDefault(m => m.Badge != null && m.Badge == badge);
        }

        /// <summary>
        /// Returns employees based on their badges
        /// </summary>
        /// <param name="badges">The list of Badges</param>
        /// <returns></returns>
        public IQueryable<EmployeeAll> GetEmployeesByBadges(string[] badges)
        {
            return ((EmployeeEntities)Context).EmployeeAlls.Where(m => m.Badge != null && badges.Contains(m.Badge));            
        }

        public IQueryable<EmployeeAll> GetEmployees(
            string badge = null, 
            string name=null, 
            string firstName = null,
            string lastName = null,
            string location=null,
            string division=null,
            string deptName = null,
            string ntLogin=null,
            bool? inEmployeeTable=null,
            string jobTitle=null
            )
        {
            Expression<Func<EmployeeAll, bool>> whereClause = null;
            var exp = new Expression<Func<EmployeeAll, bool>>[9];

            if (!string.IsNullOrEmpty(badge)) exp[0] = m => m.Badge != null && m.Badge.Contains(badge);
            if (!string.IsNullOrEmpty(name)) exp[1] = m => m.EmployeeName != null && m.EmployeeName.Contains(name);
            if (!string.IsNullOrEmpty(firstName)) exp[2] = m => m.FirstName != null && m.FirstName.Contains(firstName);
            if (!string.IsNullOrEmpty(lastName)) exp[3] = m => m.LastName != null && m.LastName.Contains(lastName);
            if (!string.IsNullOrEmpty(location)) exp[4] = m => m.Location!= null && m.Location.Contains(location);
            if (!string.IsNullOrEmpty(division)) exp[5] = m => m.Division != null && m.Division.Contains(division);
            if (!string.IsNullOrEmpty(deptName)) exp[6] = m => m.DeptName != null && m.DeptName.Contains(deptName);
            if (!string.IsNullOrEmpty(ntLogin)) exp[7] = m => m.NTLogin != null && m.NTLogin.Contains(ntLogin);
            if (!string.IsNullOrEmpty(jobTitle)) exp[8] = m => m.JobTitle != null && m.JobTitle.Contains(jobTitle);

            for (int i = 0; i < exp.Length; i++)
            {
                if (exp[i] != null)
                {
                    if (whereClause == null)
                        whereClause = exp[i];
                    else
                        whereClause = whereClause.And(exp[i]);
                }
            }

            return whereClause == null
                ? ((EmployeeEntities)Context).EmployeeAlls.AsQueryable()
                : ((EmployeeEntities)Context).EmployeeAlls.Where(whereClause);            
        }
     
    }    
}
