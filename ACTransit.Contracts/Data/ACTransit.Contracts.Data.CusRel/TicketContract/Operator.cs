using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace ACTransit.Contracts.Data.CusRel.TicketContract
{
    [DataContract] //, JsonObject(MemberSerialization.OptIn)
    public class Operator
    {
        [Display(Name = "Badge Number"), Description("Operator Badge Number"),
         RegularExpression(@"\d{3,6}", ErrorMessage = "Must be 3 to 6 digit number, leading zeros optional."), DataMember]
        public string Badge { get; set; }

        [Display(Name = "Name"), Description("Operator Name"), DataMember]
        public string Name { get; set; }

        [Display(Name = "Info"), Description("Operator Info"), DataMember]
        public string Info { get; set; }

        [Display(Name = "Description"), Description("Operator Description"), MaxLength(1024), DataMember]
        public string Description { get; set; }
    }
}