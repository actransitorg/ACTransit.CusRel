using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace ACTransit.Contracts.Data.CusRel.TicketContract.Params
{
    [DataContract] //, JsonObject(MemberSerialization.OptIn)
    public class TicketSearchParams : Ticket
    {
        [Display(Name = "Incident At From"), DataMember]
        public DateTime? IncidentAtFrom { get; set; }

        [Display(Name = "Incident At To"), DataMember]
        public DateTime? IncidentAtTo { get; set; }

        [Display(Name = "Received At From"), DataMember]
        public DateTime? ReceivedAtFrom { get; set; }

        [Display(Name = "Received At To"), DataMember]
        public DateTime? ReceivedAtTo { get; set; }

        [Display(Name = "Exclude Ticket Statuses"), DataMember]
        public List<string> ExcludeTicketStatusList { get; set; }

        [Display(Name = "Include Ticket Statuses"), DataMember]
        public List<string> IncludeTicketStatusList { get; set; }

        [Display(Name = "Include Contact History"), DataMember]
        public bool IncludeContactHistory { get; set; }

        [Display(Name = "Include Research History"), DataMember]
        public bool IncludeResearchHistory { get; set; }

        [Display(Name = "Full Name"), DataMember]
        public string FullName { get; set; }

        [Display(Name = "Is Forward Order?"), DataMember]
        public bool IsForwardOrder { get; set; }

        [Display(Name = "Search using 'OR'?"), DataMember]
        public bool SearchAsUnion { get; set; }

        public static TicketSearchParams EmptyTicketSearchParams()
        {
            var result = new TicketSearchParams();
            result.Empty();
            return result;
        }
    }
}