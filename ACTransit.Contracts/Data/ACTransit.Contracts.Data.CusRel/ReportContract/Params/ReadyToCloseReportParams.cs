using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace ACTransit.Contracts.Data.CusRel.ReportContract.Params
{
    [DataContract]
    public class ReadyToCloseReportParamItem
    {
        [Display(Name = "Id"), DataMember]
        public int Id { get; set; }

        [Display(Name = "Current Status"), DataMember]
        public string CurrentStatus { get; set; }

        [Display(Name = "New Status"), DataMember]
        public string NewStatus { get; set; }    
    }

    [DataContract] //, JsonObject(MemberSerialization.OptIn)
    public class ReadyToCloseReportParams : ReportParams
    {
        public ReadyToCloseReportParams()
        {
            Items = new List<ReadyToCloseReportParamItem>();
        }

        [Display(Name = "Close Items"), DataMember]
        public List<ReadyToCloseReportParamItem> Items { get; set; }
    }
}