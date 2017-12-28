using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using ACTransit.Contracts.Data.CusRel.ReportContract.Result;
using Newtonsoft.Json;

namespace ACTransit.Contracts.Data.CusRel.ReportContract
{
    [DataContract] //, JsonObject(MemberSerialization.OptIn)
    public class LostFoundReport : ReportResult
    {
        [DataMember]
        public List<LostFoundReportTableItem> Items { get; set; }
    }

    [DataContract] //, JsonObject(MemberSerialization.OptIn)
    public class LostFoundReportTableItem
    {
        [Display(Name = "Ticket #"), DataMember]
        public int Id { get; set; }

        [Display(Name = "First Name"), DataMember]
        public string CustomerFirstName { get; set; }

        [Display(Name = "Last Name"), DataMember]
        public string CustomerLastName { get; set; }

        [Display(Name = "Phone"), DataMember]
        public string CustomerPhoneNumber { get; set; }

        [Display(Name = "Email"), DataMember]
        public string CustomerEmail { get; set; }

        [Display(Name = "Incident Date"), DataMember]
        public DateTime? IncidentAt { get; set; }

        [Display(Name = "Route"), DataMember]
        public string Route { get; set; }

        [Display(Name = "Cust Comments"), DataMember]
        public string Comments { get; set; }

        [Display(Name = "Category"), DataMember]
        public string LostItemCategory { get; set; }

        [Display(Name = "Type"), DataMember]
        public string LostItemType { get; set; }
    }
}