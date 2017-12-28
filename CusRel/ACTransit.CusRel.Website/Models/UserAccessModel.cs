using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web.Mvc;
using ACTransit.Contracts.Data.CusRel.UserContract;
using ACTransit.Contracts.Data.CusRel.UserContract.Result;
using ACTransit.CusRel.Services;
using Newtonsoft.Json;

namespace ACTransit.CusRel.Models
{
    [DataContract, JsonObject(MemberSerialization.OptIn)]
    public class UserAccessModel: UsersResult
    {
        #region model Builder Objects
        #endregion

        #region SendToClient

        [DataMember]
        public User SelectedEmployee { get; set; }

        [DataMember]
        public User Header { get; set; }

        #endregion

        #region ServerSideOnly

        public IEnumerable<SelectListItem> Employees
        {
            get { return new SelectList(Users, "Id", "Username", SelectedEmployee.Id); }
        }

        #endregion

        #region Constructors

        public UserAccessModel(ServicesProxy servicesProxy, User defaultEmployee = null)
        {
            Header = new User();
            var usersResult = servicesProxy.UserService.GetUsers(true);
            Users = usersResult.Users.OrderBy(u=>u.Id).ToList();

            if (servicesProxy.RequestState != null && servicesProxy.RequestState.UserDetails != null)
                SelectedEmployee = defaultEmployee ?? usersResult.GetUser(servicesProxy.RequestState.UserDetails.Id);                

            if (SelectedEmployee == null)
                SelectedEmployee = new User();
        }

        #endregion

    }
}