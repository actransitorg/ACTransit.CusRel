using System;
using System.Collections.Generic;
using System.Linq;
using ACTransit.Contracts.Data.CusRel.ReportContract;
using ACTransit.Contracts.Data.CusRel.TicketContract;
using ACTransit.Contracts.Data.CusRel.TicketContract.Params;
using ACTransit.Contracts.Data.CusRel.UserContract;
using ACTransit.CusRel.Repositories.Extensions;
using ACTransit.Entities.CustomerRelations;
using ACTransit.CusRel.Repositories.Search;
using ACTransit.Framework.Extensions;
using Newtonsoft.Json;

namespace ACTransit.CusRel.Repositories.Mapping
{
    public static class TicketConversionExtensions
    {
        public static tblContacts CreateContact(this Ticket Ticket)
        {
            // TODO: Use object initializer once code is proven accurate
            var result = new tblContacts();
            result.FileNum = Ticket.ShouldIgnore ? -1 : Ticket.Id;
            result.FeedbackId = Ticket.Source != null ? Ticket.Source.FeedbackId : null;
            result.ClerkId = null; // deprecated
            result.UserId = Ticket.Source != null && Ticket.Source.ReceivedBy != null ? Ticket.Source.ReceivedBy.Id : null;
            result.ADAComplaint = Ticket.IsAdaComplaint.HasValue ? (Ticket.IsAdaComplaint.GetValueOrDefault() ? "Y" : "N") : null;
            result.SeniorComplaint = null; // deprecated
            result.ContactSource = Ticket.Source != null && Ticket.Source.Via != null ? Ticket.Source.Via.ToString() : null;
            result.ReceivedDateTime = Ticket.Source != null && Ticket.Source.ReceivedAt != null
                ? Ticket.Source.ReceivedAt
                : (Ticket.DaysOpen > 0
                    ? (DateTime?)DateTime.Now.Date.AddDays(-Ticket.DaysOpen)
                    : null);
            result.Priority = Ticket.Priority != null ? Ticket.Priority.ToString() : null;
            result.ResolvedDateTime = Ticket.Resolution != null ? Ticket.Resolution.ResolvedAt : null;
            result.FirstName = Ticket.Contact != null && Ticket.Contact.Name != null ? Ticket.Contact.Name.First : null;
            result.LastName = Ticket.Contact != null && Ticket.Contact.Name != null ? Ticket.Contact.Name.Last : null;
            result.Addr1 = Ticket.Contact != null && Ticket.Contact.Address != null && !string.IsNullOrEmpty(Ticket.Contact.Address.Addr1) ? Ticket.Contact.Address.Addr1 : null;
            result.Addr2 = Ticket.Contact != null && Ticket.Contact.Address != null && !string.IsNullOrEmpty(Ticket.Contact.Address.Addr2) ? Ticket.Contact.Address.Addr2 : null;
            result.CustCity = Ticket.Contact != null && Ticket.Contact.Address != null && !string.IsNullOrEmpty(Ticket.Contact.Address.City) ? Ticket.Contact.Address.City : null;
            result.CustState = Ticket.Contact != null && Ticket.Contact.Address != null ? Ticket.Contact.Address.State : null;
            result.CustZip = Ticket.Contact != null && Ticket.Contact.Address != null ? Ticket.Contact.Address.ZipCode : null;
            result.HomePhone = null; // deprecated
            result.WorkPhone = null; // deprecated
            result.CellPhone = Ticket.Contact != null && Ticket.Contact.Phone != null && !string.IsNullOrEmpty(Ticket.Contact.Phone.Number) ? Ticket.Contact.Phone.Number : null; 
            result.Email = Ticket.Contact != null && !string.IsNullOrEmpty(Ticket.Contact.Email) ? Ticket.Contact.Email : null;
            result.RespondVia = Ticket.ResponseCriteria != null ? Ticket.ResponseCriteria.Via.ToString() : null;
            result.IncidentDateTime = Ticket.Incident != null ? Ticket.Incident.IncidentAt : null;
            result.VehNo = Ticket.Incident != null ? Ticket.Incident.VehicleNumber : null;
            result.Route = Ticket.Incident != null ? Ticket.Incident.Route : null;
            result.Destination = Ticket.Incident != null ? Ticket.Incident.Destination : null;
            result.Location = Ticket.Incident != null ? Ticket.Incident.Location : null;
            result.IncidentCity = Ticket.Incident != null ? Ticket.Incident.City : null;
            result.Badge = Ticket.Operator != null && !string.IsNullOrEmpty(Ticket.Operator.Badge) ? Ticket.Operator.Badge.PadLeft(6, '0') : null;
            result.EmployeeDesc = Ticket.Operator != null ? Ticket.Operator.Description : null;
            result.ResponseRequested = Ticket.ResponseCriteria != null && Ticket.ResponseCriteria.HasRequestedResponse.HasValue ? (Ticket.ResponseCriteria.HasRequestedResponse.GetValueOrDefault() ? "Y" : "N") : null;
            result.Division = Ticket.Incident != null ? Ticket.Incident.Division : null;
            result.CustComments = Ticket.Comments;
            result.AssignedTo = Ticket.Assignment != null && Ticket.Assignment.Employee != null ? Ticket.Assignment.Employee.Id : null;
            result.ForAction = Ticket.Assignment != null && Ticket.Assignment.GroupContact != null ? Ticket.Assignment.GroupContact.ToString() : null;
            result.ForInfos = null; // deprecated
            result.ContactStatus = Ticket.Contact != null ? Ticket.Contact.Status : null;
            result.TicketStatus = Ticket.Status != null ? Ticket.Status.ToString().PascalCaseToDescription() : null;
            result.Resolution = Ticket.Resolution != null ? Ticket.Resolution.Action : null;
            result.ResolutionComment = Ticket.Resolution != null && !string.IsNullOrEmpty(Ticket.Resolution.Comment) ? Ticket.Resolution.Comment : null;
            result.updatedBy = Ticket.UpdatedBy != null ? Ticket.UpdatedBy.Id : null;
            result.updatedOn = Ticket.UpdatedAt;
            result.Reasons = Ticket.Reasons != null ? String.Join(";", Ticket.Reasons.Where(t => !string.IsNullOrEmpty(t)).ToArray()) : null;
            result.TitleVI = Ticket.IsTitle6.HasValue ? (Ticket.IsTitle6.GetValueOrDefault() ? "Y" : "N") : null;
            result.LostItemCategory = Ticket.LostItem != null ? Ticket.LostItem.Category : null;
            result.LostItemType = Ticket.LostItem != null ? Ticket.LostItem.Type : null;
            result.OperatorName = Ticket.Operator != null ? Ticket.Operator.Name : null;

            return result;
        }

        /// <summary>
        /// Due to POCO contacts, and without partial class changes to tblContacts (different namespace), extension methods are used to simulate derived/base class behavior.
        /// This has small performance penality, using serialization, but the DRY principle is achieved.
        /// </summary>
        /// <param name="TicketSearchParams">SearchTicketRequest object </param>
        /// <returns>SearchContacts object with all populated.</returns>
        public static SearchContacts CreateSearchContact(this TicketSearchParams TicketSearchParams)
        {
            var baseTicket = JsonConvert.DeserializeObject<Ticket>(JsonConvert.SerializeObject(TicketSearchParams));
            var contact = baseTicket.CreateContact();
            var result = JsonConvert.DeserializeObject<SearchContacts>(JsonConvert.SerializeObject(contact));
            result.ReceivedDateTimeFrom = TicketSearchParams.ReceivedAtFrom;
            result.ReceivedDateTimeTo = TicketSearchParams.ReceivedAtTo;
            result.IncidentDateTimeFrom = TicketSearchParams.IncidentAtFrom;
            result.IncidentDateTimeTo = TicketSearchParams.IncidentAtTo;
            result.ExcludeTicketStatusList = TicketSearchParams.ExcludeTicketStatusList;
            result.IncludeTicketStatusList = TicketSearchParams.IncludeTicketStatusList;
            result.IncludeContactHistory = TicketSearchParams.IncludeContactHistory;
            result.IncludeResearchHistory = TicketSearchParams.IncludeResearchHistory;
            result.FullName = TicketSearchParams.FullName;
            result.IsForwardOrder = TicketSearchParams.IsForwardOrder;
            result.SearchAsUnion = TicketSearchParams.SearchAsUnion;
            return result;
        }

        public static TicketEntities ToEntities(this Ticket Ticket)
        {
            var result = new TicketEntities
            {
                Contact = Ticket.CreateContact(),
                Attachments = Ticket.Attachments != null ? Ticket.Attachments.Select(item => TicketEntities.ToAttachment(item, Ticket)).ToList() : new List<tblAttachments>(),
                ResponseHistory = Ticket.ResponseHistory != null ? Ticket.ResponseHistory.Select(item => TicketEntities.ToContactHistory(item, Ticket)).ToList() : new List<tblContactHistory>(),
                ResearchHistory = Ticket.ResearchHistory != null ? Ticket.ResearchHistory.Select(item => TicketEntities.ToResearchHistory(item, Ticket)).ToList() : new List<tblResearchHistory>(),
                LinkedContacts = Ticket.LinkedTickets != null ? Ticket.LinkedTickets.Select(childTicket => TicketEntities.ToLinkedContact(Ticket, childTicket)).ToList() : new List<tblLinkedContacts>(),
                //ChangeHistory = Ticket.ChangeHistory != null ? Ticket.ChangeHistory.Select(item => TicketEntities.ToChangeHistory(item, Ticket)).ToList() : new List<tblUpdateLog>()
            };

            // merge ResponseHistory back to ContactStatus, if applicable
            if (Ticket.ResponseHistory != null)
            {
                var lastResponseHistory = Ticket.ResponseHistory.FirstOrDefault();
                if (lastResponseHistory != null && lastResponseHistory.Via.ToString() != result.Contact.ContactStatus)
                    result.Contact.ContactStatus = lastResponseHistory.Via.ToString();
            }

            return result;
        }

        public static Ticket FromEntities(this TicketEntities TicketEntities, int? LinkedId = null)
        {
            if (TicketEntities == null) return null;
            var contact = TicketEntities.Contact;

            // TODO: Use object initializer once code is proven accurate
            var ticket = new Ticket();
            ticket.Id = contact.FileNum;
            ticket.LinkedId = LinkedId;
            ticket.Status = contact.TicketStatus != null ? contact.TicketStatus.NullableTrim().DescriptionToPascalCase().EnumParse(TicketStatus.New) : TicketStatus.New;
            ticket.Priority = contact.Priority.NullableTrim().EnumParse(TicketPriority.Normal);
            ticket.Source = new TicketSource
            {
                ReceivedAt = contact.ReceivedDateTime,
                ReceivedBy = new User(Id: contact.UserId.NullOrWhiteSpaceTrim()),
                Via = contact.ContactSource.NullableTrim().EnumParse(TicketSourceVia.WEB),
                FeedbackId = contact.FeedbackId
            };
            ticket.Contact = new Contact
            {
                Name = new Name
                {
                    First = contact.FirstName.NullOrWhiteSpaceTrim(),
                    Last = contact.LastName.NullOrWhiteSpaceTrim()
                },
                Address = new Address
                {
                    Addr1 = contact.Addr1.NullOrWhiteSpaceTrim(),
                    Addr2 = contact.Addr2.NullOrWhiteSpaceTrim(),
                    City = contact.CustCity.NullOrWhiteSpaceTrim(),
                    State = contact.CustState.NullOrWhiteSpaceTrim(),
                    ZipCode = contact.CustZip.NullOrWhiteSpaceTrim(),
                },
                Email = contact.Email,
                Phone = new Phone
                {
                    Number = contact.CellPhone.NullOrWhiteSpaceTrim(),
                    Kind = PhoneKind.Home
                },
                Status = contact.ContactStatus.NullableTrim()
            };
            ticket.IsAdaComplaint = contact.ADAComplaint == "Y";
            ticket.IsTitle6 = contact.TitleVI == "Y";
            ticket.Incident = new Incident
            {
                IncidentAt = contact.IncidentDateTime,
                VehicleNumber = contact.VehNo.NullOrWhiteSpaceTrim(),
                Route = contact.Route.NullOrWhiteSpaceTrim(),
                Location = contact.Location.NullOrWhiteSpaceTrim(),
                Destination = contact.Destination.NullOrWhiteSpaceTrim(),
                City = contact.IncidentCity.NullOrWhiteSpaceTrim(),
                Division = contact.Division.NullOrWhiteSpaceTrim()
            };
            ticket.Operator = new Operator
            {
                Badge = contact.Badge.NullOrWhiteSpaceTrim(),
                Description = contact.EmployeeDesc.NullOrWhiteSpaceTrim(),
                Name = contact.OperatorName.NullOrWhiteSpaceTrim(),
            };
            ticket.LostItem = new LostItem
            {
                Category = contact.LostItemCategory != null ? contact.LostItemCategory.NullableTrim() : null,
                Type = contact.LostItemType != null ? contact.LostItemType.NullableTrim() : null,
            };
            ticket.Reasons = contact.Reasons != null ? new List<string>(contact.Reasons.Split(new[] {';'})) : null;
            ticket.Comments = contact.CustComments;
            ticket.ResponseCriteria = new ResponseCriteria
            {
                HasRequestedResponse = contact.ResponseRequested == "Y",
                Via = ParseResponseCriteriaVia(contact.RespondVia)
            };
            ticket.Resolution = new Resolution
            {
                Action = contact.Resolution.NullOrWhiteSpaceTrim(),
                Comment = contact.ResolutionComment.NullOrWhiteSpaceTrim(),
                ResolvedAt = contact.ResolvedDateTime
            };
            ticket.Assignment = new Assignment
            {
                GroupContact = new GroupContact(contact.ForAction.NullOrWhiteSpaceTrim()),
                Employee = new User(Id: contact.AssignedTo.NullOrWhiteSpaceTrim())
            };
            ticket.ResponseHistory = TicketEntities.ResponseHistory != null ? TicketEntities.ResponseHistory.Select(ch => TicketEntities.FromContactHistory(ch, contact.ContactStatus)).OrderByDescending(rh => rh.ResponseAt).ToList() : null;
            ticket.Attachments = TicketEntities.Attachments != null ? TicketEntities.Attachments.Select(TicketEntities.FromAttachment).ToList() : null;
            ticket.ResearchHistory = TicketEntities.ResearchHistory != null ? TicketEntities.ResearchHistory.Select(TicketEntities.FromResearchHistory).OrderByDescending(rh => rh.ResearchedAt).ToList() : null;
            ticket.LinkedTickets = TicketEntities.LinkedContacts != null ? TicketEntities.FromLinkedContacts(TicketEntities.LinkedContacts, contact.FileNum) : null;
            ticket.UpdatedAt = contact.updatedOn;
            ticket.ChangeHistory = TicketEntities.ChangeHistory != null ? TicketEntities.ChangeHistory.Select(TicketEntities.FromChangeHistory).OrderByDescending(ch => ch.ChangeAt).ThenBy(ch => ch.ColumnName).ToList() : null;
            ticket.UpdatedBy = new User(Id: contact.updatedBy.NullOrWhiteSpaceTrim());
            ticket.DaysOpen = (
                (contact.ResolvedDateTime.HasValue
                ? contact.ResolvedDateTime.GetValueOrDefault().Date
                : DateTime.Now.Date) - contact.ReceivedDateTime.GetValueOrDefault().Date).Days;

            return ticket;
        }

        public static ResponseCriteriaVia ParseResponseCriteriaVia(string value)
        {
            if (value == null) return ResponseCriteriaVia.None;
            var result = value.NullableTrim().EnumParse(ResponseCriteriaVia.None);
            if (result == ResponseCriteriaVia.None && value.ToLower().Contains("phone"))
                result = ResponseCriteriaVia.Phone;
            return result;
        }

        public static List<ComplaintCode> FromEntities(this List<tblCustomerComplaintCodes> Codes)
        {
            return Codes.Select(code => code.CreateComplaintCode()).ToList();
        }

        public static ComplaintCode CreateComplaintCode(this tblCustomerComplaintCodes Code)
        {
            return new ComplaintCode
            {
                Code = Code.ComplaintCode.TrimEnd(),
                Group = Code.ComplaintGroup.TrimEnd(),
                Category = Code.ComplaintCategory.TrimEnd(),
                Description = Code.Description.TrimEnd(),
                PastDueDays = Code.PastDueDays
            };
        }

        public static List<GroupContact> FromEntities(this List<tblCustomerReferenceCodes> Items)
        {
            return Items.Select(code => code.CreateComplaintCode()).ToList();
        }

        public static GroupContact CreateComplaintCode(this tblCustomerReferenceCodes Item)
        {
            return new GroupContact
            {
                Code = Item.REFER_CODE.TrimEnd(),
                Description = Item.REFER_DESC.TrimEnd(),
                Email = Item.Email,
                Order = Item.Order,
                IsVisible = Item.IsVisible
            };
        }

        public static tblCustomerReferenceCodes ToEntity(this GroupContact GroupContact)
        {
            return new tblCustomerReferenceCodes
            {
                REFER_CODE = GroupContact.Code,
                REFER_DESC = GroupContact.Description,
                Email = GroupContact.Email,
                IsVisible = GroupContact.IsVisible, 
                Order = GroupContact.Order
            };
        }

        public static List<AssignedToReportTableItem> ToAssignedToReport(this List<Ticket> Items)
        {
            return Items.Select(code => code.CreateAssignedToReportTableItem()).ToList();
        }

        public static AssignedToReportTableItem CreateAssignedToReportTableItem(this Ticket Item)
        {
            var lastResponse = LastResponse(Item);
            return new AssignedToReportTableItem
            {
                Id = Item.Id,
                Status = Item.Status.ToString(),
                IncidentAt = Item.Incident.IncidentAt,
                AssignedTo = Item.Assignment.Employee.Id,
                GroupContact = Item.Assignment.GroupContact.Description,
                Reasons = Item.Reasons != null ? string.Join(";", Item.Reasons.ToArray()) : null,
                IsAdaComplaint = Item.IsAdaComplaint.GetValueOrDefault(),
                IsTitle6 = Item.IsTitle6.GetValueOrDefault(),
                Comments = Item.Comments,
                DaysOpen = Item.DaysOpen,
                Priority = Item.Priority.ToString(),
                ContactVia = Item.ResponseCriteria.Via.ToString().PascalCaseToDescription(),
                //ContactVia = lastResponse != null ? lastResponse.Via.ToString().PascalCaseToDescription() : null,
                OperatorName = Item.Operator != null ? (Item.Operator.Name ?? Item.Operator.Badge) : null
            };
        }

        public static List<ForActionReportTableItem> ToForActionReport(this List<Ticket> Items)
        {
            return Items.Select(code => code.CreateForActionReportTableItem()).ToList();
        }

        public static ForActionReportTableItem CreateForActionReportTableItem(this Ticket Item)
        {
            return new ForActionReportTableItem
            {
                Id = Item.Id,
                Status = Item.Status.ToString(),
                IncidentAt = Item.Incident.IncidentAt,
                AssignedTo = Item.Assignment.Employee.Id,
                GroupContact = Item.Assignment.GroupContact.Description,
                Reasons = Item.Reasons != null ? string.Join(";", Item.Reasons.ToArray()) : null,
                IsAdaComplaint = Item.IsAdaComplaint.GetValueOrDefault(),
                IsTitle6 = Item.IsTitle6.GetValueOrDefault(),
                Comments = Item.Comments,
                DaysOpen = Item.DaysOpen,
                Priority = Item.Priority.ToString(),
                Route = Item.Incident.Route,
                OperatorName = Item.Operator != null ? (Item.Operator.Name ?? Item.Operator.Badge) : null,
                ContactVia = Item.ResponseCriteria.Via.ToString().PascalCaseToDescription()
            };
        }

        public static List<ReadyToCloseReportTableItem> ToReadyToCloseReport(this List<Ticket> Items)
        {
            return Items.Select(code => code.CreateReadyToCloseReportTableItem()).ToList();
        }

        public static ResponseHistory LastResponse(this Ticket Item)
        {
            return Item.ResponseHistory != null && Item.ResponseHistory.Count > 0
                ? Item.ResponseHistory[0] // list is in time decending order
                : null;            
        }

        public static ReadyToCloseReportTableItem CreateReadyToCloseReportTableItem(this Ticket Item)
        {
            var lastResponse = LastResponse(Item);
            return new ReadyToCloseReportTableItem
            {
                Id = Item.Id,
                IncidentAt = Item.Incident.IncidentAt,
                GroupContact = Item.Assignment.GroupContact.Value,
                AssignedTo = Item.Assignment.Employee != null ? Item.Assignment.Employee.Id : null,
                Reasons = Item.Reasons != null ? string.Join(";", Item.Reasons.ToArray()) : null,
                ResponseBy = lastResponse != null ? lastResponse.ResponseBy.Id : null,
                ResponseVia = lastResponse == null 
                    ? ResponseHistoryVia.New.ToString() 
                    : (lastResponse.Via != ResponseHistoryVia.Unknown 
                        ? lastResponse.Via.ToString().PascalCaseToDescription() 
                        : lastResponse.ViaAsString.PascalCaseToDescription()),
                TicketStatus = Item.Status.ToString(),
                Comments = Item.Comments
            };
        }

        public static List<LostFoundReportTableItem> ToLostFoundReport(this List<Ticket> Items)
        {
            return Items.Select(code => code.CreateLostFoundReportTableItem()).OrderByDescending(i => i.IncidentAt).ToList();
        }

        public static LostFoundReportTableItem CreateLostFoundReportTableItem(this Ticket Item)
        {
            return new LostFoundReportTableItem
            {
                Id = Item.Id,
                CustomerFirstName = Item.Contact.Name.First,
                CustomerLastName = Item.Contact.Name.Last,
                CustomerPhoneNumber = Item.Contact.Phone.Number,
                CustomerEmail = Item.Contact.Email,
                IncidentAt = Item.Incident.IncidentAt,
                Route = Item.Incident.Route,
                Comments = Item.Comments,
                LostItemCategory = Item.LostItem.Category,
                LostItemType = Item.LostItem.Type
            };
        }

        public static List<RejectedReportTableItem> ToRejectedReport(this List<Ticket> Items)
        {
            return Items.Select(code => code.CreateRejectedReportTableItem()).OrderByDescending(i => i.IncidentAt).ToList();
        }

        public static RejectedReportTableItem CreateRejectedReportTableItem(this Ticket Item)
        {
            var lastResearchHistory = Item.ResearchHistory != null ? Item.ResearchHistory.FirstOrDefault() : null;

            return new RejectedReportTableItem
            {
                Id = Item.Id,
                IncidentAt = Item.Incident.IncidentAt,
                GroupContact = Item.Assignment.GroupContact.Value,
                Reasons = Item.Reasons != null ? string.Join(";", Item.Reasons.ToArray()) : null,
                Comments = Item.Comments,
                LastResearchComments = lastResearchHistory != null ? lastResearchHistory.Comment : null,
            };
        }

        public static TicketChanges ToTicketChanges(this TicketEntities.ChangeTracking Changes)
        {
            return new TicketChanges
            {
                AssignmentGroupContact = Changes.ForAction,
                AssignmentEmployee = Changes.AssignedTo,
                TicketPriority = Changes.Priority
            };
        }
    }
}    