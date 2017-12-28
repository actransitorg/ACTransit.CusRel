using System.Collections.Generic;
using System.Runtime.Serialization;
using ACTransit.Contracts.Data.CusRel.LookupContract;
using Newtonsoft.Json;

namespace ACTransit.Contracts.Data.CusRel.LookupContract.Result
{
    [DataContract] //, JsonObject(MemberSerialization.OptIn)
    public class SettingsResult : Common.Result
    {
        [DataMember]
        public List<Setting> Settings { get; set; }
    }
}