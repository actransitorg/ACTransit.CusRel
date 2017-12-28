using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ACTransit.CusRel.Models;
using ACTransit.CusRel.Models.Attributes;
using ACTransit.CusRel.Security;
using ACTransit.CusRel.Models.Filters;

namespace ACTransit.CusRel.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [NoCache]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        [NoCache]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")] 
        public ActionResult Login(string ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            return View(new LoginModel { RememberMe = true });
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryTokenCustomFilter]
        public ActionResult Login(LoginModel model, string ReturnUrl)
        {
            if (model.UserName != null)
                model.UserName = model.UserName.Trim();
            if (model.Password != null)
                model.Password = model.Password.Trim();
            log.Debug(string.Format("Begin Login({0}:{1}, {2})", model.UserName, model.RememberMe, ReturnUrl));
            try
            {
                var cusRelUser = ServicesProxy.RequestState.UserDetails;
                if (cusRelUser == null)
                {
                    InitRequestState(model.UserName);
                    cusRelUser = ServicesProxy.RequestState.UserDetails;
                }

                var modelState = ModelState.IsValid;
                var isEmail = model.UserName != null && model.UserName.Contains("@");

                var loginResult = false;
                var retries = 3;

                while (retries > 0)
                    try
                    {
                        loginResult = WindowsImpersonation.Login(model.UserName, model.Password);
                        retries = -1;
                    }
                    catch (Exception e)
                    {
                        log.Error(string.Format("WindowsImpersonation.Login: {0}, Retries: {1}", e.Message, retries));
                        retries -= 1;
                    }

                log.Debug(string.Format("ModelState.IsValid: {0}, loginResult: {1}, isEmail: {2}, cusRelUser: {3}", modelState, loginResult, isEmail, cusRelUser != null ? cusRelUser.Username : null));
                if (cusRelUser == null || isEmail || !modelState || !loginResult)
                {
                    ViewBag.ReturnUrl = ReturnUrl;
                    var error =
                        cusRelUser == null || model.UserName == null
                            ? @"User not found. Please contact your.email@your.company.dns to obtain access."
                            : (isEmail
                                ? @"Please use your Windows username instead of email address."
                                : (!modelState
                                    ? @"Please enter both User Name (not email) and Password."
                                    : @"The UserName or Password provided is incorrect."));
                    log.Debug(string.Format("ModelState.IsValid Error: {0}, ReturnUrl: {1}", error, ReturnUrl));
                    ModelState.AddModelError("", error);
                    return View(model);
                }

                log.Debug("SetAuthCookie");

                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1,
                model.UserName,
                DateTime.Now,
                DateTime.Now.AddDays(180),
                model.RememberMe,
                "",
                FormsAuthentication.FormsCookiePath);
                string encTicket = FormsAuthentication.Encrypt(ticket);
                HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
                cookie.Expires = DateTime.Now.AddDays(180);
                Response.Cookies.Add(cookie);

                CustomPrincipal newUser = new CustomPrincipal(model.UserName);
                HttpContext.User = newUser;

                return Redirect(ReturnUrl ?? "../Home");

            }
            finally
            {
                log.Debug(string.Format("End Login({0}:{1}, {2})", model.UserName, model.RememberMe, ReturnUrl));
            }
        }

        
        public ActionResult Logout(string ReturnUrl = null)
        {
            log.Debug("SignOut");
            FormsAuthentication.SignOut();
            Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1)); 
            Response.Cache.SetCacheability(HttpCacheability.NoCache); 
            Response.Cache.SetNoStore();
            Session.Abandon();
            return Redirect(ReturnUrl ?? "../Account/Login");
        }


        //public ActionResult Logout()
        //{
        //    var sam = FederatedAuthentication.SessionAuthenticationModule;
        //    sam.SignOut();
        //    var fam = FederatedAuthentication.WSFederationAuthenticationModule;
        //    var signOutRequest = new SignOutRequestMessage(new Uri(fam.Issuer))
        //    {
        //        Reply = Request.Url.Scheme + "://" + Request.Url.Authority + "/"
        //    };
        //    var url = signOutRequest.WriteQueryString();
        //    return Redirect(url);
        //}
    }
}
