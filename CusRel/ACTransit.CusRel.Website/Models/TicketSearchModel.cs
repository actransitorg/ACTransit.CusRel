using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ACTransit.Contracts.Data.CusRel.Common;
using ACTransit.Contracts.Data.CusRel.TicketContract;
using ACTransit.Contracts.Data.CusRel.TicketContract.Result;
using ACTransit.CusRel.Services;
using ACTransit.Framework.Extensions;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace ACTransit.CusRel.Models
{
    [DataContract, JsonObject(MemberSerialization.OptIn)]
    public class TicketSearchModel : TicketSearchFieldsResult
    {
        [DataMember]
        public TicketModel TicketModel { get; set; }

        [DataMember]
        public Dictionary<string, int> TicketStatuses { get; set; }

        [DataMember]
        public Dictionary<string, int> TicketPriorities { get; set; }

        [DataMember]
        public Dictionary<string, int> TicketSourceVias { get; set; }

        [DataMember]
        public Dictionary<string, int> ResponseCriteriaVias { get; set; }

        //[DataMember]
        //public Dictionary<string, int> ContactStatuses { get; set; }

        public TicketSearchModel(ServicesProxy servicesProxy)
        {
            ServicesProxy = servicesProxy;
            var ticketResult = ServicesProxy.TicketService.GetTicketSearchFields();
            Ticket = ticketResult.Ticket;
            SearchFields = ticketResult.SearchFields;
            TicketModel = new TicketModel(ServicesProxy, Ticket, true)
            {
                Ticket = null // use TicketSearchModel.Ticket instead of this.Ticket, nulled to reduce serialization size
            };
            //Ticket.UpdatedAt = null;
            //Ticket.UpdatedBy = null;

            MergeResults(ticketResult);

            TicketStatuses = ConvertExtensions.EnumDictionary<TicketStatus>();
            TicketPriorities = ConvertExtensions.EnumDictionary<TicketPriority>();
            TicketSourceVias = ConvertExtensions.EnumDictionary<TicketSourceVia>();
            ResponseCriteriaVias = ConvertExtensions.EnumDictionary<ResponseCriteriaVia>();
            //ContactStatuses = ConvertExtensions.EnumDictionary<ContactStatus>();


            //SearchFields.First(f => f.ObjectGraphRef == "Ticket.IncidentAtFrom").SelectItemsRef = "DateTimePicker";
            //SearchFields.First(f => f.ObjectGraphRef == "Ticket.IncidentAtTo").SelectItemsRef = "DateTimePicker";
            //SearchFields.First(f => f.ObjectGraphRef == "Ticket.ReceivedAtFrom").SelectItemsRef = "DateTimePicker";
            //SearchFields.First(f => f.ObjectGraphRef == "Ticket.ReceivedAtTo").SelectItemsRef = "DateTimePicker";
            //SearchFields.First(f => f.ObjectGraphRef == "Ticket.Id").SelectItemsRef = "TextBox";
            SearchFields.First(f => f.ObjectGraphRef == "Ticket.Status").SelectItemsRef = "TicketStatuses";
            SearchFields.First(f => f.ObjectGraphRef == "Ticket.Priority").SelectItemsRef = "TicketPriorities";
            //SearchFields.First(f => f.ObjectGraphRef == "Ticket.Source.ReceivedAt").SelectItemsRef = "DateTimePicker";
            //SearchFields.First(f => f.ObjectGraphRef == "Ticket.Source.ReceivedBy.Id").ResourceUri = "User/GetUsers";
            SearchFields.First(f => f.ObjectGraphRef == "Ticket.Source.ReceivedBy.Id").SelectItemsRef = "TicketModel.Employees";
            SearchFields.First(f => f.ObjectGraphRef == "Ticket.Source.ReceivedBy.Id").Name = "Received By Username";
            //SearchFields.First(f => f.ObjectGraphRef == "Ticket.Source.ReceivedBy.Username").ResourceUri = "User/GetUsers";
            SearchFields.First(f => f.ObjectGraphRef == "Ticket.Source.ReceivedBy.Username").SelectItemsRef = "TicketModel.Employees";
            SearchFields.First(f => f.ObjectGraphRef == "Ticket.Source.ReceivedBy.Username").Name = "Received By User";
            //SearchFields.First(f => f.ObjectGraphRef == "Ticket.Source.ReceivedBy.Name").SelectItemsRef = "TextBox";
            //SearchFields.First(f => f.ObjectGraphRef == "Ticket.Source.ReceivedBy.Email").SelectItemsRef = "TicketModel.Users";
            //SearchFields.First(f => f.ObjectGraphRef == "Ticket.Source.ReceivedBy.Division").SelectItemsRef = "TicketModel.RouteInfo.Divisions";
            SearchFields.First(f => f.ObjectGraphRef == "Ticket.Source.Via").SelectItemsRef = "TicketSourceVias";
            SearchFields.First(f => f.ObjectGraphRef == "Ticket.Source.Via").Name = "Received Via";
            //SearchFields.First(f => f.ObjectGraphRef == "Ticket.Source.FeedbackId").SelectItemsRef = "TextBox";
            SearchFields.First(f => f.ObjectGraphRef == "Ticket.Contact.Name.First").SelectItemsRef = "TextBox";
            SearchFields.First(f => f.ObjectGraphRef == "Ticket.Contact.Name.First").Name = "Customer First Name";
            SearchFields.First(f => f.ObjectGraphRef == "Ticket.Contact.Name.Last").SelectItemsRef = "TextBox";
            SearchFields.First(f => f.ObjectGraphRef == "Ticket.Contact.Name.Last").Name = "Customer Last Name";
            //SearchFields.First(f => f.ObjectGraphRef == "Ticket.FullName").SelectItemsRef = "TextBox";
            SearchFields.First(f => f.ObjectGraphRef == "Ticket.Contact.Address.Addr1").SelectItemsRef = "TextBox";
            SearchFields.First(f => f.ObjectGraphRef == "Ticket.Contact.Address.Addr1").Name = "Customer Address 1";
            SearchFields.First(f => f.ObjectGraphRef == "Ticket.Contact.Address.Addr2").SelectItemsRef = "TextBox";
            SearchFields.First(f => f.ObjectGraphRef == "Ticket.Contact.Address.Addr2").Name = "Customer Address 2";
            SearchFields.First(f => f.ObjectGraphRef == "Ticket.Contact.Address.City").SelectItemsRef = "TicketModel.Cities";
            SearchFields.First(f => f.ObjectGraphRef == "Ticket.Contact.Address.City").ValueAsKey = true;
            SearchFields.First(f => f.ObjectGraphRef == "Ticket.Contact.Address.City").Name = "Customer Address City";
            SearchFields.First(f => f.ObjectGraphRef == "Ticket.Contact.Address.State").SelectItemsRef = "TicketModel.States";
            SearchFields.First(f => f.ObjectGraphRef == "Ticket.Contact.Address.State").Name = "Customer Address State";
            SearchFields.First(f => f.ObjectGraphRef == "Ticket.Contact.Address.State").ValueAsKey = true;
            SearchFields.First(f => f.ObjectGraphRef == "Ticket.Contact.Address.ZipCode").SelectItemsRef = "TextBox";
            SearchFields.First(f => f.ObjectGraphRef == "Ticket.Contact.Address.ZipCode").Name = "Customer Address Zip";
            //SearchFields.First(f => f.ObjectGraphRef == "Ticket.Contact.Email").SelectItemsRef = "TextBox";
            //SearchFields.First(f => f.ObjectGraphRef == "Ticket.Contact.Phone.Number").SelectItemsRef = "TextBox";
            //SearchFields.First(f => f.ObjectGraphRef == "Ticket.Contact.Status").SelectItemsRef = "ContactStatuses";
            SearchFields.First(f => f.ObjectGraphRef == "Ticket.IsAdaComplaint").SelectItemsRef = "CheckBox";
            SearchFields.First(f => f.ObjectGraphRef == "Ticket.IsTitle6").SelectItemsRef = "CheckBox";
            //SearchFields.First(f => f.ObjectGraphRef == "Ticket.Incident.IncidentAt").SelectItemsRef = "DateTimePicker";
            SearchFields.First(f => f.ObjectGraphRef == "Ticket.Incident.VehicleNumber").SelectItemsRef = "TextBox";
            SearchFields.First(f => f.ObjectGraphRef == "Ticket.Incident.Route").SelectItemsRef = "TicketModel.RouteInfo.Routes";
            SearchFields.First(f => f.ObjectGraphRef == "Ticket.Incident.Route").ValueAsKey = true;
            SearchFields.First(f => f.ObjectGraphRef == "Ticket.Incident.Location").SelectItemsRef = "TextBox";
            SearchFields.First(f => f.ObjectGraphRef == "Ticket.Incident.Destination").SelectItemsRef = "TicketModel.RouteInfo.Directions";
            SearchFields.First(f => f.ObjectGraphRef == "Ticket.Incident.Destination").ValueAsKey = true;
            SearchFields.First(f => f.ObjectGraphRef == "Ticket.Incident.City").SelectItemsRef = "TicketModel.Cities";
            SearchFields.First(f => f.ObjectGraphRef == "Ticket.Incident.City").ValueAsKey = true;
            SearchFields.First(f => f.ObjectGraphRef == "Ticket.Incident.Division").SelectItemsRef = "TicketModel.RouteInfo.Divisions";
            SearchFields.First(f => f.ObjectGraphRef == "Ticket.Incident.Division").ValueAsKey = true;
            SearchFields.First(f => f.ObjectGraphRef == "Ticket.Operator.Badge").SelectItemsRef = "TextBox";
            SearchFields.First(f => f.ObjectGraphRef == "Ticket.Operator.Name").SelectItemsRef = "TextBox";
            SearchFields.First(f => f.ObjectGraphRef == "Ticket.Operator.Description").SelectItemsRef = "TextBox";
            SearchFields.First(f => f.ObjectGraphRef == "Ticket.Reasons").SelectItemsRef = "TicketModel.ComplaintCodes";
            SearchFields.First(f => f.ObjectGraphRef == "Ticket.Comments").SelectItemsRef = "TextBox";
            SearchFields.First(f => f.ObjectGraphRef == "Ticket.ResponseCriteria.HasRequestedResponse").SelectItemsRef = "CheckBox";
            SearchFields.First(f => f.ObjectGraphRef == "Ticket.ResponseCriteria.HasRequestedResponse").Name = "Has Requested Response";
            SearchFields.First(f => f.ObjectGraphRef == "Ticket.ResponseCriteria.Via").SelectItemsRef = "ResponseCriteriaVias";
            SearchFields.First(f => f.ObjectGraphRef == "Ticket.ResponseCriteria.Via").Name = "Response Requested Via";
            //SearchFields.First(f => f.ObjectGraphRef == "Ticket.Resolution.Action").SelectItemsRef = "TextBox";
            //SearchFields.First(f => f.ObjectGraphRef == "Ticket.Resolution.Comment").SelectItemsRef = "TextBox";
            SearchFields.First(f => f.ObjectGraphRef == "Ticket.Resolution.ResolvedAt").SelectItemsRef = "DateTimePicker";
            SearchFields.First(f => f.ObjectGraphRef == "Ticket.Assignment.GroupContact.Value").SelectItemsRef = "TicketModel.GroupContactValues";
            SearchFields.First(f => f.ObjectGraphRef == "Ticket.Assignment.GroupContact.Value").Name = "Assigned Department";
            //SearchFields.First(f => f.ObjectGraphRef == "Ticket.Assignment.Employee.Id").ResourceUri = "User/GetUsers";
            SearchFields.First(f => f.ObjectGraphRef == "Ticket.Assignment.Employee.Id").SelectItemsRef = "TicketModel.Employees";
            SearchFields.First(f => f.ObjectGraphRef == "Ticket.Assignment.Employee.Id").Name = "Assigned Employee Username";
            //SearchFields.First(f => f.ObjectGraphRef == "Ticket.Assignment.Employee.Username").ResourceUri = "User/GetUsers";
            SearchFields.First(f => f.ObjectGraphRef == "Ticket.Assignment.Employee.Username").SelectItemsRef = "TicketModel.Employees";
            SearchFields.First(f => f.ObjectGraphRef == "Ticket.Assignment.Employee.Username").Name = "Assigned Employe Username";
            SearchFields.First(f => f.ObjectGraphRef == "Ticket.Assignment.Employee.Id").Name = "Assigned Employee Name";
            //SearchFields.First(f => f.ObjectGraphRef == "Ticket.Assignment.Employee.Name").SelectItemsRef = "TextBox";
            //SearchFields.First(f => f.ObjectGraphRef == "Ticket.Assignment.Employee.Email").SelectItemsRef = "TicketModel.Users";
            //SearchFields.First(f => f.ObjectGraphRef == "Ticket.Assignment.Employee.Division").SelectItemsRef = "TicketModel.RouteInfo.Divisions";
            SearchFields.First(f => f.ObjectGraphRef == "Ticket.UpdatedAt").SelectItemsRef = "DateTimePicker";
            //SearchFields.First(f => f.ObjectGraphRef == "Ticket.UpdatedBy.Id").ResourceUri = "User/GetUsers";
            SearchFields.First(f => f.ObjectGraphRef == "Ticket.UpdatedBy.Id").SelectItemsRef = "TicketModel.Employees";
            SearchFields.First(f => f.ObjectGraphRef == "Ticket.UpdatedBy.Id").Name = "Last Updated By Username";
            //SearchFields.First(f => f.ObjectGraphRef == "Ticket.UpdatedBy.Username").ResourceUri = "User/GetUsers";
            SearchFields.First(f => f.ObjectGraphRef == "Ticket.UpdatedBy.Username").SelectItemsRef = "TicketModel.Employees";
            SearchFields.First(f => f.ObjectGraphRef == "Ticket.UpdatedBy.Username").Name = "Last Updated By Name";
            //SearchFields.First(f => f.ObjectGraphRef == "Ticket.UpdatedBy.Name").SelectItemsRef = "TextBox";
            //SearchFields.First(f => f.ObjectGraphRef == "Ticket.UpdatedBy.Email").SelectItemsRef = "TicketModel.Users";
            //SearchFields.First(f => f.ObjectGraphRef == "Ticket.UpdatedBy.Division").SelectItemsRef = "TicketModel.RouteInfo.Divisions";
            SearchFields.First(f => f.ObjectGraphRef == "Ticket.DaysOpen").SelectItemsRef = "TextBox";
            SearchFields.First(f => f.ObjectGraphRef == "Ticket.SearchAsUnion").SelectItemsRef = "CheckBox";
            SearchFields.First(f => f.ObjectGraphRef == "Ticket.SearchAsUnion").Name = "Search with 'OR'";

            SearchFields.RemoveAll(p => p.SelectItemsRef == null);
            SearchFields = SearchFields.OrderBy(f => f.Name).ToList();
        }

        public string CanSearchVisibility
        {
            get { return ServicesProxy.RequestState.UserDetails.CanSearchTickets ? "" : " hidden"; ; }
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        public ServicesProxy ServicesProxy { get; private set; }

    }

    public static class ConvertExtensions
    {
        public static Dictionary<string, int> EnumDictionary<T>()
        {
            //var type = typeof(T);
            //var fis = type.GetFields();
            //foreach (var fi in fis)
            //{
            //    var attributes=fi.GetCustomAttributes(typeof (DisplayAttribute), false);
            //    if (attributes.Length > 0 && !string.IsNullOrWhiteSpace(((DisplayAttribute) attributes[0]).Name))
            //    {
                    
            //    }
            //}
            
            return Enum.GetValues(typeof(T)).Cast<T>().ToDictionary(
                t =>
                {
                    var fi = t.GetType().GetField(t.ToString());
                    var attributes = (DisplayAttribute[])fi.GetCustomAttributes(typeof(DisplayAttribute), false);
                    string res = (attributes.Length > 0 && !string.IsNullOrWhiteSpace(((DisplayAttribute) attributes[0]).Name))
                        ? attributes[0].Name: t.ToString();                    
                    return res.PascalCaseToDescription();
                    
                }, 
                t => (int)(object)t
            );

            //return Enum.GetValues(typeof(T)).Cast<T>().ToDictionary(t => t.ToString().PascalCaseToDescription(), t => (int)(object)t);
        }
    }
}