using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace ACTransit.Contracts.Data.CusRel.ReportContract.Params
{
    [DataContract] //, JsonObject(MemberSerialization.OptIn)
    public class ReportParams
    {
        public ReportParams()
        {
            RangeStart = new DateTime(1970, 1, 1);
            RangeEnd = new DateTime(1970, 1, 1);
        }

        [Display(Name = "Range Start Date & Time"), DataMember]
        public DateTime RangeStart { get; set; }

        [Display(Name = "Range End Date & Time"), DataMember]
        public DateTime RangeEnd { get; set; }
    }
}