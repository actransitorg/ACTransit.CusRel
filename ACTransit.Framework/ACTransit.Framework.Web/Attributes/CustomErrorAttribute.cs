using System;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ACTransit.Framework.Web.Exceptions;
using ACTransit.Framework.Web.Infrastructure;
using log4net;

namespace ACTransit.Framework.Web.Attributes
{
    public class CustomErrorAttribute : HandleErrorAttribute
    {
        private readonly ILog _logger;
        public CustomErrorAttribute()
        {
            _logger = LogManager.GetLogger(GetType());
        }

        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.Exception == null)
                return;

            var currentException = filterContext.Exception;
            bool isAjaxCall = filterContext.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            if (filterContext.ExceptionHandled || !filterContext.HttpContext.IsCustomErrorEnabled)
            {
                return;
            }

            // HTTPErrorCode doesn't matter when it is ajax call, we should handle it here.
            if (!isAjaxCall &&
                !(currentException is FriendlyException) &&
                !currentException.GetType().IsAssignableFrom(typeof(FriendlyException)) &&
                !currentException.GetType().IsAssignableFrom(typeof(Framework.Exceptions.FriendlyException)) &&
                new HttpException(null, currentException).GetHttpCode() != 500)
            {
                return;
            }

            if (!ExceptionType.IsInstanceOfType(currentException))
            {
                return;
            }

            //Send an email if the error is anything other than Not Found.
            if (filterContext.HttpContext.Response.StatusCode != (int)HttpStatusCode.NotFound)
                WebMailer.EmailError(currentException);

            // if the request is AJAX return JSON else view.
            if (isAjaxCall)
            {
                string m = "There was an error processing your request. Please try again in a few moments.";                
                Exception exception = currentException as FriendlyException;
                if (exception != null)
                    m = exception.Message;
                else
                {
                    exception = currentException as Framework.Exceptions.FriendlyException;
                    if (exception != null)
                        m = exception.Message;
                }
              
                filterContext.Result = new JsonResult
                {
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    Data = new
                    {
                        error = true,
                        message = m, //currentException.Message,
                        urlReferrer = filterContext.HttpContext.Request.UrlReferrer == null ? "" : filterContext.HttpContext.Request.UrlReferrer.OriginalString
                    }
                };
            }
            else
            {
                var controllerName = (string)filterContext.RouteData.Values["controller"];
                var actionName = (string)filterContext.RouteData.Values["action"];
                var model = new HandleErrorInfo(currentException, controllerName, actionName);

                filterContext.Result = new ViewResult
                {
                    ViewName = View,
                    MasterName = Master,
                    ViewData = new ViewDataDictionary<HandleErrorInfo>(model),
                    TempData = filterContext.Controller.TempData
                };

                string messaage = "";
                Exception exception = currentException as FriendlyException;
                if (exception != null)
                    messaage = exception.Message;
                else
                {
                    exception = currentException as Framework.Exceptions.FriendlyException;
                    if (exception != null)
                        messaage = exception.Message;
                }

                if (!string.IsNullOrWhiteSpace(messaage))
                {
                    if (((ViewResult)filterContext.Result).TempData.ContainsKey("ErrorMessage"))
                        ((ViewResult)filterContext.Result).TempData["ErrorMessage"] = messaage;
                    else
                        ((ViewResult)filterContext.Result).TempData.Add("ErrorMessage", messaage);
                }

            }

            // log the error using log4net.
            string message = filterContext.HttpContext.Request.UrlReferrer == null
                ? ""
                : filterContext.HttpContext.Request.UrlReferrer.OriginalString + "\r\n";
            message += currentException.Message;
            _logger.Error(message, currentException);

            filterContext.ExceptionHandled = true;
            filterContext.HttpContext.Response.Clear();
            filterContext.HttpContext.Response.StatusCode = 500;

            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
        }

    }
}