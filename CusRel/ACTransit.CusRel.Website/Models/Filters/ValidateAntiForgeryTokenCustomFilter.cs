using System;
using System.Web.Mvc;

namespace ACTransit.CusRel.Models.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ValidateAntiForgeryTokenCustomFilter : FilterAttribute, IAuthorizationFilter
    {
        private readonly ValidateAntiForgeryTokenAttribute _validator;
        private log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ValidateAntiForgeryTokenCustomFilter()
        {
            _validator = new ValidateAntiForgeryTokenAttribute();
        }

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            try
            { 
                _validator.OnAuthorization(filterContext);
            }
            catch (Exception ex)
            {
                //Ignore error when validating token if it is originated from another session opened from the same user
                //Refer to http://stackoverflow.com/questions/14970102/anti-forgery-token-is-meant-for-user-but-the-current-user-is-username for more information about the error
                //Please note that neither decorating the method in the controller with the cache attribute or using AntiForgeryConfig.SuppressIdentityHeuristicChecks = true work
                if (!ex.Message.Contains("The provided anti-forgery token was meant for user \"\""))
                    throw;
                else
                    log.Warn(ex.ToString());
            }

        }
    }
}