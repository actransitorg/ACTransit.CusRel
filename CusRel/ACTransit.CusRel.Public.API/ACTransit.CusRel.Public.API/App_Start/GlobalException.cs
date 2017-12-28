using System.Diagnostics.Contracts;
using System.Net;
using System.Net.Http;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using log4net;

namespace ACTransit.CusRel.Public.API
{
    public class GlobalExceptionLogger : IExceptionLogger
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public virtual Task LogAsync(ExceptionLoggerContext context, CancellationToken cancellationToken)
        {
            return !ShouldLog(context) ? Task.FromResult(0) : LogAsyncCore(context, cancellationToken);
        }

        public virtual Task LogAsyncCore(ExceptionLoggerContext context, CancellationToken cancellationToken)
        {
            Task.Factory.StartNew(() => LogCore(context), cancellationToken);   
            return Task.FromResult(0);
        }

        public virtual void LogCore(ExceptionLoggerContext context)
        {
            if (context == null)
            {
                Log.Error("ExceptionLoggerContext is null");
            }
            Log.Error(context.ExceptionContext.Exception);
        }

        public virtual bool ShouldLog(ExceptionLoggerContext context)
        {
            if (context == null)
            {
                Log.Error("ExceptionLoggerContext is null");
            }
            return context.ExceptionContext.CatchBlock.IsTopLevel;
        }
    }

    // =======================================================================

    public class GlobalExceptionHandler : ExceptionHandler
    {
        public virtual void HandleCore(ExceptionHandlerContext context)
        {
            context.Result = new ExceptionResponse
            {
                statusCode = context.Exception is SecurityException ? HttpStatusCode.Unauthorized : HttpStatusCode.InternalServerError,
                message = "An internal exception occurred.",
                request = context.Request
            };
        }
    }

    // =======================================================================

    public class ExceptionResponse : IHttpActionResult
    {
        public HttpStatusCode statusCode { get; set; }
        public string message { get; set; }
        public HttpRequestMessage request { get; set; }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(statusCode)
            {
                RequestMessage = request,
                Content = new StringContent(message)
            };
            return Task.FromResult(response);
        }
    }
}