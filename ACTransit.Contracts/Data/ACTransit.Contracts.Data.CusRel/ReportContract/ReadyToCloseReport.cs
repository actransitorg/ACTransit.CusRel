using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using ACTransit.Contracts.Data.CusRel.ReportContract.Result;
using Newtonsoft.Json;

namespace ACTransit.Contracts.Data.CusRel.ReportContract
{
    [DataContract] //, JsonObject(MemberSerialization.OptIn)
    public class ReadyToCloseReport : ReportResult
    {
        [DataMember]
        public List<ReadyToCloseReportTableItem> Items { get; set; }
    }

    [DataContract] //, JsonObject(MemberSerialization.OptIn)
    public class ReadyToCloseReportTableItem
    {
        [Display(Name = "Ticket #"), DataMember]
        public int Id { get; set; }

        [Display(Name = "Incident Date"), DataMember]
        public DateTime? IncidentAt { get; set; }

        [Display(Name = "Dept"), DataMember]
        public string GroupContact { get; set; }

        [Display(Name = "Assigned To"), DataMember]
        public string AssignedTo { get; set; }

        [Display(Name = "Reason(s)"), DataMember]
        public string Reasons { get; set; }

        [Display(Name = "Contacted By"), DataMember]
        public string ResponseBy { get; set; }

        [Display(Name = "Contacted Via"), DataMember]
        public string ResponseVia { get; set; }

        [Display(Name = "Ticket Status"), DataMember]
        public string TicketStatus { get; set; }

        [Display(Name = "Comments"), DataMember]
        public string Comments { get; set; }
    }
}