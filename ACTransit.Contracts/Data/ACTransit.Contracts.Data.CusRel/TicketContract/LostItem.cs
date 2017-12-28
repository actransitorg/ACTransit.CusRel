using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace ACTransit.Contracts.Data.CusRel.TicketContract
{
    [DataContract] //, JsonObject(MemberSerialization.OptIn)
    public class LostItem
    {
        [Display(Name = "Item Category"), DataMember]
        public string Category { get; set; }

        [Display(Name = "Item Type"), DataMember]
        public string Type { get; set; }
    }
}