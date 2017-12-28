using System;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Validation;
using System.Linq;
using ACTransit.Contracts.Data.CusRel.Common;
using ACTransit.Contracts.Data.CusRel.TicketContract;
using ACTransit.DataAccess.CustomerRelations;
using ACTransit.Entities.CustomerRelations;
using ACTransit.CusRel.Repositories.Mapping;

namespace ACTransit.CusRel.Repositories
{
    public class TicketEntitiesRepository : IDisposable
    {
        private readonly CusRelEntities context;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public bool DetachGet { get; set; }

        public TicketEntitiesRepository()
        {
            context = new CusRelEntities();
        }

        public TicketEntitiesRepository(CusRelEntities context)
        {
            this.context = context;
        }

        // =============================================================

        #region Save/Dispose

        public int SaveChanges(Audit audit = null)
        {
            log.Debug("Begin SaveChanges");
            try
            {
                var changeCount = context.SaveChanges();
                log.Debug(string.Format("context.SaveChanges: {0}", changeCount));
                if (audit == null || changeCount == 0) return changeCount;

                UpdateAudit(audit);

                return changeCount;
            }
            finally
            {
                log.Debug("Begin SaveChanges");
            }
        }

        private bool disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed && disposing)
                context.Dispose();
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public class Audit
        {
            public int TicketId;
            public string Username;

            public Audit(TicketEntities ticketEntities)
            {
                TicketId = ticketEntities.Contact.FileNum;
                Username = ticketEntities.Contact.updatedBy;
            }
        }

        #endregion

        // =============================================================

        [Flags]
        internal enum TicketInclude
        {
            Ticket = 1,
            Attachment = 2,
            ResponseHistory = 4,
            ResearchHistory = 8,
            IncidentUpdateHistory = 16,
            LinkedTicket = 32,
            ChangeHistory = 64,
        }

        private Ticket FilterTicket(Ticket Ticket, params TicketInclude[] include)
        {
            log.Debug(string.Format("Begin FilterTicket({0}, {1})", Ticket.Id, include.Length));
            try
            {
                if (!include.Any(t => t.HasFlag(TicketInclude.Ticket)))
                {
                    log.Debug(string.Format("Ticket.ShouldIgnore"));
                    Ticket.ShouldIgnore = true;
                }
                if (!include.Any(t => t.HasFlag(TicketInclude.Attachment)))
                {
                    log.Debug(string.Format("Ignoring Ticket.Attachments"));
                    Ticket.Attachments = null;
                }
                if (!include.Any(t => t.HasFlag(TicketInclude.ResponseHistory)))
                {
                    log.Debug(string.Format("Ignoring Ticket.ResponseHistory"));
                    Ticket.ResponseHistory = null;
                }
                if (!include.Any(t => t.HasFlag(TicketInclude.ResearchHistory)))
                {
                    log.Debug(string.Format("Ignoring Ticket.ResearchHistory"));
                    Ticket.ResearchHistory = null;
                }
                if (!include.Any(t => t.HasFlag(TicketInclude.LinkedTicket)))
                {
                    log.Debug(string.Format("Ignoring Ticket.LinkedTickets"));
                    Ticket.LinkedTickets = null;
                }
                if (!include.Any(t => t.HasFlag(TicketInclude.ChangeHistory)))
                {
                    log.Debug(string.Format("Ignoring Ticket.ChangeHistory"));
                    Ticket.ChangeHistory = null;
                }
                    
                return Ticket;
            }
            finally
            {
                log.Debug(string.Format("End FilterTicket({0}, {1})", Ticket.Id, include.Length));
            }
        }

        private TicketEntities Fill(tblContacts contact)
        {
            log.Debug(string.Format("Begin Fill({0})", contact.FileNum));
            try
            {
                var Id = contact.FileNum;
                var result = new TicketEntities
                {
                    Contact = contact,
                    Attachments = context.tblAttachments.Where(a => a.FileNum == Id).ToList(),
                    ResponseHistory = context.tblContactHistory.Where(a => a.FileNum == Id).ToList(),
                    ResearchHistory = context.tblResearchHistory.Where(a => a.FileNum == Id).ToList(),
                    IncidentUpdateHistory = context.tblIncidentUpdateHistory.Where(a => a.FileNum == Id).ToList(),
                    LinkedContacts = context.tblLinkedContacts.Where(a => a.FileNum == Id).ToList(),
                    ChangeHistory = context.tblUpdateLog.Where(a => a.FileNum == Id).ToList(),
                };
                if (DetachGet)
                {
                    log.Debug(string.Format("Detaching new TicketEntities"));
                    result.Detach(context);
                }
                    
                return result;
            }
            finally
            {
                log.Debug(string.Format("End Fill({0})", contact.FileNum));
            }
        }

        public TicketEntities GetContact(int Id)
        {
            var result = new TicketEntities
            {
                Contact = context.tblContacts.FirstOrDefault(c => c.FileNum == Id)
            };
            if (!DetachGet) return result;
            log.Debug(string.Format("Detaching {0}", result.Contact.FileNum));
            result.Detach(context);
            return result;
        }

        public TicketEntities Get(int Id)
        {
            var contact = context.tblContacts.FirstOrDefault(c => c.FileNum == Id);
            return contact == null ? null : Fill(contact);
        }

        public TicketEntities GetPrevious(int Id)
        {
            var contact = context.tblContacts.OrderByDescending(c => c.FileNum).FirstOrDefault(c => c.FileNum < Id);
            return contact == null ? null : Fill(contact);
        }

        public TicketEntities GetNext(int Id)
        {
            var contact = context.tblContacts.OrderBy(c => c.FileNum).FirstOrDefault(c => c.FileNum > Id);
            return contact == null ? null : Fill(contact);
        }

        public TicketEntities GetLast()
        {
            var contact = context.tblContacts.OrderByDescending(c => c.FileNum).FirstOrDefault();
            return contact == null ? null : Fill(contact);
        }

        public TicketEntities GetRandom()
        {
            var contact = context.tblContacts.OrderBy(c => Guid.NewGuid()).FirstOrDefault();
            return contact == null ? null : Fill(contact);
        }

        public TicketEntities TrackChanges(TicketEntities TicketEntities, TicketEntities currentTicketEntities)
        {
            TicketEntities.Changes.ForAction = (TicketEntities.Contact.ForAction ?? "").ToLower().Trim() != (currentTicketEntities.Contact.ForAction ?? "").ToLower().Trim();
            TicketEntities.Changes.AssignedTo = (TicketEntities.Contact.AssignedTo ?? "").ToLower().Trim() != (currentTicketEntities.Contact.AssignedTo ?? "").ToLower().Trim();
            TicketEntities.Changes.Priority = TicketEntities.Contact.Priority != currentTicketEntities.Contact.Priority;
            return TicketEntities;
        }

        public TicketEntities ManageChanges(TicketEntities TicketEntities)
        {
            log.Debug(string.Format("Begin ManageChanges({0})", TicketEntities.Contact.FileNum));
            try
            {
                if (TicketEntities.Contact.FileNum < 1) return TicketEntities;
                var currentTicketEntities = Get(TicketEntities.Contact.FileNum);
                var contact = TicketEntities.Contact;

                // Preserve deprecated columns
                contact.ForInfos = currentTicketEntities.Contact.ForInfos;
                contact.Resolution = currentTicketEntities.Contact.Resolution;
                contact.ResolutionComment = currentTicketEntities.Contact.ResolutionComment;
                contact.SeniorComplaint = currentTicketEntities.Contact.SeniorComplaint;
                contact.HomePhone = currentTicketEntities.Contact.HomePhone;
                contact.WorkPhone = currentTicketEntities.Contact.WorkPhone;

                // Handle change tracking
                return TrackChanges(TicketEntities, currentTicketEntities);

            }
            finally
            {
                log.Debug(string.Format("End ManageChanges({0})", TicketEntities.Contact.FileNum));
            }
        }

        public Result AddOrUpdate(Ticket Ticket)
        {
            var isNew = Ticket.Id == 0;
            var result = new Result();
            result.SetOK();
            try
            {
                var ticketEntities = ManageChanges(Ticket.ToEntities());
                var maxCount = 2;

                log.Debug("Database.BeginTransaction");
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        while (result.OK && maxCount >= 0)
                        {
                            maxCount--;
                            ticketEntities.PrepareContext(context);
                            var count = SaveChanges(new Audit(ticketEntities));
                            if (count > 0)
                            {
                                if (isNew)
                                {
                                    Ticket.Id = ticketEntities.Contact.FileNum;
                                    ticketEntities = ManageChanges(Ticket.ToEntities());
                                    isNew = false;
                                }
                                else
                                {
                                    maxCount = -1;
                                    result.Id = ticketEntities.Contact.FileNum > -1
                                        ? ticketEntities.Contact.FileNum.ToString()
                                        : Ticket.Id.ToString();
                                    Ticket.Changes = ticketEntities.Changes.ToTicketChanges();
                                    log.Debug("transaction.Commit");
                                    transaction.Commit();
                                }
                            }
                            else
                                result.SetFail("Changes not saved");
                        }
                    }
                    catch (Exception e)
                    {
                        log.Debug("transaction.Rollback", e);
                        transaction.Rollback();
                        throw;
                    }
                }

            }
            catch (DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);
 
                // Join the list to a single string.
                var fullErrorMessage = string.Join("; ", errorMessages);
 
                // Combine the original exception message with the new one.
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                log.Debug(string.Format("Fail AddOrUpdate: {0}", exceptionMessage));

                // Throw a new DbEntityValidationException with the improved exception message.
                throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);

            }
            catch (Exception e)
            {
                log.Debug("Fail AddOrUpdate", e);
                result.SetFail(e);
            }
            return result;
        }

        public Result AddOrUpdateAttachment(Ticket Ticket)
        {
            return AddOrUpdate(FilterTicket(Ticket, TicketInclude.Attachment));
        }

        public Result AddOrUpdateResponseHistory(Ticket Ticket)
        {
            return AddOrUpdate(FilterTicket(Ticket, TicketInclude.Ticket, TicketInclude.ResponseHistory));
        }

        public Result AddOrUpdateResearchHistory(Ticket Ticket)
        {
            return AddOrUpdate(FilterTicket(Ticket, TicketInclude.ResearchHistory));
        }

        public Result AddOrUpdateLinkedTicket(Ticket Ticket)
        {
            return AddOrUpdate(FilterTicket(Ticket, TicketInclude.LinkedTicket));
        }

        public Result DeleteAttachment(Ticket Ticket, Attachment Attachment)
        {
            var ticket = FilterTicket(Ticket, TicketInclude.Attachment);
            ticket.Attachments = ticket.Attachments.Where(item => item.Id == Attachment.Id).ToList();
            foreach (var item in ticket.Attachments)
                item.ShouldDelete = true;
            return AddOrUpdate(ticket);
        }

        public Result DeleteLinkedTicket(Ticket Ticket, Ticket LinkedTicket)
        {
            var ticket = FilterTicket(Ticket, TicketInclude.LinkedTicket);
            ticket.LinkedTickets = ticket.LinkedTickets.Where(item => item.Id == LinkedTicket.Id).ToList();
            foreach (var item in ticket.LinkedTickets)
                item.ShouldUnlink = true;
            return AddOrUpdate(ticket);
        }

        private const string FixUpdateLogQuery =
@"UPDATE [CusRel].[dbo].[tblUpdateLog]
SET UserId=@p1
WHERE Id IN (
    SELECT up.Id
    FROM [CusRel].[dbo].[tblUpdateLog] up
    INNER JOIN (
        SELECT TOP 1 [FileNum], [DateUpdated]
        FROM [CusRel].[dbo].[tblUpdateLog]
        WHERE FileNum=@p0
            AND DateUpdated > DATEADD(minute, -1, GETDATE())
        GROUP BY FileNum, DateUpdated
        ORDER BY DateUpdated DESC
    ) q ON q.FileNum = up.FileNum AND q.DateUpdated = up.DateUpdated
)";

        public void UpdateAudit(Audit audit)
        {
            context.Database.SqlQuery<tblUpdateLog>(FixUpdateLogQuery, audit.TicketId, audit.Username);
        }

        public Result TrackView(int ticketId, string userId, DateTime? viewDate = null)
        {
            log.Debug(string.Format("Begin TrackView({0})", ticketId));

            var result = new Result();
            result.SetOK();

            try
            {
                if (!string.IsNullOrEmpty(userId))
                    context.TrackFileView(ticketId, userId, viewDate ?? DateTime.Now);
            }
            catch (Exception e)
            {
                log.Debug(string.Format("Fail TrackView({0})", ticketId), e);
                result.SetFail(e);
            }

            return result;
        }
    }
}
