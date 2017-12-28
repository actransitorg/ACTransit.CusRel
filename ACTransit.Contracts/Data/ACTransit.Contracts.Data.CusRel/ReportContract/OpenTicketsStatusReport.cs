using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using ACTransit.Contracts.Data.CusRel.ReportContract.Result;
using Newtonsoft.Json;

namespace ACTransit.Contracts.Data.CusRel.ReportContract
{
    [DataContract] //, JsonObject(MemberSerialization.OptIn)
    public class OpenTicketsStatusReport : ReportResult
    {
        [DataMember]
        public List<OpenTicketsStatusReportTableItem> Items { get; set; }

        [DataMember]
        public List<OpenTicketsStatusReportTableItem> GroupItems { get; set; }
    }

    [DataContract] //, JsonObject(MemberSerialization.OptIn)
    public class OpenTicketsStatusReportTableItem
    {
        [Display(Name = "Code"), DataMember]
        public string Code { get; set; }

        [Display(Name = "Description"), DataMember]
        public string Description { get; set; }

        [Display(Name = "New"), DataMember]
        public int NewCount { get; set; }

        [Display(Name = "Assigned"), DataMember]
        public int AssignedCount { get; set; }

        [Display(Name = "Pending Contact"), DataMember]
        public int PendingContactCount { get; set; }

        [Display(Name = "Rejected"), DataMember]
        public int RejectedCount { get; set; }

        [Display(Name = "Ready to Close"), DataMember]
        public int ReadyToCloseCount { get; set; }
    }
}