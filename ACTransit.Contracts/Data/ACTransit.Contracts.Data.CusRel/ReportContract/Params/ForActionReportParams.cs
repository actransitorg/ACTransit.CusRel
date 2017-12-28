using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace ACTransit.Contracts.Data.CusRel.ReportContract.Params
{
    [DataContract] //, JsonObject(MemberSerialization.OptIn)
    public class ForActionReportParams : ReportParams
    {
        [Display(Name = "Dept Contact"), DataMember]
        public string GroupContact { get; set; }
    }
}