using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using ACTransit.Contracts.Data.CusRel.ReportContract.Result;
using Newtonsoft.Json;

namespace ACTransit.Contracts.Data.CusRel.ReportContract
{
    [DataContract] //, JsonObject(MemberSerialization.OptIn)
    public class ProductivityReport : ReportResult
    {
        [DataMember]
        public List<ProductivityReportTableItem> Items { get; set; }

        [DataMember]
        public List<ProductivityReportTableItem> GroupItems { get; set; }
    }

    [DataContract] //, JsonObject(MemberSerialization.OptIn)
    public class ProductivityReportTableItem
    {
        [Display(Name = "Code"), DataMember]
        public string Code { get; set; }

        [Display(Name = "Description"), DataMember]
        public string Description { get; set; }

        [Display(Name = "Total Received"), DataMember]
        public int ReceivedCount { get; set; }

        [Display(Name = "Total Closed"), DataMember]
        public int ClosedCount { get; set; }
    }
}