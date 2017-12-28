using System.Web.Mvc;
using ACTransit.CusRel.Models;
using ACTransit.CusRel.Models.Auth;
using ACTransit.CusRel.Services;

namespace ACTransit.CusRel.Controllers
{
    [Authorize]
    [ClaimsAuthorize("UserAccess", "CanAccessAdmin")]
    public class AdminController : BaseController
    {
        protected override void OnAuthorization(AuthorizationContext context)
        {
            base.OnAuthorization(context);
            ServicesProxy.TicketService = new TicketService(ServicesProxy);
            ServicesProxy.SettingsService = new SettingsService();
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult UserAccess()
        {
            return View(new UserAccessModel(ServicesProxy));
        }

        [HttpGet]
        public ActionResult EditUser(string id)
        {
            return View(new EditUserModel(ServicesProxy, id));
        }

        [HttpPost]
        public ActionResult EditUser(EditUserModel model)
        {
            var saveResult = ServicesProxy.UserService.SaveUser(model.User);
            return RedirectToAction("UserAccess", "Admin");
        }

        [HttpPost]
        public ActionResult DeleteUser(EditUserModel model)
        {
            var saveResult = ServicesProxy.UserService.DeleteUser(model.User.Id);
            return RedirectToAction("UserAccess", "Admin");
        }

        [HttpGet]
        public ActionResult GroupContacts()
        {
            return View(new GroupContactModel(ServicesProxy));
        }

        [HttpGet]
        public ActionResult EditGroupContact(string id)
        {
            return View(new EditGroupContactModel(ServicesProxy, id));
        }

        [HttpPost]
        public ActionResult EditGroupContact(EditGroupContactModel model)
        {
            model.GroupContact.IsVisible = model.IsVisible; // hack due to Nullable<bool> not being supported by CheckBoxFor()
            var saveResult = ServicesProxy.TicketService.UpdateGroupContact(model.GroupContact);
            return RedirectToAction("GroupContacts", "Admin");
        }

        [HttpGet]
        public ActionResult SiteConfiguration()
        {
            return View(new SettingsModel(ServicesProxy));
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult SiteConfiguration(SettingsModel model)
        {
            ServicesProxy.SettingsService.SaveSettings(model.Settings);
            return RedirectToAction("SiteConfiguration", "Admin");
        }
    }
}