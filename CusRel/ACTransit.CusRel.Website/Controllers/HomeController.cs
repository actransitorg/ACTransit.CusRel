using System;
using System.Web.Mvc;
using ACTransit.CusRel.Models;
using ACTransit.CusRel.Services;

namespace ACTransit.CusRel.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        protected override void OnAuthorization(AuthorizationContext context)
        {
            base.OnAuthorization(context);
            ServicesProxy.SettingsService = new SettingsService();
        }

        public ActionResult Index()
        {
            return View(new HomeModel(ServicesProxy));
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        public string TestExceptions()
        {
            var a = DateTime.Parse("asdf");
            return DateTime.Now.ToString();
        }
    }
}