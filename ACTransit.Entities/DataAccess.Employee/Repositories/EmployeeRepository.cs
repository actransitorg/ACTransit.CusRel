using System;
using System.Linq;
using System.Linq.Expressions;
using ACTransit.Framework.Extensions;
using EM=ACTransit.Entities.Employee;

namespace ACTransit.DataAccess.Employee.Repositories
{
    public class EmployeeRepository:BaseRepository
    {
        public EM.Employee GetEmployee(int empId)
        {
            return ((EmployeeEntities)Context).Employees.FirstOrDefault(m => m.Emp_Id == empId);
        }


        /// <summary>
        /// Returns employee based on Lan Id
        /// </summary>
        /// <param name="lanId">The LanId (NTLogin)</param>
        /// <returns></returns>
        public EM.Employee GetEmployee(string lanId)
        {
            return ((EmployeeEntities)Context).Employees.FirstOrDefault(m => m.NTLogin != null && m.NTLogin == lanId);
        }

        public EM.Employee GetEmployeeByBadge(string badge)
        {
            return ((EmployeeEntities)Context).Employees.FirstOrDefault(m => m.Badge != null && m.Badge == badge);
        }

        /// <summary>
        /// Returns employees based on their Lan Id
        /// </summary>
        /// <param name="lanIds">The list of all LanIds (NTLogin)</param>
        /// <returns></returns>
        public IQueryable<EM.Employee> GetEmployees(string[] lanIds)
        {
            return ((EmployeeEntities)Context).Employees.Where(m =>  m.NTLogin != null && lanIds.Contains(m.NTLogin));
        }

        /// <summary>
        /// Returns employees based on their badges
        /// </summary>
        /// <param name="badges">The list of all Badges (EmployeeId )</param>
        /// <returns></returns>
        public IQueryable<EM.Employee> GetEmployeesByBadges(string[] badges)
        {
            return ((EmployeeEntities)Context).Employees.Where(m => m.Badge != null && badges.Contains(m.Badge));
        }

        public IQueryable<EM.Employee> GetEmployees(string badge = null, string name = null, string firstName = null,
            string lastName = null, string middleName = null, DateTime? birthDate = null,
            string address = null,
            string preferredPhone = null, string workPhone = null, string cellPhone = null, string email = null,
            string deptName = null, string jobTitle = null, string ntLogin = null, string prefName = null)
        {
            Expression<Func<EM.Employee, bool>> whereClause = null;
            var exp = new Expression<Func<EM.Employee, bool>>[15];

            if (badge != null) exp[0] = m => m.Badge != null && m.Badge.Contains(badge);
            if (name != null) exp[1] = m => m.Name != null && m.Name.Contains(name);
            if (firstName != null) exp[2] = m => m.FirstName != null && m.FirstName.Contains(firstName);
            if (lastName != null) exp[3] = m => m.LastName != null && m.LastName.Contains(lastName);
            if (middleName != null) exp[4] = m => m.MiddleName != null && m.MiddleName.Contains(middleName);
            if (birthDate != null)
                exp[5] = m => m.BirthDate != null && m.BirthDate != null && m.BirthDate.Value == birthDate.Value;
            if (address != null)
                exp[6] = m => (m.Address01 != null && m.Address01.Contains(address)) ||
                              (m.Address02 != null && m.Address02.Contains(address)) ||
                              (m.City != null && m.City.Contains(address)) ||
                              (m.State != null && m.State.Contains(address));

            if (preferredPhone != null) exp[7] = m => m.PreferredPhone != null && m.PreferredPhone.Contains(preferredPhone);
            if (cellPhone != null) exp[8] = m => m.CellPhone != null && m.CellPhone.Contains(cellPhone);
            if (workPhone != null) exp[9] = m => m.WorkPhone != null && m.WorkPhone.Contains(workPhone);
            if (email != null) exp[10] = m => m.EmailAddress != null && m.EmailAddress.Contains(email);
            if (deptName != null) exp[11] = m => m.DeptName != null && m.DeptName.Contains(deptName);
            if (jobTitle != null) exp[12] = m => m.JobTitle != null && m.JobTitle.Contains(jobTitle);
            if (ntLogin != null) exp[13] = m => m.NTLogin != null && m.NTLogin.Contains(ntLogin);
            if (prefName != null) exp[14] = m => m.Pref_Name != null && m.Pref_Name.Contains(prefName);

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

            var contacts = ((EmployeeEntities) Context).Employees.OrderBy(c => c.Name);

            return whereClause == null
                ? contacts
                : contacts.Where(whereClause);
        }

        public IQueryable<EM.Employee> GetEmployees(EM.Employee employeeCriteria)
        {
            Expression<Func<EM.Employee, bool>> whereClause = null;
            var exp = new Expression<Func<EM.Employee, bool>>[20];

            if (employeeCriteria.Badge != null) exp[0] = m => m.Badge != null && m.Badge.Contains(employeeCriteria.Badge);
            if (employeeCriteria.Name != null) exp[1] = m => m.Name != null && m.Name.Contains(employeeCriteria.Name);
            if (employeeCriteria.FirstName != null) exp[2] = m => m.FirstName != null && m.FirstName.Contains(employeeCriteria.FirstName);
            if (employeeCriteria.LastName != null) exp[3] = m => m.LastName != null && m.LastName.Contains(employeeCriteria.LastName);
            if (employeeCriteria.MiddleName != null) exp[4] = m => m.MiddleName != null && m.MiddleName.Contains(employeeCriteria.MiddleName);
            if (employeeCriteria.BirthDate != null) exp[5] = m => m.BirthDate != null && m.BirthDate != null && m.BirthDate.Value == employeeCriteria.BirthDate.Value;
            if (employeeCriteria.Address01 != null) exp[6] = m => m.Address01 != null && m.Address01.Contains(employeeCriteria.Address01);
            if (employeeCriteria.Address02 != null) exp[7] = m => m.Address02 != null && m.Address02.Contains(employeeCriteria.Address02);
            if (employeeCriteria.City != null) exp[8] = m => m.City != null && m.City.Contains(employeeCriteria.City);
            if (employeeCriteria.State != null) exp[9] = m => m.State != null && m.State.Contains(employeeCriteria.State);
            if (employeeCriteria.PreferredPhone != null) exp[10] = m => m.PreferredPhone != null && m.PreferredPhone.Contains(employeeCriteria.PreferredPhone);
            if (employeeCriteria.CellPhone != null) exp[11] = m => m.CellPhone != null && m.CellPhone.Contains(employeeCriteria.CellPhone);
            if (employeeCriteria.WorkPhone != null) exp[12] = m => m.WorkPhone != null && m.WorkPhone.Contains(employeeCriteria.WorkPhone);
            if (employeeCriteria.EmailAddress != null) exp[13] = m => m.EmailAddress != null && m.EmailAddress.Contains(employeeCriteria.EmailAddress);
            if (employeeCriteria.DeptName != null) exp[14] = m => m.DeptName != null && m.DeptName.Contains(employeeCriteria.DeptName);
            if (employeeCriteria.JobTitle != null) exp[15] = m => m.JobTitle != null && m.JobTitle.Contains(employeeCriteria.JobTitle);
            if (employeeCriteria.NTLogin != null) exp[16] = m => m.NTLogin != null && m.NTLogin.Contains(employeeCriteria.NTLogin);
            if (employeeCriteria.Empl_Status != null) exp[17] = m => m.Empl_Status != null && m.Empl_Status == employeeCriteria.Empl_Status;

            foreach (var t in exp)
            {
                if (t == null) continue;
                whereClause = whereClause == null ? t : whereClause.And(t);
            }

            var contacts = ((EmployeeEntities)Context).Employees.AsQueryable();
            if (whereClause != null)
                contacts = contacts.Where(whereClause);

            return contacts.OrderBy(c => c.Name);
        }

        public int ExecuteSql(string sql)
        {
            return ((EmployeeEntities) Context).Database.ExecuteSqlCommand(sql);
        }
    }
}
