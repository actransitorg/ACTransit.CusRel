using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Web.Mvc;
using ACTransit.Contracts.Data.CusRel.Common;
using ACTransit.Contracts.Data.CusRel.LookupContract;
using ACTransit.Contracts.Data.CusRel.TicketContract;
using ACTransit.Contracts.Data.CusRel.TicketContract.Result;
using ACTransit.Contracts.Data.CusRel.UserContract;
using ACTransit.CusRel.Services;
using ACTransit.Framework.Extensions;
using Ganss.XSS;
using Newtonsoft.Json;

namespace ACTransit.CusRel.Models
{
    [DataContract, JsonObject(MemberSerialization.OptIn)]
    public class TicketModel : TicketResult
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private bool isSearching = false;

        #region SendToClient

        [DataMember]
        public IEnumerable<string> Cities { get; set; }

        [DataMember]
        public IEnumerable<string> States { get; set; }

        [DataMember]
        public RouteInfo RouteInfo { get; set; }

        [DataMember]
        public Operator Operator { get; set; }

        [DataMember]
        public IEnumerable<User> Employees { get; set; }

        [DataMember]
        public IEnumerable<ComplaintCode> ComplaintCodes { get; set; }

        [DataMember]
        public IEnumerable<GroupContact> GroupContactValues { get; set; }

        [DataMember]
        public IEnumerable<ResponseHistoryVia> ViaResponseHistory { get; set; }

        [DataMember]
        public SortedDictionary<string, string[]> LostItemNodes { get; set; }

        #endregion

        #region ServerSideOnly

        public ServicesProxy ServicesProxy { get; private set; }

        public IEnumerable<KeyValuePair<string, string>> Users
        {
            get
            {
                return Employees.Select(item => new KeyValuePair<string, string>(item.Id, item.Username)).ToList();
            }
        }

        public IEnumerable<KeyValuePair<string, string>> Reasons
        {
            get
            {
                return ComplaintCodes.Select(item => new KeyValuePair<string, string>(item.Value, item.Description)).ToList();
            }
        }

        public IEnumerable<SelectListItem> ReasonCode1
        {
            get { return new SelectList(ComplaintCodes, Ticket.ReasonCode1); }
        }

        public IEnumerable<SelectListItem> ReasonCode2
        {
            get { return new SelectList(ComplaintCodes, Ticket.ReasonCode2); }
        }

        public IEnumerable<SelectListItem> ReasonCode3
        {
            get { return new SelectList(ComplaintCodes, Ticket.ReasonCode3); }
        }
        public IEnumerable<SelectListItem> GroupContacts
        {
            get { return new SelectList(GroupContactValues, "Value", "Description"); }
        }

        public IEnumerable<SelectListItem> Directions
        {
            get { return new SelectList(RouteInfo.Directions); }
        }

        public IEnumerable<SelectListItem> Divisions
        {
            get { return new SelectList(RouteInfo.Divisions); }
        }

        public IEnumerable<SelectListItem> Lines
        {
            get { return new SelectList(RouteInfo.Routes); }
        }

        public IEnumerable<SelectListItem> Via
        {
            get
            {
                return (
                    from object item in Enum.GetValues(typeof (ResponseHistoryVia))
                    where item.ToString() != ResponseHistoryVia.Unknown.ToString()
                    select new SelectListItem
                    {
                        Text = item.ToString().PascalCaseToDescription(),
                        Value = ((int) item).ToString()
                    }).ToList();
            }
        }

        public IEnumerable<KeyValuePair<string, string>> TicketStatuses
        {
            get
            {
                var result = (
                    from object item in Enum.GetValues(typeof(TicketStatus))
                    where CanCloseTicket || (!CanCloseTicket && !item.ToString().ToLower().StartsWith("closed"))
                    select new KeyValuePair<string, string>(
                        item.ToString(), 
                        item.ToString().PascalCaseToDescription())
                ).ToList();
                return result;
            }
        }

        //public string TicketStatusValue
        //{
        //    get { return Ticket.Status.HasValue ? ((int)Ticket.Status).ToString() : TicketStatus.New.ToString(); }
        //}

        public IEnumerable<SelectListItem> EmptySelectList
        {
            get { return new List<SelectListItem>(); }
        }

        //private string[] lostItemCategories;
        //public IEnumerable<SelectListItem> LostItemCategories
        //{
        //    get { return new SelectList(lostItemCategories); }
        //}

        //private string[] lostItemTypes;
        //public IEnumerable<SelectListItem> LostItemTypes
        //{
        //    get { return new SelectList(lostItemTypes); }
        //}

        public string TicketCommentsVisibility
        {
            get
            {
                if (ServicesProxy == null)
                    log.Debug("TicketCommentsVisibility: ServicesProxy is null");
                else if (ServicesProxy.RequestState == null)
                    log.Debug("TicketCommentsVisibility: ServicesProxy.RequestState is null");
                else if (ServicesProxy.RequestState.UserDetails == null)
                    log.Debug("TicketCommentsVisibility: ServicesProxy.RequestState.UserDetails is null");
                return ServicesProxy.RequestState.UserDetails.CanAddTicketComments ? "" : " hidden";
            }
        }

        public bool IsTicketAssignmentVisible
        {
            get { return ServicesProxy.RequestState.UserDetails.CanAssignTicket; }
        }

        public string IsValidCustomerEmail
        {
            get { return Ticket.Contact != null && TicketService.IsValidEmail(Ticket.Contact.Email) ? "true" : "false"; }
        }

        public bool IsTicketLostItemVisible
        {
            get { return Ticket.Reasons != null && Ticket.Reasons.Any(i => i.ToLower().Contains("lost property")); }
        }

        public bool CanCloseTicket
        {
            get { return ServicesProxy.RequestState.UserDetails.CanCloseTicket; }
        }

        public string CanSearchVisibility
        {
            get { return ServicesProxy.RequestState.UserDetails.CanSearchTickets ? "" : " hidden"; }
        }

        public bool IsIncidentTimeAm
        {
            get
            {
                return Ticket != null && Ticket.Incident != null && Ticket.Incident.IncidentAt.HasValue
                    ? Ticket.Incident.IncidentAt.GetValueOrDefault().Hour < 12
                    : false;
                //: DateTime.Now.Hour < 12;
            }
        }

        public bool IsIncidentTimePm
        {
            get
            {
                return Ticket != null && Ticket.Incident != null && Ticket.Incident.IncidentAt.HasValue
                    ? Ticket.Incident.IncidentAt.GetValueOrDefault().Hour >= 12
                    : false;
                //: DateTime.Now.Hour >= 12;
            }
        }

        public string IncidentDate
        {
            get
            {
                return Ticket != null && Ticket.Incident != null && Ticket.Incident.IncidentAt.HasValue
                    ? Ticket.Incident.IncidentAt.GetValueOrDefault().ToString("M/d/yyyy")
                    : "";
                    //: (Ticket != null && Ticket.Id == 0 ? DateTime.Now.ToString("M/d/yyyy") : "");
            }
        }

        public string IncidentTime
        {
            get
            {
                return Ticket != null && Ticket.Incident != null && Ticket.Incident.IncidentAt.HasValue
                    ? Ticket.Incident.IncidentAt.GetValueOrDefault().ToString("h:mm")
                    : "";
                //: (Ticket != null && Ticket.Id == 0 ? DateTime.Now.ToString("h:mm") : "");
            }
        }

        public string Action
        {
            get { return IsEditMode ? "Update" : "Create"; }
        }

        public string DetailsTitle
        {
            get { return IsEditMode ? "Ticket " + Ticket.Id : "New Ticket"; }
        }

        public bool IsEditMode
        {
            get
            {
                return Ticket != null && Ticket.Id > 0;
            }
        }

        public DateTime? TicketAddedAt
        {
            get
            {
                if (Ticket.ChangeHistory == null) return null;
                var value = Ticket.ChangeHistory.Where(t => t != null).Min(t => t.ChangeAt);
                return value.HasValue ? (DateTime?)value.Value : null;
            }
        }

        public ModelStateDictionary ModelState { get; private set; }

        public void Validate()
        {
            if (ModelState != null)
            {
                if (Ticket != null && Ticket.Id == 0 && Ticket.Incident != null)
                {
                    if (!Ticket.Incident.IncidentAt.HasValue)
                        ModelState.AddModelError("ticket.Incident.IncidentAt", "Incident Date/Time cannot be empty.");
                    else if (Ticket.Incident.IncidentAt.Value >= DateTime.Now)
                        ModelState.AddModelError("ticket.Incident.IncidentAt", "Incident Date/Time cannot be in the future.");
                }

                if (!ModelState.IsValid)
                    Errors.AddRange(from modelState in ModelState.Values from error in modelState.Errors select error.ErrorMessage);
            }
        }

        public bool IsValid
        {
            get
            {
                if (Errors.Count == 0)
                    Validate();
                return Errors.Count == 0;
            }
        }

        #endregion

        #region Constructors

        public TicketModel(ServicesProxy servicesProxy)
        {
            log.Debug(string.Format("Create TicketModel({0})", servicesProxy != null ? servicesProxy.ToString() : null));
            ServicesProxy = servicesProxy;
            Init();
        }

        public TicketModel(ServicesProxy servicesProxy, Ticket ticket)
        {
            log.Debug(string.Format("Create TicketModel({0}, {1})", 
                servicesProxy != null ? servicesProxy.ToString() : null, 
                ticket != null ? ticket.Id : -1));
            ServicesProxy = servicesProxy;
            Ticket = ticket;
            Init();
        }

        public TicketModel(ServicesProxy servicesProxy, Ticket ticket, ModelStateDictionary ModelState)
        {
            log.Debug(string.Format("Create TicketModel({0}, {1}, {2})", 
                servicesProxy != null ? servicesProxy.ToString() : null,
                ticket != null ? ticket.Id : -1,
                ModelState != null ? ModelState.IsValid.ToString() : ""));
            ServicesProxy = servicesProxy;
            Ticket = ticket;
            Init();
            this.ModelState = ModelState;
            Validate();
        }

        public TicketModel(ServicesProxy servicesProxy, Ticket ticket, bool isSearching)
        {
            log.Debug(string.Format("Create TicketModel({0}, {1}, {2})",
                servicesProxy != null ? servicesProxy.ToString() : null,
                ticket != null ? ticket.Id : -1,
                isSearching));
            ServicesProxy = servicesProxy;
            Ticket = ticket;
            this.isSearching = isSearching;
            Init();
        }

        public TicketModel(ServicesProxy servicesProxy, Result result): this(servicesProxy)
        {
            ServicesProxy = servicesProxy;
            MergeResults(result);
            log.Debug(string.Format("Create TicketModel: {0}", result));
        }

        public TicketModel(ServicesProxy servicesProxy, TicketResult ticketResult)
        {
            ServicesProxy = servicesProxy;
            if (ticketResult == null)
            {
                ticketResult = servicesProxy.TicketService.GetEmptyTicket(Ticket.EmptyTicketEnum.NewTicket);
                log.Debug(string.Format("TicketService.GetEmptyTicket: {0}", ticketResult));
            }
            if (ticketResult != null)
            {
                Ticket = ticketResult.Ticket;
                MergeResults(ticketResult);
            }
            log.Debug(string.Format("TicketModel: {0}", ToString()));
            if (!OK) return;
            Init();
        }

        private void Init()
        {
            if (Ticket == null) // new ticket
            {
                var ticketResult = ServicesProxy.TicketService.GetEmptyTicket(Ticket.EmptyTicketEnum.NewTicket);
                Ticket = ticketResult.Ticket;
                MergeResults(ticketResult);

                if (ServicesProxy != null && ServicesProxy.RequestState != null)
                    Ticket.Source.ReceivedBy = ServicesProxy.RequestState.UserDetails;

                Ticket.ResponseCriteria.Via = ResponseCriteriaVia.None;
            }

            if (ServicesProxy.RequestState != null && !isSearching)
            {
                Ticket.UpdatedBy = ServicesProxy.RequestState.UserDetails;
                Ticket.UpdatedAt = DateTime.Now;
            }

            var addressStaticLists = ServicesProxy.TicketService.GetAddressStaticLists();

            Cities = addressStaticLists.Cities;
            States = addressStaticLists.States;
            LostItemNodes = ServicesProxy.SettingsService.GetLostItemNodes();
            //lostItemCategories = ServicesProxy.SettingsService.GetSetting(new Setting {Name = "LostItemCategories"}).Setting.Value.Split(';');
            //lostItemTypes = ServicesProxy.SettingsService.GetSetting(new Setting { Name = "LostItemTypes" }).Setting.Value.Split(';');
            ComplaintCodes = ServicesProxy.TicketService.GetComplaintCodes().ComplaintCodes;
            GroupContactValues = ServicesProxy.TicketService.GetGroupContacts().GroupContacts;
            Employees = ServicesProxy.UserService.GetUsers().Users;
            Errors = new List<string>();

            try
            {
                var route = (Ticket.Incident != null ? Ticket.Incident.Route : "");
                var date = Ticket.Incident != null ? (Ticket.Incident.IncidentAt < DateTime.Now ? Ticket.Incident.IncidentAt : DateTime.Now) : null;
                RouteInfo = GetRouteInfo(route, isSearching ? null : date, ServicesProxy);

                //Fix a temporary issue where some routes are assigned to both D2 and D3 after D3 reopening
                //Uncomment if required so in edit mode user will not be able to change the existing selection for routes assigned to more than one division
                //if (IsEditMode && Ticket.Incident != null && RouteInfo.Route == Ticket.Incident.Route && !RouteInfo.Divisions.Contains(Ticket.Incident.Division))
                //{
                //    RouteInfo.Divisions.Clear();
                //    RouteInfo.Divisions.Add(Ticket.Incident.Division);
                //}
            }
            catch (Exception e)
            {
                SetFail(e.InnerException?.Message ?? e.Message);
                log.Debug(e.InnerException?.Message ?? e.Message);
            }

            try
            {
                Operator = Ticket.Operator != null ? ServicesProxy.UserService.GetOperator(Ticket.Operator.Badge).Operator : null;
            }
            catch (Exception e)
            {
                SetFail(e.InnerException?.Message ?? e.Message);
                log.Debug(e.InnerException?.Message ?? e.Message);
                Operator = new Operator();
            }

            if (Ticket.Assignment?.Employee?.Id != null && Ticket.Assignment.Employee.Username == null)
            {
                var userResult = ServicesProxy.UserService.GetUser(Ticket.Assignment.Employee.Id);
                if (userResult.OK && userResult.User != null)
                    Ticket.Assignment.Employee.Username = userResult.User.Username;
            }

            if (Ticket.Source.ReceivedBy?.Id != null && Ticket.Source.ReceivedBy.Username == null)
            {
                var userResult = ServicesProxy.UserService.GetUser(Ticket.Source.ReceivedBy.Id);
                if (userResult.OK && userResult.User != null)
                    Ticket.Source.ReceivedBy.Username = userResult.User.Username;
            }

            if (Ticket.ResearchHistory != null)
                foreach (var item in Ticket.ResearchHistory)
                {
                    var userResult = ServicesProxy.UserService.GetUser(item.ResearchedBy.Id, true);
                    if (userResult.OK && userResult.User != null)
                        item.ResearchedBy = userResult.User;
                }

            if (Ticket.ResponseHistory != null)
                foreach (var item in Ticket.ResponseHistory)
                {
                    var userResult = ServicesProxy.UserService.GetUser(item.ResponseBy.Id, true);
                    if (userResult.OK && userResult.User != null)
                        item.ResponseBy = userResult.User;
                }

            Sanitize();

        }

        /// <summary>
        /// Clean up web input using HtmlSanitizer (https://github.com/mganss/HtmlSanitizer)
        /// HtmlSanitizer is unit tested with the OWASP XSS Filter Evasion Cheat Sheet (https://www.owasp.org/index.php/XSS_Filter_Evasion_Cheat_Sheet)
        /// </summary>
        private void Sanitize()
        {
            // clean up web input
            Ticket = ServicesProxy.TicketService.Sanitize(Ticket);
        }

        public string LastResponse(Ticket ticket)
        {
            if (ticket.ResponseHistory != null && ticket.ResponseHistory.LastOrDefault() != null)
                return ticket.ResponseHistory.Last().ViaAsString.PascalCaseToDescription() + ": " +
                       ticket.ResponseHistory.Last().Comment;
            return "n/a";
        }

        public string FixTableName(string TableName)
        {
            switch (TableName)
            {
                case "Contact":
                    return "Ticket";
                case "ContactHistory":
                    return "Contact History";
                case "ResearchHistory":
                    return "Research History";
            }
            return TableName;
        }

        #endregion

        #region Static Methods

        public static RouteInfo GetRouteInfo(string Route, DateTime? incidentDateTime, ServicesProxy servicesProxy)
        {
            //var cacheKey = "Route" + (Route != null ? " " + Route : "");
            //var routes = AppCache.Cache.Get(cacheKey) as RouteInfo;
            //if (routes != null) return routes;

            var routes = new RouteInfo();
            if (servicesProxy.MapsScheduleService == null) return routes;
            var routeInfoResult = servicesProxy.MapsScheduleService.GetRouteInfo(Route, incidentDateTime);
            if (!routeInfoResult.OK)
                throw new Exception(string.Join(" ", routeInfoResult.Errors));
            routes = routeInfoResult.RouteInfo;

            //AppCache.Set(cacheKey, routes, 86400);
            return routes;
        }

        public static Result UploadFiles(HttpFileCollectionBase Files, ServicesProxy servicesProxy, Ticket ticket)
        {
            var result = new Result();
            foreach (string file in Files)
            {
                var hpf = Files[file];
                if (hpf == null || hpf.ContentLength == 0) continue;
                var input = new byte[hpf.ContentLength];
                hpf.InputStream.Read(input, 0, hpf.ContentLength);
                if (ticket.Attachments == null)
                    ticket.Attachments = new List<Attachment>();
                ticket.Attachments.Add(new Attachment
                {
                    Id = ticket.Id,
                    Filename = hpf.FileName,
                    Data = input,
                    Description = hpf.FileName,
                    UploadedAt = DateTime.Now,
                });
                if (ticket.Id == 0) continue;
                var addResult = servicesProxy.TicketService.AddOrUpdateAttachment(ticket);
                result.MergeResults(addResult);
            }
            return result;            
        }

        #endregion
    }
}