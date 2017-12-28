using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;
using ACTransit.Framework.Exceptions;
using ACTransit.Framework.Web.Infrastructure;
using log4net;

namespace ACTransit.Framework.Web.Attributes
{
    /// <summary>
    /// Exception filter for ASP.Net WebAPI
    /// </summary>
    public class CustomExceptionAttribute: ExceptionFilterAttribute 
    {
        private readonly ILog _logger;
        public CustomExceptionAttribute()
        {
            _logger = LogManager.GetLogger(GetType());
        }

        public override async void OnException(HttpActionExecutedContext filterContext)
        {
            if (filterContext.Exception == null)
                return;


            var currentException = filterContext.Exception;
            IEnumerable<string> headerValues;
            bool isAjaxCall = false;
            if (filterContext.Request.Headers.TryGetValues("X-Requested-With", out headerValues))            
                isAjaxCall = headerValues.FirstOrDefault() == "XMLHttpRequest";

            //if (filterContext.ActionContext.RequestContext.IncludeErrorDetail)
            //    return;

            // HTTPErrorCode doesn't matter when it is ajax call, we should handle it here.
            if (!isAjaxCall &&
                !(currentException is FriendlyException) &&
                new HttpException(null, currentException).GetHttpCode() != 500)
            {
                return;
            }


            WebMailer.EmailError(currentException);

            string m = "There was an error processing your request. Please try again in a few moments.";
            var exception = currentException as FriendlyException;
            if (exception != null)
                m = exception.Message;


            // if the request is AJAX return JSON else view.
          

            // log the error using log4net.
            StringBuilder message = new StringBuilder();
            message.AppendLine(filterContext.Request.RequestUri.ToString());            
            using (var headers = filterContext.Request.Headers.GetEnumerator())
            {
                while (headers.MoveNext())
                {
                    message.Append(headers.Current.Key).Append(":").AppendLine(headers.Current.Value?.ToString());
                }                
            }
            message.AppendLine(currentException.Message);
            message.AppendLine("content if any and less than 1K:");
            try
            {                
                if (filterContext.Request?.Content?.Headers?.ContentLength < 1024)
                {
                    if (filterContext?.ActionContext?.ActionArguments?.Count > 0)
                    {
                        foreach (var arg in filterContext?.ActionContext?.ActionArguments)
                        {
                            message.Append(arg.Key).Append(":").AppendLine(new JavaScriptSerializer().Serialize(arg.Value));
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                message.Append("Error trying to serialize the content, ").AppendLine(ex.Message);
            }
            
            _logger.Error(message, currentException);

            filterContext.Response = filterContext.Request.CreateResponse(HttpStatusCode.InternalServerError, new
            {
                error = true,
                message = m, //currentException.Message,
                urlReferrer = filterContext.Request.RequestUri
            });

        }
    }
}
