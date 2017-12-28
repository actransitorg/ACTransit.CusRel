using System.Runtime.Serialization;
using ACTransit.Contracts.Data.CusRel.UserContract;
using Newtonsoft.Json;

namespace ACTransit.Contracts.Data.CusRel.LookupContract.Result
{
    [DataContract] //, JsonObject(MemberSerialization.OptIn)
    public class GroupContactResult : Common.Result
    {
        [DataMember]
        public GroupContact GroupContact { get; set; }
    }
}