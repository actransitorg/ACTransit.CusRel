using System;
using System.Threading.Tasks;
using ACTransit.CusRel.ServiceHost.Common;
using ACTransit.CusRel.ServiceHost.Request;
using log4net;

namespace ACTransit.CusRel.ServiceHost.Tasks
{
    public interface ITask
    {
        bool Execute();
    }

    public class TaskBase : ITask
    {
        protected readonly Cancellation cancellation;
        private static readonly ILog log = LogManager.GetLogger(nameof(TaskBase));

        private readonly int runEverySecs;
        protected DateTime firstRunAt { get; private set; }
        protected int runCount { get; private set; }
        protected DateTime startRunAt = DateTime.MinValue;
        protected RequestState requestState;

        public TaskBase() { }

        public TaskBase(Cancellation cancellation = null)
        {
            this.cancellation = cancellation;
            Init();
        }

        public TaskBase(int runEverySecs, Cancellation cancellation = null)
        {
            log.Debug("Creating TaskBase");
            this.runEverySecs = runEverySecs;
            this.cancellation = cancellation;
            Init();
        }

        private void Init()
        {
            firstRunAt = DateTime.Now;
            runCount = 0;
        }

        protected bool IsCancelled => cancellation != null && cancellation.IsCancelled;

        public TimeSpan? DelayFor
        {
            get
            {
                var delay = runEverySecs <= 0
                    ? (TimeSpan?)null
                    : firstRunAt.AddSeconds((runCount * runEverySecs)).Subtract(DateTime.Now);
                var delaySec = delay.HasValue ? Math.Max(delay.Value.TotalSeconds, 0) : 0;
                log.Debug($"LastRunDuration: {LastRunSecsAgo}s, DelayFor: {delaySec}s");
                return delay;
            }
        }

        public double LastRunSecsAgo => (DateTime.Now - startRunAt).TotalSeconds;

        public virtual bool Execute()
        {
            return false;
        }

        protected virtual void MarkExecuting(bool resetState = true)
        {
            if (resetState)
                requestState = new RequestState();
            startRunAt = DateTime.Now;
            runCount += 1;
            IsError = false;
        }

        protected virtual void MarkDone()
        {
        }

        protected bool IsError;

        protected virtual void MarkError()
        {
            IsError = true;
        }

        private int retryCount;

        public int RetryCount
        {
            get { return retryCount; }
            set
            {
                retryCount = value;
                if (retryCount != Config.Instance.EmailTooManyRetriesCount) return;
                var email = new EmailError();
                email.GenerateRetryEmail(requestState.Client);
            }
        }

        protected void Wait()
        {
            log.Debug("Begin Wait");
            if (runEverySecs <= 0) return;
            var delay = DelayFor;
            var waitTask = Task.Delay(delay.GetValueOrDefault(), cancellation.Token);
            if (!delay.HasValue || delay.Value.TotalMilliseconds <= 0)
            {
                log.Debug("No Wait");
                return;
            }
            waitTask.Wait(cancellation.Token);
            log.Debug("End Wait");
        }
    }
}
