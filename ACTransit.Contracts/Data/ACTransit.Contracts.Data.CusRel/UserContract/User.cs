using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace ACTransit.Contracts.Data.CusRel.UserContract
{
    [DataContract] //, JsonObject(MemberSerialization.OptIn)
    public class User
    {
        public User()
        {
        }

        public User(string Id)
        {
            this.Id = Id;
        }

        public User(string Id, string Name = null, string Username = null)
        {
            this.Id = Id;
            this.Name = Name;
            this.Username = Username;
        }

        [DataMember, MaxLength(16)]
        public string Id { get; set; }

        [DataMember, MaxLength(30)]
        public string Username { get; set; }

        [Display(Name = "Full Name"), DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string Division { get; set; }

        [Display(Name = "Dept Contact"), DataMember]
        public GroupContact GroupContact { get; set; }

        [Display(Name = "Add Ticket Comments?"), NotMapped, JsonIgnore]
        public bool CanAddTicketComments { get; set; }

        [Display(Name = "Assign Tickets?"), NotMapped, JsonIgnore]
        public bool CanAssignTicket { get; set; }

        [Display(Name = "Access Admin?"), NotMapped, JsonIgnore]
        public bool CanAccessAdmin { get; set; }

        [Display(Name = "Allow Close Ticket?"), NotMapped, JsonIgnore]
        public bool CanCloseTicket { get; set; }

        [Display(Name = "Allow Search Tickets?"), NotMapped, JsonIgnore]
        public bool CanSearchTickets { get; set; }

        [Display(Name = "Allow View Unassigned?"), NotMapped, JsonIgnore]
        public bool CanViewUnassigned { get; set; }

        [Display(Name = "Notify When Assigned?"), NotMapped, JsonIgnore]
        public bool GetsNotificationOnAssignment { get; set; }

        [Display(Name = "Reminder Delay Days"), NotMapped, JsonIgnore]
        public byte DaysReminderNotification { get; set; }

        [Display(Name = "Allow View Only Dept Tickets?"), NotMapped, JsonIgnore]
        public bool CanViewOnlyDeptTickets { get; set; }
    }
}