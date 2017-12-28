using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using ACTransit.Contracts.Data.CusRel.UserContract;
using Newtonsoft.Json;

namespace ACTransit.Contracts.Data.CusRel.TicketContract
{
    [DataContract] //, JsonObject(MemberSerialization.OptIn)
    public class TicketSource
    {
        [Display(Name = "Received At"), DataMember]
        public DateTime? ReceivedAt { get; set; }

        [Display(Name = "Received By"), DataMember]
        public User ReceivedBy { get; set; }

        [Display(Name = "Received Via"), DataMember]
        public TicketSourceVia? Via { get; set; }

        [Display(Name = "CusRel Id"), DataMember]
        public int? FeedbackId { get; set; }
    }

    [DataContract,]
    [Description("Ticket Source Via")]
    public enum TicketSourceVia
    {
        [Display(Name = "Board of Directors")]
        BoardofDirectors,
        [Display(Name = "Comment Card")] CommentCard,
        Email,
        Letter,
        Operations,
        Phone,
        [Display(Name = "511")] Five11,
        [Display(Name = "Walk In")] WalkIn,
        WEB
    }
}