using System;
using System.Collections.Generic;
using System.Linq;
using ACTransit.Contracts.Data.CusRel.Common;
using ACTransit.Contracts.Data.CusRel.LookupContract;
using ACTransit.Contracts.Data.CusRel.LookupContract.Result;
using ACTransit.Contracts.Data.CusRel.UserContract;
using ACTransit.Contracts.Data.CusRel.UserContract.Result;
using ACTransit.CusRel.Repositories;
using ACTransit.Framework.Extensions;

namespace ACTransit.CusRel.Services
{
    public class UserService: IDisposable
    {
        private readonly UserRepository userRepository = new UserRepository();

        public UserResult GetUser(string UserId, bool fill = false)
        {
            if (string.IsNullOrWhiteSpace(UserId))
                return UserResult.AsFailed();
            var idx = UserId.IndexOf(@"\");
            if (idx > -1)
                UserId = UserId.Substring(idx + 1);
            return userRepository.GetUser(UserId, fill);
        }

        public OperatorResult GetOperator(string Badge)
        {
            var result = string.IsNullOrWhiteSpace(Badge)
                ? Result<OperatorResult>.FailedResult("Not valid Badge") 
                : userRepository.GetOperator(Badge);
            if (!result.OK || result.Operator == null)
            {
                result.SetFail("Unknown Badge");
                result.Operator = null;
            }
            return result;
        }

        public UsersResult GetUsers(bool fill = false)
        {
            return userRepository.GetUsers(false, fill);
        }

        public UserResult SaveUser(User user)
        {
            if (string.IsNullOrEmpty(user.Id) && !string.IsNullOrEmpty(user.Username))
                user.Id = user.Username;
            return userRepository.SaveUser(user);
        }

        public Result DeleteUser(string userId)
        {
            return userRepository.DeleteUser(userId);
        }

        public List<Employee> GetEmployees(string badge = "", string lastName = "", string firstName = "")
        {
            var employees = userRepository.GetEmployees(badge, null, firstName, lastName).OrderBy(e => e.LastName).Select(m => new Employee
            {
                Badge = m.Badge.NullableTrim(),
                LastName = m.LastName.NullableTrim(),
                FirstName = m.FirstName.NullableTrim(),
                CellPhone = m.CellPhone.NullableTrim(),
                BusinessPhone = m.WorkPhone.NullableTrim(),
                Department = m.DeptName.NullableTrim(),
                EmailAddress = m.EmailAddress.NullableTrim()
            }).ToList();
            return employees;
        }

        public GroupContactResult GetGroupContact(GroupContact groupContact)
        {
            return userRepository.GetGroupContact(groupContact);
        }

        public void Dispose()
        {
        }
    }
}
