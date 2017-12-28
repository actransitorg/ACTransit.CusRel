using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace ACTransit.Contracts.Data.CusRel.LookupContract
{
    [DataContract] //, JsonObject(MemberSerialization.OptIn)
    public class IncidentDrivers
    {
        [Display(Name = "Incident At"), DataMember]
        public DateTime? IncidentAt { get; set; }

        [DataMember]
        public List<IncidentDriver> Drivers { get; set; }
    }

    [DataContract] //, JsonObject(MemberSerialization.OptIn)
    public class IncidentDriver
    {
        [DataMember]
        public string Badge { get; set; }

        [DataMember]
        public string Route { get; set; }
    }
}