using System.Runtime.Serialization;
using ACTransit.Contracts.Data.CusRel.LookupContract.Result;
using ACTransit.Contracts.Data.CusRel.UserContract;
using ACTransit.CusRel.Services;
using Newtonsoft.Json;

namespace ACTransit.CusRel.Models
{
    [DataContract, JsonObject(MemberSerialization.OptIn)]
    public class EditGroupContactModel
    {
        #region model Builder Objects
        
        private readonly GroupContactsResult groupContactsResult;

        #endregion

        #region SendToClient

        [DataMember]
        public GroupContact GroupContact { get; set; }

        #endregion

        #region ServerSideOnly

        public bool IsVisible { get; set; }

        #endregion

        #region Constructors

        public EditGroupContactModel() { }

        public EditGroupContactModel(ServicesProxy servicesProxy, string id)
        {
            groupContactsResult = servicesProxy.TicketService.GetGroupContacts(true);
            if (!groupContactsResult.OK) return;

            GroupContact = groupContactsResult.GetGroupContact(id);
            IsVisible = GroupContact.IsVisible.GetValueOrDefault();
        }

        #endregion

    }
}