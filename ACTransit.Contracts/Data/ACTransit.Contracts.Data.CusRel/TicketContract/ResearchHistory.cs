using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using ACTransit.Contracts.Data.CusRel.UserContract;
using Newtonsoft.Json;

namespace ACTransit.Contracts.Data.CusRel.TicketContract
{
    [DataContract] //, JsonObject(MemberSerialization.OptIn)
    public class ResearchHistory
    {
        [Key, DataMember]
        public int Id { get; set; }

        [Display(Name = "Researched At"), DataMember]
        public DateTime? ResearchedAt { get; set; }

        [Display(Name = "Researched By"), DataMember]
        public User ResearchedBy { get; set; }

        [Required, Display(Name = "Comments"), DataMember]
        public string Comment { get; set; }
    }
}