using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using ACTransit.Contracts.Data.CusRel.ReportContract.Result;
using Newtonsoft.Json;

namespace ACTransit.Contracts.Data.CusRel.ReportContract
{
    [DataContract] //, JsonObject(MemberSerialization.OptIn)
    public class ForActionReport : ReportResult
    {
        [DataMember]
        public List<ForActionReportTableItem> Items { get; set; }
    }

    [DataContract] //, JsonObject(MemberSerialization.OptIn)
    public class ForActionReportTableItem
    {
        [Display(Name = "Ticket #"), DataMember]
        public int Id { get; set; }

        [Display(Name = "Status"), DataMember]
        public string Status { get; set; }

        [Display(Name = "Incident Date"), DataMember]
        public DateTime? IncidentAt { get; set; }

        [Display(Name = "Assigned"), DataMember]
        public string AssignedTo { get; set; }

        [Display(Name = "Dept"), DataMember]
        public string GroupContact { get; set; }

        [Display(Name = "Reason(s)"), DataMember]
        public string Reasons { get; set; }

        [Display(Name = "ADA"), DataMember]
        public bool IsAdaComplaint { get; set; }

        [Display(Name = "Title VI"), DataMember]
        public bool IsTitle6 { get; set; }

        [Display(Name = "Comments"), DataMember]
        public string Comments { get; set; }

        [Display(Name = "Days Open"), DataMember]
        public int DaysOpen { get; set; }

        [Display(Name = "Priority"), DataMember]
        public string Priority { get; set; }

        [Display(Name = "Route/Line"), DataMember]
        public string Route { get; set; }

        [Display(Name = "Operator"), DataMember]
        public string OperatorName { get; set; }

        [Display(Name = "Contact Via"), DataMember]
        public string ContactVia { get; set; }
    }
}