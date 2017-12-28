using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using ACTransit.Contracts.Data.CusRel.Common;
using ACTransit.Contracts.Data.CusRel.LookupContract.Result;
using ACTransit.Contracts.Data.CusRel.UserContract;
using ACTransit.Contracts.Data.CusRel.UserContract.Result;
using ACTransit.DataAccess.CustomerRelations;
using ACTransit.DataAccess.Employee;
using ACTransit.CusRel.Repositories.Mapping;
using System.Data.Entity;
using ACTransit.DataAccess.Employee.Repositories;
using ACTransit.Entities.CustomerRelations;
using ACTransit.Entities.Employee;


namespace ACTransit.CusRel.Repositories
{
    public class UserRepository: IDisposable
    {
        private CusRelEntities cusRelContext;
        private EmployeeEntities employeeEntities;

        #region Constructors / Initialization

        public UserRepository()
        {
        }

        public UserRepository(CusRelEntities context)
        {
            cusRelContext = context;
        }

        public UserRepository(EmployeeEntities context)
        {
            employeeEntities = context;
        }

        public void InitCusRelContext()
        {
            if (cusRelContext == null)
                cusRelContext = new CusRelEntities("CusRelUserEntities");
        }

        public void InitEmployeeContext()
        {
            if (employeeEntities == null)
                employeeEntities = new EmployeeEntities();
        }

        #endregion

        // =============================================================

        #region Save/Dispose

        public int SaveChanges()
        {
            if (cusRelContext != null)
                return cusRelContext.SaveChanges();
            if (employeeEntities != null)
                return employeeEntities.SaveChanges();
            return 0;
        }

        private UserResult AddOrUpdate(User User)
        {
            var result = new UserResult
            {
                User = User, 
                Roles = User.GetRoles()
            };

            try
            {
                var user = User.ToEntities();
                cusRelContext.AuthorizedUsers.AddOrUpdate(user);
                var count = SaveChanges();
                if (count > 0)
                    result.SetOK();
            }
            catch (Exception e)
            {
                result.SetFail(e);
            }
            return result;
        }

        private Result Delete(AuthorizedUsers user)
        {
            var result = new Result();

            try
            {
                if (string.IsNullOrEmpty(user.UserId))
                    throw new Exception("UserId is required to delete.");
                cusRelContext.Entry(user).State = EntityState.Deleted;
                var count = SaveChanges();
                if (count > 0)
                    result.SetOK();
            }
            catch (Exception e)
            {
                result.SetFail(e);
            }
            return result;
        }

        private bool disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed && disposing)
            {
                if (cusRelContext != null)
                    cusRelContext.Dispose();
                if (employeeEntities != null)
                    employeeEntities.Dispose();
            }
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        // =============================================================

        #region Public Methods

        public UserResult GetUser(string UserId, bool fill = false)
        {
            try
            {
                InitCusRelContext();
                var users = GetUsers(false, fill);
                if (!users.OK)
                    throw new Exception(users.ToString());

                var user = users.Users.FirstOrDefault(u => String.Equals(u.Id, UserId, StringComparison.CurrentCultureIgnoreCase)) ??
                           users.Users.FirstOrDefault(u => String.Equals(u.Username, UserId, StringComparison.CurrentCultureIgnoreCase));

                return new UserResult
                {
                    User = user,
                    Roles = user != null ? user.GetRoles() : null
                };
            }
            catch (Exception e)
            {
                var result = new UserResult();
                result.SetFail(e);
                return result;
            }
        }

        private static string SearchBadge(string Badge)
        {
            return Badge.PadLeft(6, '0');
        }

        public OperatorResult GetOperator(string Badge)
        {
            try
            {
                InitEmployeeContext();
                var searchBadge = SearchBadge(Badge);
                return new OperatorResult
                {
                    Operator = employeeEntities.Employees.FirstOrDefault(b => b.Badge == searchBadge).ToOperator(),
                };
            }
            catch (Exception e)
            {
                var result = new OperatorResult();
                result.SetFail(e);
                return result;
            }
        }

        private List<User> usersCache;
        private List<GroupContact> groupContactsCache;

        public UsersResult GetUsers(bool byDivision = false, bool fill = false)
        {
            try
            {
                InitCusRelContext();
                if (usersCache == null)
                    usersCache = byDivision
                        ? cusRelContext.AuthorizedUsers.Where(u => u.Division != "IN").OrderBy(u => u.Division).ThenBy(u => u.UserName).ToList().Select(u => u.FromEntities()).ToList()
                        : cusRelContext.AuthorizedUsers.Where(u => u.Division != "IN").OrderBy(u => u.UserName).ThenBy(u => u.Division).ToList().Select(u => u.FromEntities()).ToList();

                if (fill && groupContactsCache == null)
                {
                    if (groupContactsCache == null)
                        groupContactsCache = cusRelContext.tblCustomerReferenceCodes.ToList().FromEntities();

                    foreach (var user in usersCache)
                        user.GroupContact = groupContactsCache.FirstOrDefault(gc => gc.Code == user.GroupContact.Code);
                }
                
                return new UsersResult
                {
                    Users = usersCache
                };
            }
            catch (Exception e)
            {
                var result = new UsersResult();
                result.SetFail(e);
                return result;
            }
        }

        public GroupContactResult GetGroupContact(GroupContact groupContact)
        {
            InitCusRelContext();
            if (groupContactsCache == null)
                groupContactsCache = cusRelContext.tblCustomerReferenceCodes.ToList().FromEntities();

            return new GroupContactResult
            {
                GroupContact = groupContactsCache.FirstOrDefault(gc => gc.Code == groupContact.Code || gc.Description == groupContact.Description || gc.Value == groupContact.Value)
            };
        }


        public UserResult SaveUser(User user)
        {
            try
            {
                InitCusRelContext();
                var result = AddOrUpdate(user);
                return result;
            }
            catch (Exception e)
            {
                var result = new UserResult();
                result.SetFail(e);
                return result;
            }
        }

        public Result DeleteUser(string userId)
        {
            try
            {
                InitCusRelContext();
                var user = cusRelContext.AuthorizedUsers.FirstOrDefault(u => u.UserId.Trim() == userId) ??
                           cusRelContext.AuthorizedUsers.FirstOrDefault(u => u.UserName.Trim() == userId);
                var result = Delete(user);
                return result;
            }
            catch (Exception e)
            {
                var result = new UserResult();
                result.SetFail(e);
                return result;
            }
        }

        public List<Employee> GetEmployees(string badge = null, string name = null, string firstName = null,
            string lastName = null, string middleName = null, DateTime? birthDate = null,
            string address = null,
            string homePhone = null, string workPhone = null, string cellPhone = null, string email = null,
            string deptName = null, string jobTitle = null, string Status = null)
        {
            InitEmployeeContext();
            using (var repository = new EmployeeRepository())
            {
                return repository.GetEmployees(
                    new Employee
                    {
                        Badge = badge,
                        Name = name,
                        FirstName = firstName,
                        LastName = lastName,
                        MiddleName = middleName,
                        BirthDate = birthDate,
                        Address01 = address,
                        PreferredPhone = homePhone,
                        WorkPhone = workPhone,
                        CellPhone = cellPhone,
                        EmailAddress = email,
                        DeptName = deptName,
                        JobTitle = jobTitle,
                        Empl_Status = Status
                    }
                ).ToList();
            }
        }

        #endregion       
    }
}
