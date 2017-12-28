using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using ACTransit.Contracts.Data.CusRel.UserContract;
using Newtonsoft.Json;

namespace ACTransit.Contracts.Data.CusRel.ReportContract.Params
{
    [DataContract] //, JsonObject(MemberSerialization.OptIn)
    public class AssignedToReportParams : ReportParams
    {
        public AssignedToReportParams()
        {
            AssignedTo = new User();
        }

        [Display(Name = "Assigned To"), DataMember]
        public User AssignedTo { get; set; }
    }
}