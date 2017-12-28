using System;
using ACTransit.CusRel.ServiceHost.Common;
using ACTransit.CusRel.ServiceHost.Request;
using ACTransit.CusRel.ServiceHost.Tasks;
using ACTransit.CusRel.ServiceHost.VirusScan.Command;
using log4net;

namespace ACTransit.CusRel.ServiceHost.VirusScan.Task
{
    public class VirusScanTask : TaskBase, ITask
    {
        private static readonly ILog log = LogManager.GetLogger(nameof(VirusScanTask));

        public VirusScanTask(int runEvery, Cancellation cancellation = null) : base(runEvery, cancellation)
        {
            log.Debug("Creating VirusScanTask");
        }

        private bool shouldRerun;

        public override bool Execute()
        {
            try
            {
                log.Debug("Begin execute cycle");
                while (!IsCancelled)
                {
                    MarkExecuting();
                    try
                    {
                        throw new Exception();
                        //if (!IsCancelled)
                        //    ResultParse(RequestManager.Execute<ProcessScanReports>(requestState));

                        //if (!IsCancelled)
                        //    ResultParse(RequestManager.Execute<PostNewFiles>(requestState));

                        //if (!IsCancelled)
                        //    ResultParse(RequestManager.Execute<ForceCloseFiles>(requestState));
                    }
                    catch (System.Data.Entity.Core.EntityException e)
                    {
                        log.Error("EntityException Error, will retry.", e);
                        MarkError();
                    }
                    catch (Exception e)
                    {
                        log.Error("Exception", e);
                        MarkError();
                    }
                    MarkDone();
                    log.Debug($"Done execute cycle, duration: {LastRunSecsAgo}s");
                    if (!IsCancelled)
                        Wait();
                }
            }
            catch (Exception e)
            {
                log.Error("Execute Error", e);
                return false;
            }
            finally
            {
                log.Debug("End Execute");
            }
            return true;
        }

        private void ResultParse(RequestBase command)
        {
            shouldRerun |= command.ShouldRetry || IsError;
        }

        protected override void MarkExecuting(bool resetState = true)
        {
            base.MarkExecuting();
            requestState.LastRunDate = Config.Instance.VirusScanTaskLastRunDate;
            shouldRerun = false;
            requestState.RunDate = startRunAt;
            log.Debug($"Previous VirusScanTaskLastRunDate: {requestState.LastRunDate}");
        }

        protected override void MarkError()
        {
            base.MarkError();
            RetryCount++;
            shouldRerun = true;
        }

        protected override void MarkDone()
        {
            base.MarkDone();
            if (!shouldRerun)
                RetryCount = 0;
            log.Debug($"Should Rerun: {shouldRerun}, RetryCount: {RetryCount}");
            if (!shouldRerun)
            {
                log.Debug("Saving LastRunDate");
                Config.Instance.VirusScanTaskLastRunDate = startRunAt;
                log.Debug($"Next VirusScanTaskLastRunDate: {startRunAt}");
            }

            requestState.Data = null;
        }
    }
}
