using System.Collections.Generic;
using ACTransit.Contracts.Data.CusRel.TicketContract;
using ACTransit.Contracts.Data.CusRel.UserContract;
using ACTransit.Entities.CustomerRelations;
using ACTransit.Entities.Employee;

namespace ACTransit.CusRel.Repositories.Mapping
{
    public static class UserConversionExtensions
    {
        public static User FromEntities(this AuthorizedUsers User)
        {
            return User == null ? null : new User
            {
                Id = User.UserId,
                Username = User.UserName,
                Name = User.UserName,
                Email = User.Email,
                Division = User.Division,
                GroupContact = new GroupContact {Code = User.ReferTo},
                CanAddTicketComments = User.AddComments == "Y",
                CanAssignTicket = User.AllowAssignTo == "Y",
                CanAccessAdmin = User.AllowSecurity == "Y",
                CanCloseTicket = User.AllowCloseTicket == "Y",
                CanSearchTickets = User.AllowSearchTickets == "Y",
                CanViewUnassigned = User.AllowViewUnassigned == "Y",
                GetsNotificationOnAssignment = User.NotifyWhenAssigned == "Y",
                DaysReminderNotification = User.ReminderDelayDays,
                CanViewOnlyDeptTickets = User.AllowOnlyDeptTickets == "Y"
            };
        }

        public static AuthorizedUsers ToEntities(this User User)
        {
            return User == null ? null : new AuthorizedUsers
            {
                UserId = User.Id,
                UserName = User.Username,
                ClerkId = null,
                AddComments = User.CanAddTicketComments ? "Y" : "N",
                AllowAssignTo = User.CanAssignTicket ? "Y" : "N",
                AllowSecurity = User.CanAccessAdmin ? "Y" : "N",
                AllowCloseTicket = User.CanCloseTicket ? "Y" : "N",
                AllowSearchTickets = User.CanSearchTickets ? "Y" : "N",
                AllowViewUnassigned = User.CanViewUnassigned ? "Y" : "N",
                NotifyWhenAssigned = User.GetsNotificationOnAssignment ? "Y" : "N",
                ReminderDelayDays = User.DaysReminderNotification,
                AllowOnlyDeptTickets = User.CanViewOnlyDeptTickets ? "Y" : "N",
                Division = User.Division,
                ReferTo = User.GroupContact.Code,
                Email = User.Email,
            };
        }

        public static string[] GetRoles(this User User)
        {
            var roles = new List<string>();
            if (User != null)
            {
                if (!User.CanAddTicketComments && !User.CanAssignTicket)
                    roles.Add("CusRelReadOnly");
                if (User.CanAddTicketComments && !User.CanAssignTicket)
                    roles.Add("CusRelCommentOnly");
                if (User.CanAddTicketComments && User.CanAssignTicket)
                    roles.Add("CusRelUser");
                if (User.CanAccessAdmin)
                    roles.Add("CusRelAdmin");
            }
            return roles.ToArray();
        }

        public static Operator ToOperator(this Employee Employee)
        {
            if (Employee == null) return null;

            return new Operator
            {
                Badge = Employee.Badge,
                Name = Employee.FullName(),
                Info = string.Format("{1} / {2} / {0}",
                    Employee.Sex.Trim(),
                    Employee.JobTitle.Trim(),
                    Employee.DeptName.Trim().ToUpper()
                )
            };
        }

        public static string FullName(this Employee Employee)
        {
            if (Employee == null) return null;
            var name = "";
            if (!string.IsNullOrWhiteSpace(Employee.LastName))
                name += Employee.LastName.Trim().ToUpper();
            if (!string.IsNullOrWhiteSpace(Employee.Suffix))
                name += (name != "" ? " " : "") + Employee.Suffix.Trim().ToUpper();
            if (!string.IsNullOrWhiteSpace(Employee.FirstName))
                name += (name != "" ? ", " : "") + Employee.FirstName.Trim().ToUpper();
            if (!string.IsNullOrWhiteSpace(Employee.MiddleName))
                name += (name.Contains(",") ? " " : ", ") + Employee.MiddleName.Trim().ToUpper();
            return name;
        }

    }
}