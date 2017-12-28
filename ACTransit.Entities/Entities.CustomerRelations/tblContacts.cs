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
    
    public partial class tblContacts
    {
        public tblContacts()
        {
            this.tblAttachments = new HashSet<tblAttachments>();
            this.tblContactHistory = new HashSet<tblContactHistory>();
            this.tblIncidentUpdateHistory = new HashSet<tblIncidentUpdateHistory>();
            this.tblLinkedContacts = new HashSet<tblLinkedContacts>();
            this.tblLinkedContacts1 = new HashSet<tblLinkedContacts>();
            this.tblResearchHistory = new HashSet<tblResearchHistory>();
            this.tblUpdateLog = new HashSet<tblUpdateLog>();
            this.tblAttachmentsTemp = new HashSet<tblAttachmentsTemp>();
            this.tblViewHistory = new HashSet<tblViewHistory>();
        }
    
        public int FileNum { get; set; }
        public Nullable<int> FeedbackId { get; set; }
        public string ClerkId { get; set; }
        public string UserId { get; set; }
        public string ADAComplaint { get; set; }
        public string SeniorComplaint { get; set; }
        public string ContactSource { get; set; }
        public Nullable<System.DateTime> ReceivedDateTime { get; set; }
        public string Priority { get; set; }
        public Nullable<System.DateTime> ResolvedDateTime { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Addr1 { get; set; }
        public string Addr2 { get; set; }
        public string CustCity { get; set; }
        public string CustState { get; set; }
        public string CustZip { get; set; }
        public string HomePhone { get; set; }
        public string WorkPhone { get; set; }
        public string CellPhone { get; set; }
        public string Email { get; set; }
        public string RespondVia { get; set; }
        public Nullable<System.DateTime> IncidentDateTime { get; set; }
        public string VehNo { get; set; }
        public string Route { get; set; }
        public string Destination { get; set; }
        public string Location { get; set; }
        public string IncidentCity { get; set; }
        public string Badge { get; set; }
        public string EmployeeDesc { get; set; }
        public string ResponseRequested { get; set; }
        public string Division { get; set; }
        public string CustComments { get; set; }
        public string AssignedTo { get; set; }
        public string ForAction { get; set; }
        public string ForInfos { get; set; }
        public string ContactStatus { get; set; }
        public string TicketStatus { get; set; }
        public string Resolution { get; set; }
        public string ResolutionComment { get; set; }
        public string updatedBy { get; set; }
        public Nullable<System.DateTime> updatedOn { get; set; }
        public string Reasons { get; set; }
        public string TitleVI { get; set; }
        public string LostItemCategory { get; set; }
        public string LostItemType { get; set; }
        public string OperatorName { get; set; }
    
        public virtual ICollection<tblAttachments> tblAttachments { get; set; }
        public virtual ICollection<tblContactHistory> tblContactHistory { get; set; }
        public virtual ICollection<tblIncidentUpdateHistory> tblIncidentUpdateHistory { get; set; }
        public virtual ICollection<tblLinkedContacts> tblLinkedContacts { get; set; }
        public virtual ICollection<tblLinkedContacts> tblLinkedContacts1 { get; set; }
        public virtual ICollection<tblResearchHistory> tblResearchHistory { get; set; }
        public virtual ICollection<tblUpdateLog> tblUpdateLog { get; set; }
        public virtual ICollection<tblAttachmentsTemp> tblAttachmentsTemp { get; set; }
        public virtual ICollection<tblViewHistory> tblViewHistory { get; set; }
    }
}
