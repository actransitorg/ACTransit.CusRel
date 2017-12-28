using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace ACTransit.Contracts.Data.CusRel.TicketContract
{
    [DataContract] //, JsonObject(MemberSerialization.OptIn)
    public class Name
    {
        [Required, Display(Name = "First Name"), MaxLength(30), DataMember]
        public string First { get; set; }

        [Required, Display(Name = "Last Name"), MaxLength(30), DataMember]
        public string Last { get; set; }

        public override string ToString()
        {
            return (First + " " + Last).Trim();
        }
    }
}