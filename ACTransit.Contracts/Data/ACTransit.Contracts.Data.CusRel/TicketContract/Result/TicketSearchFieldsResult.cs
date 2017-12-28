using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace ACTransit.Contracts.Data.CusRel.TicketContract.Result
{
    [DataContract] //, JsonObject(MemberSerialization.OptIn)
    public class TicketSearchFieldsResult : TicketResult
    {
        [Display(Name = "Search Fields"), DataMember]
        public List<TicketSearchField> SearchFields { get; set; }
    }

    [DataContract] //, JsonObject(MemberSerialization.OptIn)
    public class TicketSearchField
    {
        [DataMember]
        public string Name { get; set; }

        [Display(Name = "Default Value"), DataMember]
        public string DefaultValue { get; set; }

        [Display(Name = "Object Reference"), DataMember]
        public string ObjectGraphRef { get; set; }

        [Display(Name = "Select Items Reference"), DataMember]
        public string SelectItemsRef { get; set; }

        [Display(Name = "Use Value As Key?"), DataMember]
        public bool ValueAsKey { get; set; }

        //[Display(Name = "Resource URI"), DataMember]
        //public string ResourceUri { get; set; }

        //[Display(Name = "Value Field"), DataMember]
        //public string DataValueField { get; set; }

        //[Display(Name = "Display Text Field"), DataMember]
        //public string DataTextField { get; set; }
    }
}