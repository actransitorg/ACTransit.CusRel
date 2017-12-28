using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace ACTransit.Contracts.Data.CusRel.TicketContract
{
    [DataContract] //, JsonObject(MemberSerialization.OptIn)
    public class Incident
    {
        [Display(Name = "Incident Date/Time"), DataMember]
        public DateTime? IncidentAt { get; set; }

        [Display(Name = "Vehicle Number"), DataMember]
        public string VehicleNumber { get; set; }

        [Display(Name = "Route/Line"), DataMember]
        public string Route { get; set; }

        [Display(Name = "Location"), MaxLength(100), DataMember]
        public string Location { get; set; }

        [Display(Name = "Direction of Travel"), MaxLength(40), DataMember]
        public string Destination { get; set; }

        [Display(Name = "City"), MaxLength(30), DataMember]
        public string City { get; set; }

        [Display(Name = "Division"), StringLength(2), DataMember]
        public string Division { get; set; }
    }
}