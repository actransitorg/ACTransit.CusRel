using System.Collections.Generic;
using System.Linq;
using ACTransit.CusRel.ServiceHost.Common;
using ACTransit.CusRel.ServiceHost.Contract;

namespace ACTransit.CusRel.ServiceHost.Request
{
    public abstract class RequestBase
    {
        public RequestState State { get; private set; }

        private List<Result> results;
        public List<Result> Results
        {
            get { return results ?? (results = new List<Result>()); }
            protected set { results = value; }
        }

        public bool ShouldRetry
        {
            get
            {
                return Results.Any() && Results.Any(r => r.Code != null);
            }
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
                email.GenerateRetryEmail(State.Client);
            }
        }

        protected RequestBase(RequestState state = null)
        {
            State = state;
        }

        public virtual void Execute() { }
    }
}
