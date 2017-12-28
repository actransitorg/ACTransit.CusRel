using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using ACTransit.Contracts.Data.CusRel.UserContract;
using Newtonsoft.Json;

namespace ACTransit.Contracts.Data.CusRel.TicketContract
{
    [DataContract] //, JsonObject(MemberSerialization.OptIn)
    public class ResponseHistory
    {
        [Key, DataMember]
        public int Id { get; set; }

        [DataMember]
        public DateTime? ResponseAt { get; set; }

        [DataMember]
        public User ResponseBy { get; set; }

        [Required, DataMember]
        public ResponseHistoryVia Via { get; set; }

        [DataMember]
        public string ViaAsString { get; set; }

        [Required, DataMember]
        public string Comment { get; set; }
    }

    [DataContract]
    public enum ResponseHistoryVia
    {
        [Display(Name = "Not Applicable")] NotApplicable,

        [Display(Name = "Invalid Contact Info")] InvalidContactInfo,

        [Display(Name = "Called, left message"), Description("Phone")] CalledLeftMessage,

        [Display(Name = "Called, no answer")] CalledNoAnswer,

        [Display(Name = "Called, spoke with customer")] CalledSpokeWithCustomer,

        [Display(Name = "Sent Email"), Description("Email")] SentEmail,

        [Display(Name = "Sent Letter"), Description("Letter")] SentLetter,

        [Display(Name = "Unknown"), Description("Unknown")] Unknown,

        [Display(Name = "New"), Description("New")] New,
    }
}