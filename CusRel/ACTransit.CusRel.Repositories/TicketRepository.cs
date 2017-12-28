using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text.RegularExpressions;
using ACTransit.Contracts.Data.CusRel.Common;
using ACTransit.Contracts.Data.CusRel.LookupContract.Result;
using ACTransit.Contracts.Data.CusRel.ReportContract.Params;
using ACTransit.Contracts.Data.CusRel.TicketContract;
using ACTransit.Contracts.Data.CusRel.TicketContract.Params;
using ACTransit.Contracts.Data.CusRel.TicketContract.Result;
using ACTransit.Contracts.Data.CusRel.UserContract;
using ACTransit.CusRel.Repositories.Search;
using ACTransit.DataAccess.CustomerRelations;
using ACTransit.DataAccess.Transportation;
using ACTransit.CusRel.Repositories.Mapping;
using ACTransit.Framework.Extensions;
using Ganss.XSS;
using System.Web;

namespace ACTransit.CusRel.Repositories
{
    public class TicketRepository: IDisposable
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private CusRelEntities cusRelContext;
        private TransportationEntities transportationContext;
        private TicketEntitiesRepository ticketEntitiesRepository;

        /// <summary>
        /// DetachGet is useful for special case: updating/inserting a TicketEntities object graph, itself created from a get operation (not from a client request).  
        /// This is a side effect from the population of TicketEntities.
        /// </summary>
        public bool DetachGet { get; set; }

        #region Constructors / Initialization

        public TicketRepository()
        {
            init();
        }

        public TicketRepository(CusRelEntities context)
        {
            cusRelContext = context;
            init();
        }

        // =============================================================

        private void init()
        {
            if (cusRelContext != null && ticketEntitiesRepository == null)
                ticketEntitiesRepository = new TicketEntitiesRepository(cusRelContext);
            if (ticketEntitiesRepository != null)
                ticketEntitiesRepository.DetachGet = DetachGet;
        }

        private void InitCusRelContext()
        {
            if (cusRelContext == null)
                cusRelContext = new CusRelEntities();
            init();
        }

        private void InitTransportationContext()
        {
            if (transportationContext == null)
                transportationContext = new TransportationEntities();
            init();
        }

        #endregion

        // =============================================================

        #region Static Objects

        private static readonly Regex reRemoveMsLynxPhone = new Regex("(<span>)(?<q2>[^<]*)(<a target=\"_blank\" rel=\"nofollow\"><img alt=\".*?\"></a></span>)", RegexOptions.IgnoreCase);

        #endregion

        // =============================================================

        #region Save/Dispose

        public void Save()
        {
            if (cusRelContext != null)
                cusRelContext.SaveChanges();
            if (transportationContext != null)
                transportationContext.SaveChanges();
        }

        private bool disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed && disposing)
            {
                if (cusRelContext != null)
                    cusRelContext.Dispose();
                if (transportationContext != null)
                    transportationContext.Dispose();
            }
                
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private Ticket Fill(Ticket ticket, int depth = 1)
        {
            if (cusRelContext == null || ticket == null)
                return ticket;

            depth -= 1;

            if (ticket.LinkedTickets != null && depth >= 0)
                for (var idx = ticket.LinkedTickets.Count - 1; idx >= 0; idx--)
                {
                    if (!ticket.LinkedTickets[idx].LinkedId.HasValue) 
                        throw new NoNullAllowedException("LinkedId cannot be null for child tickets");
                    ticket.LinkedTickets[idx] = GetLinkedTicket(ticket.LinkedTickets[idx].Id, ticket.LinkedTickets[idx].LinkedId.Value, depth).Ticket;
                }
                    
            return ticket;
        }

        #endregion

        // =============================================================

        #region Private Constants

        private readonly Regex reDangerousFileExtensions = new Regex(@"\.exe|\.pif|\.application|\.gadget|\.msi|\.msp|\.com|\.scr|\.hta|\.cpl|\.msc|\.jar|\.bat|\.cmd|\.vb|\.vbs|\.vbe|\.js|\.jse|\.ws|\.wsf|\.wsc|\.wsh|\.ps1|\.ps1xml|\.ps2|\.ps2xml|\.psc1|\.psc2|\.msh|\.msh1|\.msh2|\.mshxml|\.msh1xml|\.msh2xml|\.scf|\.lnk|\.inf|\.reg", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        #endregion

        #region Public Methods

        public Ticket PrepareTicket(Ticket Ticket, RequestState requestState = null)
        {
            log.Debug(string.Format("Begin PrepareTicket({0})", Ticket.Id));
            try
            {
                if (requestState != null)
                {
                    Ticket.UpdatedBy = requestState.UserDetails;
                    Ticket.UpdatedAt = DateTime.Now;
                }

                if (Ticket.Comments != null)
                    Ticket.Comments = reRemoveMsLynxPhone.Replace(Ticket.Comments, "${q2}");

                // set Resolution DateTime if applicable
                if (Ticket.Status == TicketStatus.Closed || Ticket.Status == TicketStatus.ClosedDuplicate || Ticket.Status == TicketStatus.ClosedTooLate)
                {
                    log.Debug(string.Format("Closed Ticket"));
                    if (Ticket.Resolution == null)
                        Ticket.Resolution = new Resolution();
                    if (Ticket.Resolution.ResolvedAt == null)
                        Ticket.Resolution.ResolvedAt = DateTime.Now;
                }
                else
                    Ticket.Resolution = new Resolution
                    {
                        ResolvedAt = null
                    };

                if (Ticket.Attachments != null && Ticket.Attachments.Any())
                {
                    foreach (var attachment in Ticket.Attachments)
                    {
                        if (reDangerousFileExtensions.IsMatch(attachment.Filename))
                        {
                            log.Debug(string.Format("Deleting dangerous file: {0}", attachment.Filename));
                            attachment.ShouldDelete = true;
                        }
                    }
                    
                    var attachmentRegex = new Regex("data:(.*?);base64,(.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                    foreach (var item in Ticket.Attachments.Where(item => item.Base64Data != null && item.Data == null))
                    {
                        log.Debug(string.Format("Attachment {0}", item.Filename));
                        if (item.Base64Data == null) continue;
                        var attachmentMatch = attachmentRegex.Match(item.Base64Data);
                        if (attachmentMatch.Success)
                        {
                            item.Base64Data = attachmentMatch.Groups[2].Value;
                            item.Data = Convert.FromBase64String(item.Base64Data);
                            item.IsNew = true;
                        }
                        log.Debug(string.Format("Attachment {0}, Base64.Length: {1}, Data.Length: {2}", item.Filename, item.Base64Data != null ? item.Base64Data.Length : -1, item.Data.Length));
                        item.Base64Data = null;
                    }
                }

                return Sanitize(Ticket);
            }
            finally
            {
                log.Debug(string.Format("End PrepareTicket({0})", Ticket.Id));
            }
        }

        public Ticket Sanitize(Ticket ticket)
        {
            // clean up web input
            ticket.Comments = sanitize(ticket.Comments);
            ticket.Contact.Name.First = sanitize(ticket.Contact.Name.First);
            ticket.Contact.Name.Last = sanitize(ticket.Contact.Name.Last);
            ticket.Contact.Address.Addr1 = sanitize(ticket.Contact.Address.Addr1);
            ticket.Contact.Address.Addr2 = sanitize(ticket.Contact.Address.Addr2);
            ticket.Contact.Address.City = sanitize(ticket.Contact.Address.City);
            ticket.Contact.Address.State = sanitize(ticket.Contact.Address.State);
            ticket.Contact.Address.ZipCode = sanitize(ticket.Contact.Address.ZipCode);
            ticket.Contact.Phone.Number = sanitize(ticket.Contact.Phone.Number);
            ticket.Contact.Email = sanitize(ticket.Contact.Email);
            ticket.Incident.City = sanitize(ticket.Incident.City);
            ticket.Incident.Destination = sanitize(ticket.Incident.Destination);
            ticket.Incident.Division = sanitize(ticket.Incident.Division);
            ticket.Incident.Location = sanitize(ticket.Incident.Location);
            ticket.Incident.Route = sanitize(ticket.Incident.Route);
            ticket.Incident.VehicleNumber = sanitize(ticket.Incident.VehicleNumber);
            ticket.LostItem.Category = sanitize(ticket.LostItem.Category);
            ticket.LostItem.Type = sanitize(ticket.LostItem.Type);
            ticket.Operator.Badge = sanitize(ticket.Operator.Badge);
            ticket.Operator.Description = sanitize(ticket.Operator.Description);
            ticket.Operator.Info = sanitize(ticket.Operator.Info);
            ticket.Operator.Name = sanitize(ticket.Operator.Name);
            if (ticket.ChangeHistory != null)
                foreach (var change in ticket.ChangeHistory)
                    change.NewValue = sanitize(change.NewValue);
            if (ticket.ResponseHistory != null)
                foreach (var response in ticket.ResponseHistory)
                    response.Comment = sanitize(response.Comment);
            return ticket;
        }

        private HtmlSanitizer sanitizer;
        private string sanitize(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                if (sanitizer == null)
                    sanitizer = new HtmlSanitizer();
                var result = HttpUtility.HtmlDecode(sanitizer.Sanitize(text));
                if (text != result)
                    log.Debug(string.Format("Sanitized \"{0}\"", text));
            }
            return text;
        }

        public ComplaintCodesResult GetComplaintCodes()
        {
            InitCusRelContext();
            return new ComplaintCodesResult
            {
                ComplaintCodes = cusRelContext.tblCustomerComplaintCodes.ToList().Where(c => (c.IsVisible.GetValueOrDefault())).OrderBy(c => c.Order).ToList().FromEntities()
            };
        }

        public GroupContactsResult GetGroupContacts(bool includeHidden = false, string referCode = null)
        {
            InitCusRelContext();
            return new GroupContactsResult
            {
                GroupContacts = cusRelContext.tblCustomerReferenceCodes.ToList()
                    .Where(c => referCode == null || c.REFER_CODE.Equals(referCode))
                    .Where(c => c.IsVisible.GetValueOrDefault() || includeHidden)
                    .OrderBy(c => c.Order).ToList().FromEntities()
            };
        }

        public Result UpdateGroupContact(GroupContact groupContact)
        {
            InitCusRelContext();
            cusRelContext.tblCustomerReferenceCodes.AddOrUpdate(groupContact.ToEntity());
            Save();
            return new Result(groupContact.Code);
        }

        public AddressStaticListsResult GetAddressStaticLists()
        {
            return new AddressStaticListsResult();
        }

        public TicketResult Get(int id, RequestState requestState = null)
        {
            InitCusRelContext();

            var result = new TicketResult
            {
                Ticket = Fill(ticketEntitiesRepository.Get(id).FromEntities())
            };

            if (result.Ticket == null)
                result.SetFail("No ticket found");
            else if (requestState != null)
                TrackView(id, requestState.UserDetails.Id); //Track ticket view

            return result;
        }

        public TicketResult GetPrevious(int Id)
        {
            InitCusRelContext();
            var result = new TicketResult
            {
                Ticket = Fill(ticketEntitiesRepository.GetPrevious(Id).FromEntities())
            };
            if (result.Ticket == null)
                result.SetFail("No ticket found");
            return result;
        }

        public TicketResult GetNext(int Id)
        {
            InitCusRelContext();
            var result = new TicketResult
            {
                Ticket = Fill(ticketEntitiesRepository.GetNext(Id).FromEntities())
            };
            if (result.Ticket == null)
                result.SetFail("No ticket found");
            return result;
        }

        public TicketResult GetLast()
        {
            InitCusRelContext();
            var result = new TicketResult
            {
                Ticket = Fill(ticketEntitiesRepository.GetLast().FromEntities())
            };
            if (result.Ticket == null)
                result.SetFail("No ticket found");
            return result;
        }

        public TicketResult GetRandom()
        {
            InitCusRelContext();
            var result = new TicketResult
            {
                Ticket = Fill(ticketEntitiesRepository.GetRandom().FromEntities())
            };
            if (result.Ticket == null)
                result.SetFail("No ticket found");
            return result;
        }

        public TicketResult GetLinkedTicket(int TicketId, int LinkedId, int depth = 1)
        {
            InitCusRelContext();
            var result = new TicketResult
            {
                Ticket = Fill(ticketEntitiesRepository.Get(TicketId).FromEntities(LinkedId), depth) // get dependent entities too
                //Ticket = ticketEntitiesRepository.GetContact(TicketId).FromEntities(LinkedId)
            };
            if (result.Ticket == null)
                result.SetFail("No ticket found");
            return result;
        }

        public TicketsResult Search(TicketSearchParams Criteria, RequestState RequestState)
        {
            InitCusRelContext();
            var result = new TicketsResult
            {
                Tickets = Criteria.Search(RequestState)
            };
            if (result.Tickets == null)
                result.SetFail("No tickets found");
            return result;
        }

        public Result Create(Ticket Ticket)
        {
            return Update(Ticket);
        }

        public TicketResult Update(Ticket Ticket)
        {
            log.Debug(string.Format("Begin Update({0})", Ticket.Id));
            try
            {
                InitCusRelContext();
                var result = ticketEntitiesRepository.AddOrUpdate(Ticket);
                log.Debug(string.Format("{0} ticketEntitiesRepository.AddOrUpdate: {1}", result.OK ? "Call" : "Fail", result));
                return new TicketResult(result, Ticket);
            }
            finally
            {
                log.Debug(string.Format("End Update({0})", Ticket.Id));
            }
        }

        public Result AddOrUpdateAttachment(Ticket Ticket)
        {
            log.Debug(string.Format("Begin AddOrUpdateAttachment {0}, Count:{1}", Ticket.Id, Ticket.Attachments != null ? Ticket.Attachments.Count : 0));
            try
            {
                InitCusRelContext();
                var result = ticketEntitiesRepository.AddOrUpdateAttachment(Ticket);
                log.Debug(string.Format("{0} ticketEntitiesRepository.AddOrUpdateAttachment: {1}", result.OK ? "Call" : "Fail", result));
                return result;
            }
            finally
            {
                log.Debug(string.Format("End AddOrUpdateAttachment {0}, Count:{1}", Ticket.Id, Ticket.Attachments != null ? Ticket.Attachments.Count : 0));
            }
        }

        public Result AddOrUpdateResponseHistory(Ticket Ticket)
        {
            InitCusRelContext();
            return ticketEntitiesRepository.AddOrUpdateResponseHistory(Ticket);
        }

        public Result AddOrUpdateResearchHistory(Ticket Ticket)
        {
            InitCusRelContext();
            return ticketEntitiesRepository.AddOrUpdateResearchHistory(Ticket);
        }

        public Result AddOrUpdateLinkedTicket(Ticket Ticket)
        {
            InitCusRelContext();
            return ticketEntitiesRepository.AddOrUpdateLinkedTicket(Ticket);
        }

        public Result DeleteAttachment(Ticket Ticket, Attachment Attachment)
        {
            InitCusRelContext();
            return ticketEntitiesRepository.DeleteAttachment(Ticket, Attachment);
        }

        public Result DeleteLinkedTicket(Ticket Ticket, Ticket LinkedTicket)
        {
            InitCusRelContext();
            return ticketEntitiesRepository.DeleteLinkedTicket(Ticket, LinkedTicket);
        }

        public Result UpdateTicketStatus(ReadyToCloseReportParams reportParams, RequestState requestState = null)
        {
            var result = new Result();
            foreach (var item in reportParams.Items)
            {
                var ticketResult = Get(item.Id);
                result.MergeResults(ticketResult);
                if (!ticketResult.OK) continue;

                var ticket = ticketResult.Ticket;
                ticket.Status = item.NewStatus.NullableTrim().DescriptionToPascalCase().EnumParse(TicketStatus.Closed);
                var updateResult = Update(PrepareTicket(ticket, requestState));
                result.MergeResults(updateResult);
            }
            return result;
        }

        public Result TrackView(int ticketId, string userId, DateTime? viewDate = null)
        {
            InitCusRelContext();

            return ticketEntitiesRepository.TrackView(ticketId, userId, viewDate);
        }

        #endregion

        // =============================================================

        #region Test Data

        //private RouteInfo testRouteInfo1()
        //{
        //    return new RouteInfo
        //    {
        //        Routes = new List<string>
        //        {
        //            "1","1R","7","11","12","14","18","20","21","22","25","26","31","32","39","40","45","46","47","48","49","51A","51B","52","54","57","58L","60","62","65","67","70","71","72","72M","72R","73","74","75","76","83","85","86","88","89","93","94","95","97","98","99",
        //            "200","210","212","215","216","217","232","239","251","275","399","604","605","606","607","618","620","621","623","625","626","628","629","631","638","641","642","643","646","648","649","650","651","652","653","654","657","658","660","663","667","668","669","671","672","675","680","682","684","687","688","696","800","801","802","805","851",
        //            "B","BSD","BSN","C","CB","E","F","FS","G","H","J","L","LA","M","NL","NX","NX1","NX2","NX3","NX4","O","OX","P","S","SB","U","V"
        //        },
        //        Directions = new List<string>
        //        {
        //            "1Dir",
        //            "Clockwise",
        //            "East",
        //            "CounterClockwise",
        //            "North",
        //            "South",
        //            "West"
        //        },
        //        Divisions = new List<string>
        //        {
        //            "D2", "D4", "D6", "GO", "TRN", "Unknown"
        //        }
        //    };
        //}

        #endregion
    }

    // =============================================================
}
