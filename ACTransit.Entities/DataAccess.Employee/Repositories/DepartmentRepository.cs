using System;
using System.Linq;
using System.Linq.Expressions;
using ACTransit.Entities.Employee;
using ACTransit.Framework.Extensions;

namespace ACTransit.DataAccess.Employee.Repositories
{
    class DepartmentRepository: BaseRepository
    {
        //To fetch all Department ID data
        public Department GetDepartments()
        {
            return ((EmployeeEntities)Context).Departments.FirstOrDefault();
        }
        //To fetch all Department data particular to a ID
        public IQueryable<Department> GetDepartmentById(string deptId)
        {
            return ((EmployeeEntities)Context).Departments.Where(m => m.DepartmentId != null && m.DepartmentId.Contains(deptId));
        }
    }
}
