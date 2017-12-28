using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace ACTransit.Contracts.Data.CusRel.ReportContract.Result
{
    [DataContract] //, JsonObject(MemberSerialization.OptIn)
    public class RejectedReportResult : Common.Result
    {
        [Display(Name = "Rejected Report"), DataMember]
        public RejectedReport Report { get; set; }
    }
}