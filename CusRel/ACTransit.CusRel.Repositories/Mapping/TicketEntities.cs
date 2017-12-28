using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Linq;
using ACTransit.Contracts.Data.CusRel.TicketContract;
using ACTransit.Contracts.Data.CusRel.UserContract;
using ACTransit.DataAccess.CustomerRelations;
using ACTransit.Entities.CustomerRelations;
using ACTransit.Framework.Extensions;
using ACTransit.Framework.Web.Helpers;

namespace ACTransit.CusRel.Repositories.Mapping
{
    public class TicketEntities
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public tblContacts Contact { get; set; }
        public List<tblAttachments> Attachments;
        public List<tblContactHistory> ResponseHistory;
        public List<tblResearchHistory> ResearchHistory;
        public List<tblIncidentUpdateHistory> IncidentUpdateHistory;
        public List<tblLinkedContacts> LinkedContacts;
        public List<tblUpdateLog> ChangeHistory;
        public ChangeTracking Changes = new ChangeTracking();

        // -------------------------------------------------------------------

        #region Static Conversions

        public static tblAttachments ToAttachment(Attachment Attachment, Ticket Ticket)
        {
            return new tblAttachments
            {
                FileNum = Ticket.Id,
                AttachmentNum = Attachment.Id,
                FileName = Attachment.Filename,
                FileSize = Attachment.IsNew && Attachment.ShouldDelete
                    ? -2  // -2 = ignore 
                    : (!Attachment.IsNew && Attachment.ShouldDelete
                        ? -1 // -1 to mark for deletion?
                        : Attachment.Data.Length),
                ContentType = MimeHelper.GetMimeType(Attachment.Filename),
                BinaryData = Attachment.Data,
                DateUploaded = Attachment.UploadedAt ?? DateTime.Now,
                Description = Attachment.Description,
                UploadedBy = Attachment.UploadedBy != null ? Attachment.UploadedBy.Id : (Ticket.UpdatedBy != null ? Ticket.UpdatedBy.Id : null)
            };
        }

        public static Attachment FromAttachment(tblAttachments Attachment)
        {
            return new Attachment
            {
                Id = Attachment.AttachmentNum,
                Filename = Attachment.FileName,
                Data = Attachment.BinaryData,
                UploadedAt = Attachment.DateUploaded,
                UploadedBy = new User(Id: Attachment.UploadedBy),
                Description = Attachment.Description
            };
        }

        // -------------------------------------------------------------------

        public static tblContactHistory ToContactHistory(ResponseHistory ResponseHistory, Ticket Ticket)
        {
            return new tblContactHistory
            {
                FileNum = Ticket.Id,
                Id = ResponseHistory.Id,
                UserId = ResponseHistory.ResponseBy.Id != null ? ResponseHistory.ResponseBy.Id.TrimEnd() : null,
                ContactDateTime = ResponseHistory.ResponseAt,
                Via = ResponseHistory.Via.ToString(),
                Comment = ResponseHistory.Comment
            };
        }

        public static ResponseHistory FromContactHistory(tblContactHistory ContactHistory, string contactStatus = null)
        {
            return new ResponseHistory
            {
                Id = ContactHistory.Id,
                ResponseBy = new User(Id: ContactHistory.UserId != null ? ContactHistory.UserId.TrimEnd() : null),
                ResponseAt = ContactHistory.ContactDateTime,
                Via = ToResponseHistoryVia(ContactHistory.Via),
                ViaAsString = !string.IsNullOrEmpty(contactStatus) ? contactStatus : ContactHistory.Via.Trim().PascalCaseToDescription(),
                Comment = ContactHistory.Comment
            };
        }

        public static ResponseHistoryVia ToResponseHistoryVia(string contactHistoryVia)
        {
            var via = contactHistoryVia.NullableTrim().EnumParse(ResponseHistoryVia.Unknown);
            if (via != ResponseHistoryVia.Unknown)
                return via;
            switch (contactHistoryVia.NullableTrim())
            {
                case "":
                    return ResponseHistoryVia.NotApplicable;
                case "None":
                    return ResponseHistoryVia.NotApplicable;
                //case "Email":
                //    return ResponseHistoryVia.SentEmail;
                //case "Letter":
                //    return ResponseHistoryVia.SentLetter;
                //case "Phone":
                //    return ResponseHistoryVia.CalledLeftMessage;
            }
            return ResponseHistoryVia.Unknown;
        }

        // -------------------------------------------------------------------

        public static tblResearchHistory ToResearchHistory(ResearchHistory ResearchHistory, Ticket Ticket)
        {
            return new tblResearchHistory
            {
                FileNum = Ticket.Id,
                Id = ResearchHistory.Id,
                UserId = ResearchHistory.ResearchedBy.Id != null ? ResearchHistory.ResearchedBy.Id.TrimEnd() : null,
                EnteredDateTime = ResearchHistory.ResearchedAt,
                Comment = ResearchHistory.Comment
            };
        }

        public static ResearchHistory FromResearchHistory(tblResearchHistory ResearchHistory)
        {
            return new ResearchHistory
            {
                Id = ResearchHistory.Id,
                ResearchedBy = new User(Id: ResearchHistory.UserId != null ? ResearchHistory.UserId.TrimEnd() : null),
                ResearchedAt = ResearchHistory.EnteredDateTime,
                Comment = ResearchHistory.Comment
            };
        }

        // -------------------------------------------------------------------

        public static tblLinkedContacts ToLinkedContact(Ticket ParentTicket, Ticket ChildTicket)
        {
            return new tblLinkedContacts
            {
                Id = ChildTicket.LinkedId ?? 0,
                FileNum = ParentTicket.Id,
                LinkedFileNum = ChildTicket.Id,
                Active = !ChildTicket.ShouldUnlink
            };
        }

        public static Ticket FromLinkedContact(tblLinkedContacts Contact)
        {
            return new Ticket
            {
                Id = Contact.LinkedFileNum,         // LinkedFileNum is child's FileNum
                ShouldUnlink = !Contact.Active,
                LinkedId = Contact.Id
            };
        }

        public static List<Ticket> FromLinkedContacts(List<tblLinkedContacts> ChildContacts, int Id)
        {
            return ChildContacts.Where(c => c.FileNum == Id).Select(FromLinkedContact).ToList();
        }

        // -------------------------------------------------------------------

        public static tblUpdateLog ToChangeHistory(ChangeHistory ChangeHistory, Ticket Ticket)
        {
            return new tblUpdateLog
            {
                UserId = ChangeHistory.ChangeBy.Id.Trim(),
                FileNum = Ticket.Id,
                TableName = ChangeHistory.TableName,
                UpdateAction = ChangeHistory.Action,
                DateUpdated = ChangeHistory.ChangeAt,
                Id = ChangeHistory.Id,
                ColumnName = ChangeHistory.ColumnName,
                OldValue = ChangeHistory.OldValue,
                NewValue = ChangeHistory.NewValue
            };
        }

        public static ChangeHistory FromChangeHistory(tblUpdateLog ChangeHistory)
        {
            return new ChangeHistory
            {
                Id = ChangeHistory.Id,
                ChangeAt = ChangeHistory.DateUpdated,
                ChangeBy = new User(Id: ChangeHistory.UserId),
                Action = ChangeHistory.UpdateAction,
                TableName = ChangeHistory.TableName,
                ColumnName = ChangeHistory.ColumnName,
                OldValue = ChangeHistory.OldValue,
                NewValue = ChangeHistory.NewValue
            };
        }

        #endregion

        // =============================================================

        #region Set Logic and Relations

        public void PrepareContext(CusRelEntities context)
        {
            log.Debug("Begin PrepareContext");
            try
            {
                log.Debug(string.Format("Contact.FileNum: {0}", Contact.FileNum)); 
                if (Contact.FileNum >= 0)
                {
                    Contact.updatedOn = DateTime.Now;
                    context.tblContacts.AddOrUpdate(Contact);
                }
                if (Contact.FileNum == 0)
                    return;
                if (Attachments != null)
                    foreach (var item in Attachments)
                    {
                        // ignore when item.FileSize is 0
                        log.Debug(string.Format("Attachment id:{0} ({1}), FileSize:{2}", item.AttachmentNum, item.FileName, item.FileSize));
                        if (item.FileSize > 0)
                        {
                            if (item.DateUploaded == null)
                                item.DateUploaded = DateTime.Now;
                            context.tblAttachments.AddOrUpdate(item);
                        }
                        else if (item.FileSize == -1)
                        {
                            context.tblAttachments.Attach(item);
                            context.tblAttachments.Remove(item);
                        }
                    }
                if (ResponseHistory != null)
                    foreach (var item in ResponseHistory)
                    {
                        // deletes not allowed
                        if (item.ContactDateTime == null)
                            item.ContactDateTime = DateTime.Now;
                        context.tblContactHistory.AddOrUpdate(item);
                    }
                if (ResearchHistory != null)
                    foreach (var item in ResearchHistory)
                    {
                        // deletes not allowed
                        if (item.EnteredDateTime == null)
                            item.EnteredDateTime = DateTime.Now;
                        context.tblResearchHistory.AddOrUpdate(item);
                    }
                if (IncidentUpdateHistory != null)
                    foreach (var item in IncidentUpdateHistory)
                    {
                        // deletes not allowed
                        context.tblIncidentUpdateHistory.AddOrUpdate(item);
                    }
                if (LinkedContacts != null)
                    foreach (var item in LinkedContacts)
                    {
                        if (item.Active)
                            context.tblLinkedContacts.AddOrUpdate(item);
                        else
                        {
                            context.tblLinkedContacts.Attach(item);
                            context.tblLinkedContacts.Remove(item);
                        }
                    }
                // ChangeHistory is read-only, do nothing
            }
            finally
            {
                log.Debug("Begin PrepareContext");
            }
        }

        public void DetachObject(CusRelEntities context, object obj)
        {
            ((IObjectContextAdapter)context).ObjectContext.Detach(obj);
        }

        public void Detach(CusRelEntities context)
        {
            DetachObject(context, Contact);
            if (Attachments != null)
                foreach (var item in Attachments)
                    DetachObject(context, item);
            if (ResponseHistory != null)
                foreach (var item in ResponseHistory)
                    DetachObject(context, item);
            if (ResearchHistory != null)
                foreach (var item in ResearchHistory)
                    DetachObject(context, item);
            if (IncidentUpdateHistory != null)
                foreach (var item in IncidentUpdateHistory)
                    DetachObject(context, item);
            if (LinkedContacts != null)
                foreach (var item in LinkedContacts)
                    DetachObject(context, item);
            if (ChangeHistory != null)
                foreach (var item in ChangeHistory)
                    DetachObject(context, item);
        }

        #endregion

        // =============================================================

        #region Change Tracking

        public class ChangeTracking
        {
            public bool ForAction;
            public bool AssignedTo;
            public bool Priority;
        }

        #endregion

        // =============================================================

    }
}
