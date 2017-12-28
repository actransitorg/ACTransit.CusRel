using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ACTransit.Contracts.Data.CusRel.Common;
using ACTransit.Contracts.Data.CusRel.LookupContract.Result;
using ACTransit.Contracts.Data.CusRel.ReportContract.Params;
using ACTransit.Contracts.Data.CusRel.TicketContract;
using ACTransit.Contracts.Data.CusRel.TicketContract.Params;
using ACTransit.Contracts.Data.CusRel.TicketContract.Result;
using ACTransit.Contracts.Data.CusRel.UserContract;
using ACTransit.CusRel.Repositories;
using ACTransit.CusRel.Services.Extensions;
using ACTransit.Framework.Extensions;
using ACTransit.Framework.Notification;
using ACTransit.Framework.Web.Infrastructure;

namespace ACTransit.CusRel.Services
{
    public class TicketService
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly TicketRepository ticketRepository;
        private readonly ServicesProxy servicesProxy;

        private bool detactGet;

        public bool DetachGet
        {
            get
            {
                return detactGet;
            }
            set
            {
                detactGet = value;
                if (ticketRepository != null)
                    ticketRepository.DetachGet = value;
            }
        }

        public TicketService(bool DetachGet = false)
        {
            detactGet = DetachGet;
            ticketRepository = new TicketRepository { DetachGet = detactGet };
            servicesProxy = new ServicesProxy();
        }

        public TicketService(ServicesProxy servicesProxy, bool DetachGet = false)
        {
            detactGet = DetachGet;
            ticketRepository = new TicketRepository { DetachGet = detactGet };
            this.servicesProxy = servicesProxy;
        }

        public Ticket PrepareTicket(Ticket Ticket)
        {
            log.Debug(string.Format("Begin PrepareTicket({0})", Ticket.Id));
            try
            {
                return ticketRepository.PrepareTicket(Ticket, servicesProxy.RequestState);
            }
            finally
            {
                log.Debug(string.Format("End PrepareTicket({0})", Ticket.Id));
            }
        }

        public ComplaintCodesResult GetComplaintCodes()
        {
            return ticketRepository.GetComplaintCodes();
        }

        public GroupContactsResult GetGroupContacts(bool includeHidden = false)
        {
            return ticketRepository.GetGroupContacts(
                includeHidden,
                servicesProxy.RequestState.UserDetails.CanViewOnlyDeptTickets ? servicesProxy.RequestState.UserDetails.GroupContact.Code : null);
        }

        public Result UpdateGroupContact(GroupContact groupContact)
        {
            return ticketRepository.UpdateGroupContact(groupContact);
        }

        public AddressStaticListsResult GetAddressStaticLists()
        {
            return ticketRepository.GetAddressStaticLists();
        }

        public TicketResult GetEmptyTicket(Ticket.EmptyTicketEnum emptyTicket = Ticket.EmptyTicketEnum.Blank)
        {
            return new TicketResult
            {
                Ticket = Ticket.EmptyTicket(emptyTicket)
            };
        }

        public TicketResult GetDetached(int Id)
        {
            DetachGet = true;
            var result = ticketRepository.Get(Id);
            DetachGet = false;
            return result;
        }

        public TicketResult Get(int Id)
        {
            return ticketRepository.Get(Id, servicesProxy.RequestState);
        }

        public TicketResult GetPrevious(int Id)
        {
            return ticketRepository.GetPrevious(Id);
        }

        public TicketResult GetNext(int Id)
        {
            return ticketRepository.GetNext(Id);
        }

        public TicketResult GetLast()
        {
            return ticketRepository.GetLast();
        }

        public TicketResult GetRandom()
        {
            return ticketRepository.GetRandom();
        }

        public TicketResult GetLinkedTicket(int TicketId, int LinkedId)
        {
            return ticketRepository.GetLinkedTicket(TicketId, LinkedId);
        }

        public TicketSearchFieldsResult GetTicketSearchFields()
        {
            var ticket = TicketSearchParams.EmptyTicketSearchParams();
            var searchFields = ExtractHelper.Flatten(ticket).ToList();
            return new TicketSearchFieldsResult
            {
                Ticket = ticket,
                SearchFields = searchFields
            };
        }

        public TicketsResult Search(TicketSearchParams Criteria)
        {
            Criteria = FillSearchCriteria(Criteria);
            return ticketRepository.Search(Criteria, servicesProxy.RequestState);
        }

        public Result Create(Ticket Ticket)
        {
            return ticketRepository.Create(PrepareTicket(Ticket));
        }

        public TicketResult Update(Ticket Ticket, Ticket.TicketAction Action = Ticket.TicketAction.Update)
        {
            log.Debug(string.Format("Begin Update({0})", Ticket.Id));

            try
            {
                var result = ticketRepository.Update(PrepareTicket(Ticket));
                log.Debug(string.Format("{0} ticketRepository.Update: {1}", result.OK ? "Call" : "Fail", result));

                if (result.OK)
                {
                    //Send email when ticket is created with an assignee already
                    //Send email when ticket changes assignee
                    if ((Action == Ticket.TicketAction.Create && result.Ticket.Assignment.Employee != null) || 
                        (Action == Ticket.TicketAction.Update && result.Ticket.Changes.AssignmentEmployee && result.Ticket.Assignment.Employee != null))
                    {
                        try
                        {
                            Result emailOperationResult = servicesProxy.EmailService.EmailTicketAssigned(Ticket);

                            if (!emailOperationResult.OK)
                                log.Error(string.Format("TicketService.Update() reported the following errors when trying to send an email to {0} for ticket {1}: {2}", Ticket.Assignment.Employee.Username ?? string.Empty, Ticket.Id, string.Join(",", emailOperationResult.Errors)));
                            else
                                log.Debug(string.Format("TicketService.Update() - Sent assignment notification to {0}", Ticket.Assignment.Employee.Username ?? string.Empty));
                        }
                        catch (Exception ex)
                        {
                            log.Error(string.Format("TicketService.Update() reported the following exception when trying to send an email to {0} for ticket {1}: {2}-{3}", Ticket.Assignment.Employee.Username ?? string.Empty, Ticket.Id, ex.Message, ex.StackTrace));
                        }
                    }

                    //Send email when the priority is increased to high
                    else if (result.Ticket.Changes.TicketPriority && result.Ticket.Priority == TicketPriority.High)
                    {
                        var groupContactResult = servicesProxy.UserService.GetGroupContact(Ticket.Assignment.GroupContact);
                        log.Debug(string.Format("{0} UserService.GetGroupContact: {1}", groupContactResult.OK ? "Call" : "Fail", groupContactResult));
                        result.MergeResults(groupContactResult);

                        if (groupContactResult.OK)
                        {
                            result.Ticket.Assignment.GroupContact = groupContactResult.GroupContact;
                            result.MergeResults(servicesProxy.EmailService.EmailHighPriorityChange(result.Ticket));
                        }
                    }
                }

                return result;
            }
            finally
            {
                log.Debug(string.Format("End Update({0})", Ticket.Id));
                
            }
        }

        public Ticket Sanitize(Ticket ticket)
        {
            return ticketRepository.Sanitize(ticket);
        }

        // -------------------------------------------------------------------

        public Result EmailGroupContactChange(Ticket Ticket)
        {
            var result = new Result();
            //if (Ticket != null && IsValidEmail(Ticket.Assignment.GroupContact.Email))
            //{
            //    var emailDeptChange = servicesProxy.SettingsService.GetSetting("EmailDeptChange").Setting.Value;
            //    var emailCc = Regex.Match(emailDeptChange, "CC:([^|]*)").Groups[1].Value;
            //    var emailBcc = Regex.Match(emailDeptChange, "BCC:([^|]*)").Groups[1].Value;
            //    var emailSubject = Regex.Match(emailDeptChange, "Subject:([^|]*)").Groups[1].Value;
            //    emailSubject = emailSubject.Replace("{Ticket.Id}", Ticket.Id.ToString());
            //    var emailBody = Regex.Match(emailDeptChange, "Body:([^|]*)").Groups[1].Value;
            //    emailBody = emailBody.Replace("{Ticket.Reasons}", Ticket.Reasons != null ? string.Join(", ", Ticket.Reasons.ToArray()) : "");
            //    emailBody = emailBody.Replace("{Ticket.Id}", Ticket.Id.ToString());
            //    emailBody = emailBody.Replace("{Ticket.Comments}", Ticket.Comments);
            //    emailBody = emailBody.Replace("<br />", "<br />\r\n");

            //    try
            //    {
            //        var emailService = new SMTPEmailService(MailSettings.SmtpSection().Network.Host);
            //        var overrideToList = MailSettings.EmailOverrideTo != null ? MailSettings.EmailOverrideTo.ToList() : null;

            //        var payload = new EmailPayload
            //        {
            //            FromEmailAddress = MailSettings.SmtpSection().From,
            //            To = overrideToList != null && overrideToList.Count > 0 ? overrideToList : new List<string> { Ticket.Assignment.GroupContact.Email },
            //            Subject = emailSubject,
            //            Body = emailBody,
            //            CC = !string.IsNullOrEmpty(emailCc) ? emailCc.Split(';').ToList() : null,
            //            BCC = !string.IsNullOrEmpty(emailBcc) ? emailBcc.Split(';').ToList() : null,
            //            IsBodyHtml = true,
            //        };
            //        emailService.Send(payload);
            //    }
            //    catch (Exception e)
            //    {
            //        result.SetFail(e);
            //    }
            //}
            return result;
        }

        // -------------------------------------------------------------------

        //public Regex reDangerousFileExtensions = new Regex(@"\.exe|\.pif|\.application|\.gadget|\.msi|\.msp|\.com|\.scr|\.hta|\.cpl|\.msc|\.jar|\.bat|\.cmd|\.vb|\.vbs|\.vbe|\.js|\.jse|\.ws|\.wsf|\.wsc|\.wsh|\.ps1|\.ps1xml|\.ps2|\.ps2xml|\.psc1|\.psc2|\.msh|\.msh1|\.msh2|\.mshxml|\.msh1xml|\.msh2xml|\.scf|\.lnk|\.inf|\.reg",RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public AttachmentResult GetTicketAttachment(int id, int AttachmentId, bool ErrorOnNoAttachment = false)
        {
            log.Debug(string.Format("Begin GetTicketAttachment({0}, {1}, {2})", id, AttachmentId, ErrorOnNoAttachment));
            try
            {
                var ticketResult = Get(id);
                log.Debug(string.Format("{0} Get: {1}", ticketResult.OK ? "Call" : "Fail", ticketResult));
                if (!ticketResult.OK)
                    return Result<AttachmentResult>.FailedResult("Unknown Id");
                    
                var ticket = ticketResult.Ticket;
                var attachment = ticket.Attachments.FirstOrDefault(t => t.Id == AttachmentId);
                if (ErrorOnNoAttachment && attachment == null)
                {
                    log.Debug(string.Format("Unknown Attachment Id"));
                    return Result<AttachmentResult>.FailedResult("Unknown Attachment Id");
                }
                return new AttachmentResult
                {
                    Attachment = attachment,
                    Ticket = ticket
                };
            }
            finally
            {
                log.Debug(string.Format("End GetTicketAttachment({0}, {1}, {2})", id, AttachmentId, ErrorOnNoAttachment));
                
            }
        }
        public Result AddOrUpdateAttachment(Ticket Ticket)
        {
            return ticketRepository.AddOrUpdateAttachment(PrepareTicket(Ticket));
        }

        public Result AddOrUpdateAttachment(int id, Attachment attachment)
        {
            log.Debug(string.Format("Begin AddAttachment({0}, {1})", id, attachment.Filename));
            try
            {
                var attachmentResult = GetTicketAttachment(id, attachment.Id, attachment.Id > 0);
                log.Debug(string.Format("{0} GetTicketAttachment: {1}", attachmentResult.OK ? "Call" : "Fail", attachmentResult));
                if (!attachmentResult.OK)
                    return attachmentResult;

                var ticket = attachmentResult.Ticket;

                // set FileSize to 0 for existing attachments so they will be ignored later in ticketEntities.PrepareContext() 
                foreach (var attach in ticket.Attachments)
                    attach.Data = new byte[0];

                log.Debug(string.Format("attachment.Id: {0}", attachment.Id));

                if (attachment.Id == 0)
                    ticket.Attachments.Add(attachment);
                else
                    ticket.Attachments[ticket.Attachments.IndexOf(attachment)] = attachment;

                var result = ticketRepository.AddOrUpdateAttachment(PrepareTicket(ticket));
                log.Debug(string.Format("{0} ticketRepository.AddOrUpdateAttachment: {1}", result.OK ? "Call" : "Fail", result));
                return result;
            }
            finally
            {
                log.Debug(string.Format("End AddAttachment({0}, {1})", id, attachment.Filename));
            }
        }

        public Result DeleteAttachment(Ticket Ticket, Attachment Attachment)
        {
            return ticketRepository.DeleteAttachment(Ticket, Attachment);
        }

        public Result DeleteAttachment(int id, int AttachmentId)
        {
            ticketRepository.DetachGet = true;
            var attachmentResult = GetTicketAttachment(id, AttachmentId);
            if (!attachmentResult.OK) 
                return attachmentResult;
            var ticket = attachmentResult.Ticket;
            var attachment = attachmentResult.Attachment;
            ticketRepository.DetachGet = false;
            return ticketRepository.DeleteAttachment(ticket, attachment);
        }

        // -------------------------------------------------------------------

        public Result AddOrUpdateLinkedTicket(Ticket Ticket)
        {
            return ticketRepository.AddOrUpdateLinkedTicket(Ticket);
        }

        public Result AddOrUpdateLinkedTicket(int id, int LinkedId)
        {
            servicesProxy.TicketService.DetachGet = true;
            var ticketResult = servicesProxy.TicketService.Get(id);
            if (!ticketResult.OK) return ticketResult;
            var ticket = ticketResult.Ticket;

            servicesProxy.TicketService.DetachGet = true;
            var linkedTicketResult = servicesProxy.TicketService.Get(LinkedId);
            if (!linkedTicketResult.OK) return linkedTicketResult;
            var linkedTicket = linkedTicketResult.Ticket;

            servicesProxy.TicketService.DetachGet = false;
            ticket.LinkedTickets.Add(linkedTicket);
            return servicesProxy.TicketService.AddOrUpdateLinkedTicket(ticket);
        }

        public Result DeleteLinkedTicket(Ticket Ticket, Ticket LinkedTicket)
        {
            return ticketRepository.DeleteLinkedTicket(Ticket, LinkedTicket);
        }

        public Result DeleteLinkedTicket(int id, int LinkedId)
        {
            servicesProxy.TicketService.DetachGet = true;
            var ticketResult = servicesProxy.TicketService.Get(id);
            if (!ticketResult.OK) return ticketResult;
            var ticket = ticketResult.Ticket;

            var linkedTicket = ticket.LinkedTickets.FirstOrDefault(t => t.Id == LinkedId);
            if (linkedTicket == null) return Result.FailedResult("LinkedId not found");

            servicesProxy.TicketService.DetachGet = false;
            return servicesProxy.TicketService.DeleteLinkedTicket(ticket, linkedTicket);
        }

        // -------------------------------------------------------------------

        public Result AddOrUpdateResearchHistory(Ticket Ticket)
        {
            return ticketRepository.AddOrUpdateResearchHistory(Ticket);
        }


        public Result AddResearchHistory(int id, DateTime? dateResearchHistory, string commentResearchHistory)
        {
            if (string.IsNullOrWhiteSpace(commentResearchHistory))
                return Result.FailedResult("Comment is required.");

            var researchedBy = servicesProxy.RequestState.UserDetails;
            var ticket = PrepareTicket(servicesProxy.TicketService.Get(id).Ticket);

            var newResearchHistory = new ResearchHistory
            {
                ResearchedAt = dateResearchHistory ?? DateTime.Now,
                Comment = commentResearchHistory,
                ResearchedBy = researchedBy
            };

            ticket.ResearchHistory.Add(newResearchHistory);
            var result = servicesProxy.TicketService.AddOrUpdateResearchHistory(ticket);
            return result;
        }

        // -------------------------------------------------------------------

        public Result AddOrUpdateResponseHistory(Ticket Ticket)
        {
            return ticketRepository.AddOrUpdateResponseHistory(Ticket);
        }

        public class AddResponseDetails
        {
            public int id;
            public DateTime? dateResponseHistory;
            public string commentResponseHistory;
            public ResponseHistoryVia? ViaResponseHistory;
            public bool? sendAsEmail;
            public string emailSender;
            public string emailCc;
            public string emailBcc;
        }

        public Result AddResponseHistory(AddResponseDetails details)
        {
            //if (string.IsNullOrWhiteSpace(details.commentResponseHistory))
            //    return Result.FailedResult("Comment is required.");

            var ticket = PrepareTicket(servicesProxy.TicketService.GetDetached(details.id).Ticket);

            if (details.ViaResponseHistory == null)
                return Result.FailedResult("Via cannot be empty");

            // overwrite previous entries in order to not touch them.
            ticket.ResponseHistory = new List<ResponseHistory>
            {
                new ResponseHistory
                {
                    ResponseAt = details.dateResponseHistory ?? DateTime.Now,
                    Comment = details.commentResponseHistory,
                    ResponseBy = servicesProxy.RequestState.UserDetails,
                    Via = details.ViaResponseHistory.Value
                }
            };

            var result = servicesProxy.TicketService.AddOrUpdateResponseHistory(ticket);

            //if (result.OK && MailSettings.EmailEnabled && ticket.Contact.Email != null && IsValidEmail(ticket.Contact.Email))
            //{
            //    try
            //    {
            //        var emailService = new SMTPEmailService(MailSettings.SmtpSection().Network.Host);
            //        var overrideToList = MailSettings.EmailOverrideTo != null ? MailSettings.EmailOverrideTo.ToList() : null;

            //        var payload = new EmailPayload
            //        {
            //            FromEmailAddress = MailSettings.SmtpSection().From,
            //            To = overrideToList != null && overrideToList.Count > 0 ? overrideToList : new List<string> { ticket.Contact.Email },
            //            Subject = string.Format(MailSettings.EmailSubject, details.id),
            //            Body = details.commentResponseHistory,
            //            CC = !string.IsNullOrEmpty(details.emailCc) ? details.emailCc.Split(';').ToList() : null,
            //            BCC = !string.IsNullOrEmpty(details.emailBcc) ? details.emailBcc.Split(';').ToList() : null,
            //            IsBodyHtml = true,
            //        };
            //        emailService.Send(payload);
            //    }
            //    catch (Exception e)
            //    {
            //        result.SetFail(e);
            //    }
            //}

            return result;
        }

        // -------------------------------------------------------------------

        public static bool IsValidEmail(string value)
        {
            var re = new Regex(@"\b[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,6}\b", RegexOptions.IgnoreCase);
            return re.IsMatch(value);
        }

        public Result Close(ReadyToCloseReportParams reportParams)
        {
            foreach (var item in reportParams.Items)
            {
                var enumItem = item.CurrentStatus.NullableTrim().DescriptionToPascalCase().EnumParse(TicketStatus.Closed);
                switch (enumItem)
                {
                    case TicketStatus.ReadyToClose:
                        item.NewStatus = TicketStatus.Closed.ToString().PascalCaseToDescription();
                        break;
                    case TicketStatus.ReadyToCloseDuplicate:
                        item.NewStatus = TicketStatus.ClosedDuplicate.ToString().PascalCaseToDescription();
                        break;
                    case TicketStatus.ReadyToCloseTooLate:
                        item.NewStatus = TicketStatus.ClosedTooLate.ToString().PascalCaseToDescription();
                        break;
                }
            }
            return ticketRepository.UpdateTicketStatus(reportParams, servicesProxy.RequestState);
        }

        private TicketSearchParams FillSearchCriteria(TicketSearchParams Criteria)
        {
            if (servicesProxy.UserService != null)
            {

                // translate Username to Id when applicable

                if (Criteria.Assignment.Employee.Id == null && Criteria.Assignment.Employee.Username != null)
                {
                    var userResult = servicesProxy.UserService.GetUser(Criteria.Assignment.Employee.Username);
                    if (userResult.OK)
                        Criteria.Assignment.Employee.Id = userResult.User.Id;
                }

                if (Criteria.Source.ReceivedBy.Id == null && Criteria.Source.ReceivedBy.Username != null)
                {
                    var userResult = servicesProxy.UserService.GetUser(Criteria.Source.ReceivedBy.Username);
                    if (userResult.OK)
                        Criteria.Source.ReceivedBy.Id = userResult.User.Id;
                }

                if (Criteria.UpdatedBy != null && Criteria.UpdatedBy.Id == null && Criteria.UpdatedBy.Username != null)
                {
                    var userResult = servicesProxy.UserService.GetUser(Criteria.UpdatedBy.Username);
                    if (userResult.OK)
                        Criteria.UpdatedBy.Id = userResult.User.Id;
                }

                // Id is higher precedence than Username

                if (Criteria.Assignment.Employee.Id != null && Criteria.Assignment.Employee.Username != null)
                    Criteria.Assignment.Employee.Username = null;

                if (Criteria.Source.ReceivedBy.Id == null && Criteria.Source.ReceivedBy.Username != null)
                    Criteria.Source.ReceivedBy.Username = null;

                if (Criteria.UpdatedBy != null && Criteria.UpdatedBy.Id == null && Criteria.UpdatedBy.Username != null)
                    Criteria.UpdatedBy.Username = null;
            }
            return Criteria;
        }
    }
}
