//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ACTransit.Entities.CustomerRelations
{
    using System;
    using System.Collections.Generic;
    
    public partial class AuthorizedUsers
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string ClerkId { get; set; }
        public string AddComments { get; set; }
        public string AllowAssignTo { get; set; }
        public string AllowSecurity { get; set; }
        public string Division { get; set; }
        public string ReferTo { get; set; }
        public string Email { get; set; }
        public string AllowCloseTicket { get; set; }
        public string AllowSearchTickets { get; set; }
        public string AllowViewUnassigned { get; set; }
        public string NotifyWhenAssigned { get; set; }
        public byte ReminderDelayDays { get; set; }
        public string AllowOnlyDeptTickets { get; set; }
    }
}