using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using ACTransit.CusRel.ServiceHost.Common;
using ACTransit.CusRel.ServiceHost.Request;
using ACTransit.CusRel.ServiceHost.VirusScan.Command;
using ACTransit.DataAccess.CustomerRelations;
using ACTransit.Entities.CustomerRelations;
using log4net;
using Omu.ValueInjecter;
using Omu.ValueInjecter.Injections;

namespace ACTransit.CusRel.ServiceHost.VirusScan.Request
{
    public class VirusScanRequest : RequestBase
    {
        private static readonly ILog log = LogManager.GetLogger(nameof(VirusScanRequest));
        private CusRelEntities cusRelContext;

        protected VirusScanRequest(RequestState state = null) : base(state)
        {
            PrepareData();
        }

        public void PrepareData()
        {
            if (this is PostNewFiles)
            {
                var files = NewFiles();
                log.Debug( $"{files.Count} new file(s) found");
                State.Data = files;
            }                
            else if (this is ProcessScanReports)
            {
                var files = ProcessingFiles();
                log.Debug($"{files.Count} processing file(s) found");
                State.Data = files;
            }
            else if (this is ForceCloseFiles)
            {
                var files = ForceCloseFiles();
                log.Debug($"{files.Count} files to force close found");
                State.Data = files;
            }
        }

        private List<tblAttachmentsTemp> NewFiles()
        {
            using (cusRelContext = new CusRelEntities())
            {
                return (from f in cusRelContext.tblAttachmentsTemp
                    where f.ScanId == null
                    select f).ToList().Select(cleanData).ToList();
            }
        }

        private List<tblAttachmentsTemp> ProcessingFiles()
        {
            using (cusRelContext = new CusRelEntities())
            {
                return (from f in cusRelContext.tblAttachmentsTemp
                        where f.ScanId != null && f.ScanResult == null
                        select f).ToList().Select(cleanData).ToList();
            }
        }

        private List<tblAttachmentsTemp> ForceCloseFiles()
        {
            using (cusRelContext = new CusRelEntities())
            {
                return (from f in cusRelContext.tblAttachmentsTemp
                        where f.ScanId != null && f.ForceClose != null && f.ForceClose.Value
                        select f).ToList().Select(cleanData).ToList();
            }
        }

        protected tblAttachmentsTemp GetAttachmentTemp(tblAttachmentsTemp attachmentTemp)
        {
            using (cusRelContext = new CusRelEntities())
            {
                var attachment = 
                    (from a in cusRelContext.tblAttachmentsTemp
                    where (attachmentTemp.Id > 0 && a.Id == attachmentTemp.Id)
                        || (attachmentTemp.Id <= 0 && (attachmentTemp.AttachmentNum > 0 && a.AttachmentNum == attachmentTemp.AttachmentNum)
                            && (attachmentTemp.FileName != "" && a.FileName == attachmentTemp.FileName)
                            && (attachmentTemp.ScanId != "" && a.ScanId == attachmentTemp.ScanId))
                     select a).FirstOrDefault();
                return cleanData(attachment);
            }
        }

        protected bool UpdateAttachmentTemp(tblAttachmentsTemp attachment)
        {
            try
            {
                using (cusRelContext = new CusRelEntities())
                {
                    cusRelContext.tblAttachmentsTemp.Attach(attachment);
                    cusRelContext.Entry(attachment).State = EntityState.Modified;
                    return cusRelContext.SaveChanges() == 1;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

        protected bool AllowAttachment(tblAttachmentsTemp attachment)
        {
            try
            {
                using (cusRelContext = new CusRelEntities())
                {
                    var newAttachment = new tblAttachments();
                    newAttachment.InjectFrom(new LoopInjection(new[] { "tblContacts" }), attachment);
                    newAttachment.BinaryData = Convert.FromBase64String(attachment.Base64Data);
                    cusRelContext.tblAttachments.Attach(newAttachment);
                    cusRelContext.Entry(newAttachment).State = EntityState.Added;
                    return cusRelContext.SaveChanges() == 1;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

        protected bool CloseAttachmentTemp(tblAttachmentsTemp attachment)
        {
            try
            {
                using (cusRelContext = new CusRelEntities())
                {
                    attachment.Base64Data = null;
                    cusRelContext.tblAttachmentsTemp.Attach(attachment);
                    cusRelContext.Entry(attachment).State = EntityState.Modified;
                    return cusRelContext.SaveChanges() == 1;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }


        protected bool DeleteAttachmentTemp(tblAttachmentsTemp attachment)
        {
            try
            {
                using (cusRelContext = new CusRelEntities())
                {
                    cusRelContext.tblAttachmentsTemp.Attach(attachment);
                    cusRelContext.tblAttachmentsTemp.Remove(attachment);
                    return cusRelContext.SaveChanges() == 1;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private static readonly Regex base64re = new Regex("data:(.*?);base64,(.*)", RegexOptions.Compiled);

        private tblAttachmentsTemp cleanData(tblAttachmentsTemp attachment)
        {
            if (attachment.Base64Data != null)
            {
                var match = base64re.Match(attachment.Base64Data);
                if (match.Success)
                    attachment.Base64Data = match.Groups[2].Value;
            }
            return attachment;
        }

    }
}
