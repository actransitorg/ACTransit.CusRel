using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ACTransit.Contracts.Data.CusRel.UserContract;
using Newtonsoft.Json;

namespace ACTransit.Contracts.Data.CusRel.LookupContract.Result
{
    [DataContract] //, JsonObject(MemberSerialization.OptIn)
    public class GroupContactsResult : Common.Result
    {
        [DataMember]
        public List<GroupContact> GroupContacts { get; set; }

        public GroupContact GetGroupContact(string Code)
        {
            return GroupContacts.FirstOrDefault(gc => gc.Code == Code);
        }
    }
}