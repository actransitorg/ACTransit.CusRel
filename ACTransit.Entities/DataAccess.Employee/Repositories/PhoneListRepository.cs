using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ACTransit.Entities.Employee;
using ACTransit.Framework.Extensions;

namespace ACTransit.DataAccess.Employee.Repositories
{
    public class PhoneListRepository:BaseRepository
    {
        public IQueryable<VContacts_list> GetContacts(string badge=null, string firstName=null, string lastName=null, string prefName = null, string fullName=null, string deptartment=null, string location=null, string businessPhone=null, string cellPhone=null, string email =null, string supervisorName= null)
        {
            Expression<Func<VContacts_list, bool>> whereClause = null;
            var exp = new Expression<Func<VContacts_list, bool>>[11];

            if (badge != null) exp[0] = m => m.Badge.Contains(badge);
            if (firstName != null) exp[1] = m => m.FirstName.Contains(firstName);
            if (lastName != null) exp[2]= m => m.LastName.Contains(lastName);
            if (prefName != null) exp[3] = m => m.Pref_Name.Contains(prefName);
            if (fullName != null) exp[4] = m => m.FullName.Contains(fullName);
            if (deptartment != null) exp[5] = m => m.Department.Contains(deptartment);
            if (location != null) exp[6] = m => m.Location.Contains(location);
            if (businessPhone != null) exp[7] = m => m.BusinessPhone.Contains(businessPhone);
            if (cellPhone != null) exp[8] = m => m.CellPhone.Contains(cellPhone);
            if (email != null) exp[9] = m => m.Email.Contains(email);
            if (supervisorName != null) exp[10] = m => m.Email.Contains(supervisorName);
            

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

            var contacts = ((EmployeeEntities) Context).VContacts_list.OrderBy(c => c.FullName);

            return whereClause == null
                ? contacts
                : contacts.Where(whereClause);
        }
    }
}