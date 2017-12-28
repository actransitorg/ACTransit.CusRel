using System.Runtime.Serialization;
using ACTransit.Contracts.Data.CusRel.LookupContract;
using Newtonsoft.Json;

namespace ACTransit.Contracts.Data.CusRel.LookupContract.Result
{
    [DataContract] //, JsonObject(MemberSerialization.OptIn)
    public class IncidentDriversResult : Common.Result
    {
        [DataMember]
        public IncidentDrivers IncidentDrivers { get; set; }
    }
}