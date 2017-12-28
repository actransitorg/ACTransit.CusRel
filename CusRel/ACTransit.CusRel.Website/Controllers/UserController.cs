using System.Web.Mvc;

namespace ACTransit.CusRel.Controllers
{
    [Authorize]
    public class UserController : BaseController
    {
        [HttpGet]
        public JsonResult GetOperator(string id)
        {
            return Json(ServicesProxy.UserService.GetOperator(id), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetUsers()
        {
            return Json(ServicesProxy.UserService.GetUsers(), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetEmployees(string badge, string lastName, string firstName)
        {
            return Json(ServicesProxy.UserService.GetEmployees(badge, lastName, firstName), JsonRequestBehavior.AllowGet);
        }

    }
}