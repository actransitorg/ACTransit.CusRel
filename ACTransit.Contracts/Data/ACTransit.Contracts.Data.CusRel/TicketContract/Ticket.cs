using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using ACTransit.Contracts.Data.CusRel.UserContract;
using Newtonsoft.Json;

namespace ACTransit.Contracts.Data.CusRel.TicketContract
{
    [DataContract] //, JsonObject(MemberSerialization.OptIn)
    public class Ticket
    {
        /// <summary>
        ///     When constructing a new ticket, should it contain 'new ticket' defaults or completely empty (zeros and nulls)?
        /// </summary>
        public enum EmptyTicketEnum
        {
            Blank,
            NewTicket
        }

        public enum TicketAction
        {
            Create,
            Update
        }

        [NotMapped] public TicketChanges Changes;

        [Key, Display(Name = "Ticket #"), Description("Ticket #"), DataMember]
        public int Id { get; set; }

        [Display(Name = "Status"), Description("Ticket Status"), EnumDataType(typeof (TicketStatus)), DataMember]
        public TicketStatus? Status { get; set; }

        [Display(Name = "Priority"), Description("Ticket Priority"), EnumDataType(typeof (TicketPriority)), DataMember]
        public TicketPriority? Priority { get; set; }

        [DataMember]
        public TicketSource Source { get; set; }

        [DataMember]
        public Contact Contact { get; set; }

        [Display(Name = "ADA?"), Description("ADA?"), DataMember]
        public bool? IsAdaComplaint { get; set; }

        [Display(Name = "Title VI?"), Description("Title VI?"), DataMember]
        public bool? IsTitle6 { get; set; }

        [DataMember]
        public Incident Incident { get; set; }

        [DataMember]
        public Operator Operator { get; set; }

        [DataMember]
        public LostItem LostItem { get; set; }

        [Display(Name = "Reasons"), DataMember]
        public virtual List<string> Reasons { get; set; }

        [Range(0, 256), NotMapped]
        public int ReasonsLength
        {
            get { return Reasons == null ? 0 : Reasons.Sum(r => r != null ? r.Length : 0); }
        }

        [Required, Display(Name = "Reason Code 1"), MinLength(1, ErrorMessage = "Not optional field."), NotMapped]
        public string ReasonCode1
        {
            get { return Reasons != null && Reasons.Count >= 1 ? Reasons[0] : null; }
            set
            {
                if (Reasons == null)
                    Reasons = new List<string> {value};
                else
                {
                    if (Reasons.Count > 0)
                        Reasons.RemoveAt(0);
                    Reasons.Insert(0, value);
                }
            }
        }

        [Display(Name = "Reason Code 2"), NotMapped]
        public string ReasonCode2
        {
            get { return Reasons != null && Reasons.Count > 1 ? Reasons[1] : null; }
            set
            {
                if (Reasons == null)
                    Reasons = new List<string> {null, value};
                else
                {
                    if (Reasons.Count > 1)
                        Reasons.RemoveAt(1);
                    if (Reasons.Count == 0)
                        Reasons.Insert(0, null);
                    Reasons.Insert(1, value);
                }
            }
        }


        [Display(Name = "Reason Code 3"), NotMapped]
        public string ReasonCode3
        {
            get { return Reasons != null && Reasons.Count > 2 ? Reasons[2] : null; }
            set
            {
                if (Reasons == null)
                    Reasons = new List<string> {null, value};
                else
                {
                    if (Reasons.Count > 2)
                        Reasons.RemoveAt(2);
                    if (Reasons.Count == 0)
                        Reasons.Insert(0, null);
                    if (Reasons.Count == 1)
                        Reasons.Insert(1, null);
                    Reasons.Insert(2, value);
                }
            }
        }

        //[AllowHtml]
        [Required, Display(Name = "Comments"), DataMember]
        public string Comments { get; set; }

        [DataMember]
        public ResponseCriteria ResponseCriteria { get; set; }

        [DataMember]
        public Resolution Resolution { get; set; }

        [DataMember]
        public Assignment Assignment { get; set; }

        [DataMember]
        public virtual List<ResearchHistory> ResearchHistory { get; set; }

        [DataMember]
        public virtual List<ResponseHistory> ResponseHistory { get; set; }

        [DataMember]
        public virtual List<Ticket> LinkedTickets { get; set; }

        [DataMember]
        public virtual List<Attachment> Attachments { get; set; }

        [DataMember]
        public virtual List<ChangeHistory> ChangeHistory { get; set; }

        [Display(Name = "Last Updated At"), DataMember]
        public DateTime? UpdatedAt { get; set; }

        [Display(Name = "Last Updated By"), DataMember]
        public User UpdatedBy { get; set; }

        [DataMember]
        public bool ShouldUnlink { get; set; }

        [DataMember]
        public bool ShouldIgnore { get; set; }

        [DataMember]
        public int? LinkedId { get; set; }

        [Display(Name = "Days Open"), DataMember]
        public int DaysOpen { get; set; }

        public bool IsOpen
        {
            get
            {
                return new[]
                {
                    TicketStatus.New, 
                    TicketStatus.Rejected, 
                    TicketStatus.Redirected, 
                    TicketStatus.Assigned, 
                    TicketStatus.PendingContact, 
                    TicketStatus.ReadyToClose, 
                    TicketStatus.ReadyToCloseTooLate, 
                    TicketStatus.ReadyToCloseDuplicate
                }.Any(s => s.Equals(Status));
            }
        }


        public static Ticket EmptyTicket(EmptyTicketEnum emptyTicket = EmptyTicketEnum.Blank)
        {
            var result = new Ticket();
            result.Empty(emptyTicket);
            return result;
        }

        public void Empty(EmptyTicketEnum emptyTicket = EmptyTicketEnum.Blank)
        {
            bool isNew = emptyTicket == EmptyTicketEnum.NewTicket;
            Source = new TicketSource
            {
                Via = isNew ? (TicketSourceVia?) TicketSourceVia.Phone : null,
                ReceivedBy = new User(isNew ? "WEB" : null),
                ReceivedAt = isNew ? (DateTime?) DateTime.Now : null,
                FeedbackId = isNew ? (int?) new Random().Next() : null
            };
            Contact = new Contact
            {
                Name = new Name(),
                Address = new Address
                {
                    State = isNew ? "CA" : null
                },
                Phone = new Phone
                {
                    Kind = PhoneKind.Home
                },
                Status = null // isNew ? (ContactStatus?) ContactStatus.New : null
            };
            Incident = new Incident();
            Operator = new Operator();
            LostItem = new LostItem();
            ResponseCriteria = new ResponseCriteria();
            Resolution = new Resolution();
            Assignment = new Assignment
            {
                GroupContact = new GroupContact(),
                Employee = new User()
            };
            UpdatedBy = new User();
            Status = isNew ? (TicketStatus?) TicketStatus.New : null;
            Priority = isNew ? (TicketPriority?) TicketPriority.Normal : null;
        }
    }

    [DataContract]
    [Description("Ticket Status")]
    public enum TicketStatus
    {
        New,
        Assigned,
        Rejected,
        Redirected,
        [Display(Name = "Pending Contact")]
        PendingContact,
        [Display(Name = "Ready To Close - Completed")]
        ReadyToClose,
        [Display(Name = "Ready To Close - Too Late")]
        ReadyToCloseTooLate,
        [Display(Name = "Ready To Close - Duplicate")]
        ReadyToCloseDuplicate,

        [Display(Name = "Closed - Completed")]
        Closed,
        [Display(Name = "Closed - Too Late")]
        ClosedTooLate,
        [Display(Name = "Closed - Duplicate")]
        ClosedDuplicate,
    }

    [DataContract]
    [Description("Ticket Priority")]
    public enum TicketPriority
    {
        Normal,
        Low,
        High
    }

    public class TicketChanges
    {
        public bool AssignmentEmployee;
        public bool AssignmentGroupContact;
        public bool TicketPriority;
    }
}