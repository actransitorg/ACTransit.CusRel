using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace ACTransit.Contracts.Data.CusRel.ReportContract.Result
{
    [DataContract] //, JsonObject(MemberSerialization.OptIn)
    public class LostFoundReportResult : Common.Result
    {
        [Display(Name = "Lost and Found Report"), DataMember]
        public LostFoundReport Report { get; set; }
    }
}