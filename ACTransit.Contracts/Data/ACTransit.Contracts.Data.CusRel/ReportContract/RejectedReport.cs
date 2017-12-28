using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using ACTransit.Contracts.Data.CusRel.ReportContract.Result;
using Newtonsoft.Json;

namespace ACTransit.Contracts.Data.CusRel.ReportContract
{
    [DataContract] //, JsonObject(MemberSerialization.OptIn)
    public class RejectedReport : ReportResult
    {
        [DataMember]
        public List<RejectedReportTableItem> Items { get; set; }
    }

    [DataContract] //, JsonObject(MemberSerialization.OptIn)
    public class RejectedReportTableItem
    {
        [Display(Name = "Ticket #"), DataMember]
        public int Id { get; set; }

        [Display(Name = "Incident Date"), DataMember]
        public DateTime? IncidentAt { get; set; }

        [Display(Name = "Dept"), DataMember]
        public string GroupContact { get; set; }

        [Display(Name = "Reason"), DataMember]
        public string Reasons { get; set; }

        [Display(Name = "Last Research Comments"), DataMember]
        public string LastResearchComments { get; set; }

        [Display(Name = "Comments"), DataMember]
        public string Comments { get; set; }
    }
}