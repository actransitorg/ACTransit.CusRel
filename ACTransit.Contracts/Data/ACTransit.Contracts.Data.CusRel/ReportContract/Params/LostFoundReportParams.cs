using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace ACTransit.Contracts.Data.CusRel.ReportContract.Params
{
    [DataContract] //, JsonObject(MemberSerialization.OptIn)
    public class LostFoundReportParams : ReportParams
    {
        [Display(Name = "Lost Item Category"), DataMember]
        public string LostItemCategory { get; set; }

        [Display(Name = "Lost Item Type"), DataMember]
        public string LostItemType { get; set; }
    }
}