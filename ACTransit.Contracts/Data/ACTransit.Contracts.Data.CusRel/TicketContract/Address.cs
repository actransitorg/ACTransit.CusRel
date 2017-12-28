using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace ACTransit.Contracts.Data.CusRel.TicketContract
{
    [DataContract] //, JsonObject(MemberSerialization.OptIn)
    public class Address
    {
        [Display(Name = "Address 1"), MaxLength(30), DataMember]
        public string Addr1 { get; set; }

        [Display(Name = "Address 2"), MaxLength(30), DataMember]
        public string Addr2 { get; set; }

        [Display(Name = "City"), MaxLength(30), DataMember]
        public string City { get; set; }

        [Display(Name = "State"), MaxLength(2), DataMember]
        public string State { get; set; }

        [Display(Name = "Zip Code"), MaxLength(10), DataMember]
        public string ZipCode { get; set; }
    }
}