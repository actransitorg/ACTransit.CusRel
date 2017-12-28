using System.Threading;
using log4net;

namespace ACTransit.CusRel.ServiceHost.Tasks
{
    public class Cancellation
    {
        private readonly CancellationTokenSource TokenSource;
        private static readonly ILog log = LogManager.GetLogger(nameof(Cancellation));
        public CancellationToken Token;

        public Cancellation()
        {
            TokenSource = new CancellationTokenSource();
            Token = TokenSource.Token;
        }

        public bool IsCancelled => TokenSource.IsCancellationRequested;

        public void Cancel()
        {
            log.Debug("Begin TokenSource.Cancel");
            TokenSource.Cancel();
            log.Debug("End TokenSource.Cancel");
        }
    }
}
