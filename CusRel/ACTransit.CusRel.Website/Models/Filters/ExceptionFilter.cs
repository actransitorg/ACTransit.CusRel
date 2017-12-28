using System;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ACTransit.Framework.Web.Infrastructure;

namespace ACTransit.CusRel.Models.Filters
{
    public class ExceptionFilter : IExceptionFilter
    {
        private log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void OnException(ExceptionContext filterContext)
        {
            var exception = filterContext.Exception;
            if (exception == null) return;
            //_logger.Fatal(exception.Message, exception);

            //Send an email if the error is anything other than Not Found.
            if (exception is HttpException && ((HttpException) exception).GetHttpCode() == (int) HttpStatusCode.NotFound)
                return;
            log.Error(exception.ToString());
            try
            {
                WebMailer.EmailError(exception);
            }
            catch (Exception ex)
            {
                log.Error(ex.ToString());
            }

        }
    }
}