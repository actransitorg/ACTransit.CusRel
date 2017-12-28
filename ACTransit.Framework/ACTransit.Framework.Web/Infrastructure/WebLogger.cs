using System;
using System.Web;

namespace ACTransit.Framework.Web.Infrastructure
{
    public class WebLogger
    {
        private readonly Logging.Logger _logger;

        public WebLogger(Type type)
        {
            _logger = new Logging.Logger(type);
        }

        public void Debug(string message)
        {
            try
            {
                _logger.WriteDebug(FormatMessage(message));
            }
            catch { }
        }

        public void Info(string message)
        {
            try
            {
                _logger.Write(FormatMessage(message));
            }
            catch(Exception ex) { }
        }

        public void Error(string message)
        {
            try
            {
                _logger.WriteError(FormatMessage(message));
            }
            catch (Exception ex) { }
        }

        public void Fatal(string message)
        {
            try
            {
                _logger.WriteFatal(FormatMessage(message));
            }
            catch (Exception ex) { }
        }


        public void Fatal(string message, Exception ex)
        {
            try
            {
                _logger.WriteFatal(FormatMessage(message), ex);
            }
            catch { }
        }

        private string FormatMessage(string message)
        {
            if (HttpContext.Current == null || HttpContext.Current.User == null) 
                return message;

            var user = HttpContext.Current.User;
            var request = HttpContext.Current.Request;

            var userName = string.Empty;
            if (user != null && user.Identity != null)
                userName = user.Identity.Name;
            var iP = request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrWhiteSpace(iP))
                iP = request.UserHostAddress;
            if (string.IsNullOrWhiteSpace(iP))
                iP = request.ServerVariables["REMOTE_ADDR"];
            if (string.IsNullOrWhiteSpace(iP))
                iP = "-";
            return string.Format("{0}-{1}-{2}", userName, iP, message);
        }
    }
}
