using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace ACTransit.Contracts.Data.CusRel.ReportContract.Params
{
    [DataContract] //, JsonObject(MemberSerialization.OptIn)
    public class OpenTicketStatusReportParams : ReportParams
    {
        [Display(Name = "ADA?"), DataMember]
        public bool? IsAda { get; set; }
    }
}