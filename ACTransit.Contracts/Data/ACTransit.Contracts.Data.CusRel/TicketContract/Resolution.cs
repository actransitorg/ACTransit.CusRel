using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace ACTransit.Contracts.Data.CusRel.TicketContract
{
    [DataContract] //, JsonObject(MemberSerialization.OptIn)
    public class Resolution
    {
        [Display(Name = "Resolution Code"), DataMember]
        public string Action { get; set; }

        [Display(Name = "Resolution Comment"), DataMember]
        public string Comment { get; set; }

        [Display(Name = "Resolved At"), DataMember]
        public DateTime? ResolvedAt { get; set; }
    }
}