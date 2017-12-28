using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace ACTransit.Contracts.Data.CusRel.ReportContract.Result
{
    [DataContract] //, JsonObject(MemberSerialization.OptIn)
    public class ProductivityReportResult : Common.Result
    {
        [Display(Name = "Open Tickets Report"), DataMember]
        public ProductivityReport Report { get; set; }
    }
}