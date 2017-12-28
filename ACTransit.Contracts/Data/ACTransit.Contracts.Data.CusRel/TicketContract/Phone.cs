using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace ACTransit.Contracts.Data.CusRel.TicketContract
{
    [DataContract] //, JsonObject(MemberSerialization.OptIn)
    public class Phone
    {
        [Display(Name = "Number"), DataMember]
        public string Number { get; set; }

        [Display(Name = "Kind"), DataMember]
        public PhoneKind Kind { get; set; }
    }

    [DataContract,]
    [Description("Phone Kind")]
    public enum PhoneKind
    {
        Home,
        Mobile,
        Work
    }
}