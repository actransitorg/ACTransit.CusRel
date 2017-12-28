using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web.Mvc;
using ACTransit.Contracts.Data.CusRel.UserContract;
using ACTransit.Contracts.Data.CusRel.UserContract.Result;
using ACTransit.CusRel.Infrastructure;
using ACTransit.CusRel.Services;
using Newtonsoft.Json;

namespace ACTransit.CusRel.Models
{
    [DataContract, JsonObject(MemberSerialization.OptIn)]
    public class EditUserModel : UserResult
    {
        #region model Builder Objects

        //private readonly List<string> divisions;
        private readonly IEnumerable<SelectListItem> groupContactsItems;

        #endregion

        #region SendToClient
        #endregion

        #region ServerSideOnly

        public IEnumerable<SelectListItem> Divisions
        {
            get
            {
                return User != null
                    ? new SelectList(Config.Divisions.Split(','), User.Division)
                    : null;
            }
        }

        public SelectList GroupContacts
        {
            get
            {
                return groupContactsItems != null
                    ? new SelectList(groupContactsItems, "Value", "Text", User.GroupContact != null ? User.GroupContact.Code : null)
                    : null;
            }
        }

        #endregion

        public EditUserModel() { }

        public EditUserModel(ServicesProxy servicesProxy, string id)
        {
            if (string.IsNullOrEmpty(id))
                User = new User();
            else
            {
                var userResult = servicesProxy.UserService.GetUser(id);
                MergeResults(userResult);
                User = userResult.User ?? new User();
                if (!OK) return;
            }

            var groupContactsResult = servicesProxy.TicketService.GetGroupContacts();
            if (groupContactsResult.OK)
                groupContactsItems = from s in groupContactsResult.GroupContacts
                                     select new SelectListItem
                                     {
                                         Value = s.Code,
                                         Text = s.ToString()
                                     };
        }

        //private void loadDivisions(ServicesProxy servicesProxy)
        //{
        //    var routeInfoResult = servicesProxy.TicketService.RouteInfo(null);
        //    if (routeInfoResult.OK)
        //        divisions = routeInfoResult.RouteInfo.Divisions;            
        //}
    }
}