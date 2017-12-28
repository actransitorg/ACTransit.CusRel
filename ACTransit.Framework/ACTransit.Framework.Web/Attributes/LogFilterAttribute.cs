using System.Web.Mvc;

namespace ACTransit.Framework.Web.Attributes
{
    public class LogFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            log4net.NDC.Clear();
            log4net.NDC.Push(filterContext.ActionDescriptor.ActionName + " (" + filterContext.HttpContext.Request.HttpMethod + ") ");
            base.OnActionExecuting(filterContext);
        }
    }
}