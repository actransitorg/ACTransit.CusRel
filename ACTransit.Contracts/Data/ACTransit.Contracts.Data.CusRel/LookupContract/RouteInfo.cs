using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace ACTransit.Contracts.Data.CusRel.LookupContract
{
    [DataContract] //, JsonObject(MemberSerialization.OptIn)
    public class RouteInfo
    {
        [DataMember]
        public string Route { get; set; }

        [DataMember]
        public List<string> Routes { get; set; }

        [DataMember]
        public List<string> Divisions { get; set; }

        [DataMember]
        public List<string> Directions { get; set; }
    }
}