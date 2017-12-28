using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace ACTransit.Contracts.Data.CusRel.ReportContract.Result
{
    [DataContract] //, JsonObject(MemberSerialization.OptIn)
    public class ReadyToCloseReportResult : Common.Result
    {
        [Display(Name = "Ready To Close Report"), DataMember]
        public ReadyToCloseReport Report { get; set; }
    }
}