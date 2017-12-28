using System;
using System.Web.Mvc;
using System.Web.Routing;
using ACTransit.Contracts.Data.CusRel.Common;
using ACTransit.Contracts.Data.CusRel.UserContract.Result;
using ACTransit.CusRel.Infrastructure;
using ACTransit.CusRel.Services;

namespace ACTransit.CusRel.Controllers
{
    public abstract class BaseController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected ServicesProxy ServicesProxy = new ServicesProxy
        {
            UserService = new UserService()
        };  // TODO: Create Service Locator from DI container, then move services instantiation to Factory (or Builder)

        protected virtual bool InitRequestState(string userName = null)
        {
            log.Debug("Begin BaseController.InitRequestState");
            try
            {                
                userName = userName ?? User.Identity.Name;

                var userResult = User != null && !string.IsNullOrWhiteSpace(userName)
                    ? ServicesProxy.UserService.GetUser(userName)
                    : UserResult.AsFailed();

                if (User == null)
                    log.Debug(string.Format("User is null"));
                else if (User.Identity == null)
                    log.Debug(string.Format("User.Identity is null"));
                else
                    log.Debug(string.Format("{0} UserService.GetUser({1}): {2}", userResult.OK ? "Call" : "Fail", userName, userResult));

                ServicesProxy.RequestState = new RequestState
                {
                    Principal = User,
                    Identity = User != null ? User.Identity : null,
                    UserDetails = userResult.OK ? userResult.User : null,
                    MaxSearchCount = Config.MaxSearchCount,
                };
                
                ViewBag.RequestState = ServicesProxy.RequestState;
                if (HttpContext.Items.Contains("RequestState"))
                    HttpContext.Items["RequestState"] = ServicesProxy.RequestState;
                else
                    HttpContext.Items.Add("RequestState", ServicesProxy.RequestState);

                return ServicesProxy.RequestState.UserDetails != null ;

            }
            finally
            {
                log.Debug("End BaseController.InitRequestState");
            }
        }

        /// <summary>
        /// Reload user information (uses EF 2nd level caching)
        /// </summary>
        /// <param name="context"></param>
        protected override void OnAuthorization(AuthorizationContext context)
        {
            log.Debug("Begin BaseController.OnAuthorization");
            try
            {
                if (!InitRequestState())
                {
                    if (!context.HttpContext.Request.Url.ToString().Contains("Account/Login"))
                        context.Result = new RedirectResult("~/Account/Login");
                    else
                        return;
                }
                else
                    base.OnAuthorization(context);
            }
            finally
            {
                log.Debug("End BaseController.OnAuthorization");
            }
       }

        protected void ReturnFailed(AuthorizationContext context)
        {
            context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Error", action = "AccessDenied" }));
        }

        // fix JSON serialization overrun
        protected override JsonResult Json(object data, string contentType, System.Text.Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new JsonResult()
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior,
                MaxJsonLength = Int32.MaxValue
            };
        }

        //protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
        //{
        //    return new JsonNetResult
        //    {
        //        Data = data,
        //        ContentType = contentType,
        //        ContentEncoding = contentEncoding,
        //        JsonRequestBehavior = behavior
        //    };
        //}
    }

    // =======================================================================

    // =======================================================================
}