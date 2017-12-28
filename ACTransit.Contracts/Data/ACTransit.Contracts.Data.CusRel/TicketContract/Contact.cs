using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace ACTransit.Contracts.Data.CusRel.TicketContract
{
    [DataContract] //, JsonObject(MemberSerialization.OptIn)
    public class Contact
    {
        [Display(Name = "Name"), DataMember]
        public Name Name { get; set; }

        [Display(Name = "Address"), DataMember]
        public Address Address { get; set; }

        [Display(Name = "Email"), MinLength(6), MaxLength(50),
         RegularExpression(@"^([\S]+)@([\S]+)((\.(\S){2,})+)$", ErrorMessage = "Email is not Valid"), DataMember]
        public string Email { get; set; }

        [Display(Name = "Phone"), DataMember]
        public Phone Phone { get; set; }

        [Display(Name = "Contact Status"), DataMember]
        public string Status { get; set; }
    }

    //[DataContract]
    //[Description("Contact Status")]
    //public enum ContactStatus
    //{
    //    //[Display(Name = "Attempted - No")]
    //    //AttemptedNo,

    //    [Display(Name = "Attempted - Yes")]
    //    Attempted,

    //    [Display(Name = "Called, left message")]
    //    CalledLeftMessage,

    //    [Display(Name = "Called, no answer")]
    //    CalledNoAnswer,

    //    [Display(Name = "Called, spoke with customer")]
    //    CalledSpokeWithCustomer,

    //    [Display(Name = "Invalid Contact Info")]
    //    InvalidContactInfo,

    //    [Display(Name = "New")]
    //    New,

    //    [Display(Name = "No Attempt")]
    //    NoAttempt,

    //    [Display(Name = "Not Applicable")].
    //    NotApplicable,

    //    //[Display(Name = "Not Attempted")]
    //    //NotAttempted,

    //    [Display(Name = "Sent e-mail")]
    //    SentEmail,

    //    [Display(Name = "Sent Letter")]
    //    SentLetter,

    //    [Display(Name = "Duplicate")]
    //    Duplicate
    //}
}