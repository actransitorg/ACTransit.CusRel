using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using ACTransit.Contracts.Data.CusRel.UserContract;
using Newtonsoft.Json;

namespace ACTransit.Contracts.Data.CusRel.TicketContract
{
    [DataContract] //, JsonObject(MemberSerialization.OptIn)
    public class Assignment
    {
        [Display(Name = "Dept Contact"), DataMember]
        public GroupContact GroupContact { get; set; }

        [Display(Name = "Assigned To"), DataMember]
        public User Employee { get; set; }
    }
}