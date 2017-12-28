using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace ACTransit.Contracts.Data.CusRel.ReportContract.Result
{
    [DataContract] //, JsonObject(MemberSerialization.OptIn)
    public class AssignedToReportResult : Common.Result
    {
        [Display(Name = "# Open Tickets"), DataMember]
        public int OpenTicketCount { get; set; }

        [Display(Name = "Assigned To Report"), DataMember]
        public AssignedToReport Report { get; set; }
    }
}