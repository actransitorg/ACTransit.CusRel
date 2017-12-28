using System;
using System.Collections.Generic;
using System.Linq;
using ACTransit.Contracts.Data.CusRel.Common;
using ACTransit.Contracts.Data.CusRel.TicketContract;
using ACTransit.Entities.CustomerRelations;
using ACTransit.CusRel.Repositories.DAL;
using ACTransit.CusRel.Repositories.Mapping;
using ACTransit.Framework.Extensions;

namespace ACTransit.CusRel.Repositories.Search
{
    public class SearchContacts : tblContacts
    {
        public DateTime? IncidentDateTimeFrom { get; set; }
        public DateTime? IncidentDateTimeTo { get; set; }
        public DateTime? ReceivedDateTimeFrom { get; set; }
        public DateTime? ReceivedDateTimeTo { get; set; }
        public List<string> ExcludeTicketStatusList { get; set; }
        public List<string> IncludeTicketStatusList { get; set; }
        public bool IncludeContactHistory { get; set; }
        public bool IncludeResearchHistory { get; set; }
        public string FullName { get; set; }
        public bool SearchAsUnion { get; set; }

        public bool IsForwardOrder { get; set; }

        public IQueryable<tblContacts> BuildQuery(CusRelDbContext context)
        {
            var priority = !string.IsNullOrWhiteSpace(Priority) ? Priority.PadRight(6, ' ') : null;
            var custZip = !string.IsNullOrWhiteSpace(CustZip) ? CustZip.PadRight(10, ' ') : null;
            var respondVia = !string.IsNullOrWhiteSpace(RespondVia) ? RespondVia.PadRight(12, ' ') : null;
            var vehNo = !string.IsNullOrWhiteSpace(VehNo) ? VehNo.PadRight(4, ' ') : null;
            var route = !string.IsNullOrWhiteSpace(Route) ? Route.PadRight(4, ' ') : null;
            var badge = !string.IsNullOrWhiteSpace(Badge) ? Badge.PadLeft(6, '0') : null;
            var assignedTo = !string.IsNullOrWhiteSpace(AssignedTo) ? AssignedTo.PadRight(16, ' ') : null;
            var ticketStatus = !string.IsNullOrWhiteSpace(TicketStatus) ? TicketStatus.PadRight(16, ' ') : null;
            if (ForAction == "")
                ForAction = null;
            if (LostItemCategory != null && LostItemCategory.Trim().ToLower() == "none")
                LostItemCategory = null;
            if (LostItemType != null && LostItemType.Trim().ToLower() == "none")
                LostItemType = null;
            if (RespondVia.NullableTrim().EnumParse(ResponseCriteriaVia.None) == ResponseCriteriaVia.None)
                RespondVia = null;
            if (string.IsNullOrWhiteSpace(Reasons))
                Reasons = null;
            if (ExcludeTicketStatusList == null) // to avoid null checks
                ExcludeTicketStatusList = new List<string>();
            if (IncludeTicketStatusList == null) // to avoid null checks
                IncludeTicketStatusList = new List<string>();

            // support for DaysOpen
            if (ReceivedDateTime != null && ReceivedDateTime.Value.Date == ReceivedDateTime.Value && ReceivedDateTimeFrom == null && ReceivedDateTimeTo == null)
            {
                ReceivedDateTimeFrom = ReceivedDateTime.Value.Date;
                ReceivedDateTimeTo = ReceivedDateTime.Value.Date.AddDays(1);
                ReceivedDateTime = null;
            }

            IQueryable<tblContacts> result;

            if (!SearchAsUnion)
                result = (from c in context.tblContacts
                    where (FileNum == 0 || c.FileNum == FileNum)
                          && (FeedbackId == null || FeedbackId == 0 || c.FeedbackId == FeedbackId.Value)
                        //&& (ClerkId == null || c.ClerkId == ClerkId)
                          && (UserId == null || c.UserId == UserId)
                          && (ADAComplaint == null || c.ADAComplaint == ADAComplaint)
                          && (SeniorComplaint == null || c.SeniorComplaint == SeniorComplaint)
                          && (ContactSource == null || c.ContactSource == ContactSource)
                          && (ReceivedDateTime == null || c.ReceivedDateTime == ReceivedDateTime.Value)
                          && (Priority == null || c.Priority == priority)
                          && (ResolvedDateTime == null || c.ResolvedDateTime == ResolvedDateTime.Value)
                          && (FirstName == null || c.FirstName == FirstName)
                          && (LastName == null || c.LastName == LastName)
                          && (FullName == null || (c.FirstName.TrimEnd() + " " + c.LastName.TrimEnd()).Contains(FullName))
                          && (Addr1 == null || c.Addr1.Contains(Addr1))
                          && (Addr2 == null || c.Addr2.Contains(Addr2))
                          && (CustCity == null || c.CustCity == CustCity)
                          && (CustState == null || c.CustState == CustState)
                          && (CustZip == null || c.CustZip == custZip)
                          && (HomePhone == null || c.HomePhone.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "").Replace("/", "").Replace("=", "").Contains(HomePhone))
                          && (WorkPhone == null || c.WorkPhone.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "").Replace("/", "").Replace("=", "").Contains(WorkPhone))
                          && (CellPhone == null || c.CellPhone.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "").Replace("/", "").Replace("=", "").Contains(CellPhone))
                          && (Email == null || c.Email.Contains(Email))
                          && (RespondVia == null || c.RespondVia == respondVia)
                          && (IncidentDateTime == null || c.IncidentDateTime == IncidentDateTime.Value)
                          && (VehNo == null || c.VehNo == vehNo)
                          && (Route == null || c.Route == route)
                          && (Destination == null || c.Destination.Contains(c.Destination))
                          && (Location == null || c.Location.Contains(Location))
                          && (IncidentCity == null || c.IncidentCity == IncidentCity)
                          && (Badge == null || c.Badge == badge)
                          && (OperatorName == null || c.OperatorName.Contains(OperatorName))
                          && (EmployeeDesc == null || c.EmployeeDesc.Contains(EmployeeDesc))
                          && (ResponseRequested == null || c.ResponseRequested == ResponseRequested)
                          && (Division == null || c.Division == Division)
                          && (CustComments == null || c.CustComments.Contains(CustComments))
                          && (AssignedTo == null || c.AssignedTo == assignedTo)
                          && (ForAction == null || c.ForAction == ForAction)
                        //&& (ContactStatus == null || c.ContactStatus == ContactStatus)
                          && (TicketStatus == null || c.TicketStatus == ticketStatus)
                          && (Resolution == null || c.Resolution == Resolution)
                          && (ResolutionComment == null || c.ResolutionComment == ResolutionComment)
                          && (updatedBy == null || c.updatedBy == updatedBy)
                          && (updatedOn == null || c.updatedOn == updatedOn.Value)
                          && (Reasons == null || c.Reasons.Contains(Reasons))
                          && (TitleVI == null || c.TitleVI == TitleVI)
                          && (LostItemCategory == null || c.LostItemCategory == LostItemCategory)
                          && (LostItemType == null || c.LostItemType == LostItemType)
                          && (ReceivedDateTimeFrom == null || c.ReceivedDateTime >= ReceivedDateTimeFrom.Value)
                          && (ReceivedDateTimeTo == null || c.ReceivedDateTime <= ReceivedDateTimeTo.Value)
                          && (IncidentDateTimeFrom == null || c.IncidentDateTime >= IncidentDateTimeFrom.Value)
                          && (IncidentDateTimeTo == null || c.IncidentDateTime <= IncidentDateTimeTo.Value)
                          && (ExcludeTicketStatusList.Count == 0 || !ExcludeTicketStatusList.Contains(c.TicketStatus))
                          && (IncludeTicketStatusList.Count == 0 || IncludeTicketStatusList.Contains(c.TicketStatus))
                    select c);
            else
                result = (from c in context.tblContacts
                    where (FileNum == 0 || c.FileNum == FileNum)
                          && (FeedbackId == null || FeedbackId == 0 || c.FeedbackId == FeedbackId.Value)
                        //&& (ClerkId == null || c.ClerkId == ClerkId)
                          && (UserId == null || c.UserId == UserId)
                          && ((ADAComplaint != null && c.ADAComplaint == ADAComplaint)
                              || (SeniorComplaint != null && c.SeniorComplaint == SeniorComplaint)
                              || (ContactSource != null && c.ContactSource == ContactSource)
                              || (ReceivedDateTime != null && c.ReceivedDateTime == ReceivedDateTime.Value)
                              || (Priority != null && c.Priority == priority)
                              || (ResolvedDateTime != null && c.ResolvedDateTime == ResolvedDateTime.Value)
                              || (FirstName != null && c.FirstName == FirstName)
                              || (LastName != null && c.LastName == LastName)
                              || (FullName != null && (c.FirstName.TrimEnd() + " " + c.LastName.TrimEnd()).Contains(FullName))
                              || (Addr1 != null && c.Addr1.Contains(Addr1))
                              || (Addr2 != null && c.Addr2.Contains(Addr2))
                              || (CustCity != null && c.CustCity == CustCity)
                              || (CustState != null && c.CustState == CustState)
                              || (CustZip != null && c.CustZip == custZip)
                              || (HomePhone != null && c.HomePhone.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "").Replace("/", "").Replace("=", "").Contains(HomePhone))
                              || (WorkPhone != null && c.WorkPhone.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "").Replace("/", "").Replace("=", "").Contains(WorkPhone))
                              || (CellPhone != null && c.CellPhone.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "").Replace("/", "").Replace("=", "").Contains(CellPhone))
                              || (Email != null && c.Email.Contains(Email))
                              || (RespondVia != null && c.RespondVia == respondVia)
                              || (IncidentDateTime != null && c.IncidentDateTime == IncidentDateTime.Value)
                              || (VehNo != null && c.VehNo == vehNo)
                              || (Route != null && c.Route == route)
                              || (Destination != null && c.Destination.Contains(c.Destination))
                              || (Location != null && c.Location.Contains(Location))
                              || (IncidentCity != null && c.IncidentCity == IncidentCity)
                              || (Badge != null && c.Badge == badge)
                              || (OperatorName != null && c.OperatorName.Contains(OperatorName))
                              || (EmployeeDesc != null && c.EmployeeDesc.Contains(EmployeeDesc))
                              || (ResponseRequested != null && c.ResponseRequested == ResponseRequested)
                              || (Division != null && c.Division == Division)
                              || (CustComments != null && c.CustComments.Contains(CustComments))
                              || (AssignedTo != null && c.AssignedTo == assignedTo)
                              || (ForAction != null && c.ForAction == ForAction)
                              || (TicketStatus != null && c.TicketStatus == ticketStatus)
                              || (Resolution != null && c.Resolution == Resolution)
                              || (ResolutionComment != null && c.ResolutionComment == ResolutionComment)
                              || (updatedBy != null && c.updatedBy == updatedBy)
                              || (updatedOn != null && c.updatedOn == updatedOn.Value)
                              || (Reasons != null && c.Reasons.Contains(Reasons))
                              || (TitleVI != null && c.TitleVI == TitleVI)
                              || (LostItemCategory != null && c.LostItemCategory == LostItemCategory)
                              || (LostItemType != null && c.LostItemType == LostItemType)
                              )
                          && (ReceivedDateTimeFrom == null || c.ReceivedDateTime >= ReceivedDateTimeFrom.Value)
                          && (ReceivedDateTimeTo == null || c.ReceivedDateTime <= ReceivedDateTimeTo.Value)
                          && (IncidentDateTimeFrom == null || c.IncidentDateTime >= IncidentDateTimeFrom.Value)
                          && (IncidentDateTimeTo == null || c.IncidentDateTime <= IncidentDateTimeTo.Value)
                          && (ExcludeTicketStatusList.Count == 0 || !ExcludeTicketStatusList.Contains(c.TicketStatus))
                          && (IncludeTicketStatusList.Count == 0 || IncludeTicketStatusList.Contains(c.TicketStatus))
                    select c);

            return IsForwardOrder 
                ? result.OrderBy(t => t.FileNum) 
                : result.OrderByDescending(t => t.FileNum);
        }

        public TicketEntities Fill(TicketEntities TicketEntities, CusRelDbContext context)
        {
            if (IncludeContactHistory)
            {
                TicketEntities.ResponseHistory =
                    (from rh in context.tblContactHistory
                        where rh.FileNum == TicketEntities.Contact.FileNum
                        select rh).ToList();
            }
            if (IncludeResearchHistory)
            {
                TicketEntities.ResearchHistory =
                    (from rh in context.tblResearchHistory
                     where rh.FileNum == TicketEntities.Contact.FileNum
                     select rh).ToList();
            }
            return TicketEntities;
        }

        public List<Ticket> GetTickets(CusRelDbContext Context = null, RequestState RequestState = null)
        {
            using (var context = Context ?? new CusRelDbContext())
            {
                var result = BuildQuery(context);
                var results = RequestState != null 
                    ? result.Take(RequestState.MaxSearchCount).ToList() 
                    : result.ToList();
                return results.Select(contact => Fill(new TicketEntities { Contact = contact }, context)).Select(ticketEntity => ticketEntity.FromEntities()).ToList();
            }
        }
    }
}
