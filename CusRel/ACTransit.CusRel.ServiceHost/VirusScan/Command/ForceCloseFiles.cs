using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using ACTransit.CusRel.ServiceHost.Common;
using ACTransit.CusRel.ServiceHost.Contract;
using ACTransit.CusRel.ServiceHost.Request;
using ACTransit.CusRel.ServiceHost.VirusScan.Request;
using ACTransit.Entities.CustomerRelations;
using log4net;
using Newtonsoft.Json;

namespace ACTransit.CusRel.ServiceHost.VirusScan.Command
{
    public class ForceCloseFiles : VirusScanRequest
    {
        private static readonly ILog log = LogManager.GetLogger(nameof(ForceCloseFiles));

        public ForceCloseFiles(RequestState state = null) : base(state) { }

        public override void Execute()
        {
            base.Execute();
            log.Debug("Begin Execute");
            try
            {
                foreach (var item in (List<tblAttachmentsTemp>) State.Data)
                {
                    var attachment = GetAttachmentTemp(item);
                    if (attachment.ForceClose.GetValueOrDefault())
                    {
                        log.Info($"Force closing {item.FileName}");
                        item.ForceClose = false;
                        CloseAttachmentTemp(item);
                        Results.Add(new Result(item.ScanId, "Closed", "Closed", null));
                    }
                }
            }
            catch (Exception e)
            {
                log.Error(e);
                Results.Add(new Result(null, null, null, e.Message));

            }
            finally
            {
                log.Debug("End Execute");
            }
        }
    }
}

