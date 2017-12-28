using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web.Mvc;
using ACTransit.Contracts.Data.CusRel.LookupContract.Result;
using ACTransit.Contracts.Data.CusRel.UserContract;
using ACTransit.CusRel.Services;
using Newtonsoft.Json;

namespace ACTransit.CusRel.Models
{
    [DataContract, JsonObject(MemberSerialization.OptIn)]
    public class GroupContactModel: GroupContactsResult
    {
        #region model Builder Objects
        #endregion

        #region SendToClient

        [DataMember]
        public GroupContact GroupContact { get; set; }

        [DataMember]
        public GroupContact Header { get; set; }

        #endregion

        #region ServerSideOnly

        public IEnumerable<SelectListItem> Groups
        {
            get { return new SelectList(GroupContacts, "Code", "Description", GroupContact.Code); }
        }

        #endregion

        #region Constructors

        public GroupContactModel(ServicesProxy servicesProxy)
        {
            Header = new GroupContact();
            var groupsResult = servicesProxy.TicketService.GetGroupContacts(true);
            if (!groupsResult.OK) return;
            GroupContacts = groupsResult.GroupContacts.OrderBy(c=>c.Description).ToList();
        }

        #endregion

    }
}