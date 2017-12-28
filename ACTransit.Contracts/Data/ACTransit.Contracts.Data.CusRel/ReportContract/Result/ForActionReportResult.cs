using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace ACTransit.Contracts.Data.CusRel.ReportContract.Result
{
    [DataContract] //, JsonObject(MemberSerialization.OptIn)
    public class ForActionReportResult : Common.Result
    {
        [Display(Name = "# Open Tickets"), DataMember]
        public int OpenTicketCount { get; set; }

        [Display(Name = "For Action Report"), DataMember]
        public ForActionReport Report { get; set; }
    }
}