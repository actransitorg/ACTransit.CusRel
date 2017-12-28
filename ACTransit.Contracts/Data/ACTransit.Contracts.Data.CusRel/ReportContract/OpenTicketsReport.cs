using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using ACTransit.Contracts.Data.CusRel.ReportContract.Result;
using Newtonsoft.Json;

namespace ACTransit.Contracts.Data.CusRel.ReportContract
{
    [DataContract] //, JsonObject(MemberSerialization.OptIn)
    public class OpenTicketsReport : ReportResult
    {
        [DataMember]
        public List<OpenTicketsReportTableItem> Items { get; set; }

        [DataMember]
        public List<OpenTicketsReportTableItem> GroupItems { get; set; }
    }

    [DataContract] //, JsonObject(MemberSerialization.OptIn)
    public class OpenTicketsReportTableItem
    {
        [Display(Name = "Code"), DataMember]
        public string Code { get; set; }

        [Display(Name = "Description"), DataMember]
        public string Description { get; set; }

        [Display(Name = "New"), DataMember]
        public int NewCount { get; set; }

        [Display(Name = "Working"), DataMember]
        public int WorkingCount { get; set; }

        [Display(Name = "Over 30"), DataMember]
        public int Over30Count { get; set; }

        [Display(Name = "Total"), DataMember]
        public int TotalCount { get; set; }
    }
}