using System.Threading;
using System.Threading.Tasks;
using ACTransit.CusRel.ServiceHost.Common;
using ACTransit.CusRel.ServiceHost.VirusScan.Task;
using log4net;

namespace ACTransit.CusRel.ServiceHost.Tasks
{
    public class TaskManager
    {
        private readonly Cancellation cancellation;
        private static readonly ILog log = LogManager.GetLogger(nameof(TaskManager));
        private Task task;
        private readonly WaitOnStart waitOnStart;

        public bool IsDone => task.IsCompleted;

        public TaskManager(Cancellation cancellation = null, WaitOnStart waitOnStart = WaitOnStart.Continue)
        {
            log.Debug("Creating TaskManager");
            this.cancellation = cancellation ?? new Cancellation();
            this.waitOnStart = waitOnStart;
        }

        public void OnStart()
        {
            log.Debug("Begin OnStart");
            task = Task.Factory.StartNew(() =>
            {
                log.Info($"Creating VirusScanTask({Config.Instance.VirusScanTaskRunEverySecs})");
                var vTask = new VirusScanTask(Config.Instance.VirusScanTaskRunEverySecs, cancellation);
                if (!vTask.Execute() || cancellation.Token.IsCancellationRequested)
                {
                    // clean up here, if needed
                    log.Info("VirusScanTask.Executing Completing or Cancelling");
                }
                //cancellation.Token.ThrowIfCancellationRequested();
            }, cancellation.Token);
            log.Debug("Begin task.Wait");
            if (waitOnStart == WaitOnStart.Wait)
                task.Wait();
            else
                Thread.Sleep(1000);
            log.Debug("End OnStart");
        }

        public void OnStop()
        {
            log.Debug("Begin OnStop");
            cancellation.Cancel();
            log.Debug("End OnStop");
        }
    }

    public enum WaitOnStart
    {
        Wait,
        Continue
    }
}
