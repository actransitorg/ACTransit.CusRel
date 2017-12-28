using System.Web.Mvc;
using ACTransit.Contracts.Data.CusRel.ReportContract.Params;
using ACTransit.Contracts.Data.CusRel.UserContract;
using ACTransit.CusRel.Models;
using ACTransit.CusRel.Models.Auth;
using ACTransit.CusRel.Services;

namespace ACTransit.CusRel.Controllers
{
    [Authorize]
    public class ReportController : BaseController
    {
        protected override void OnAuthorization(AuthorizationContext context)
        {
            base.OnAuthorization(context);
            ServicesProxy.TicketService = new TicketService(ServicesProxy);
            ServicesProxy.ReportService = new ReportService(ServicesProxy);
            ServicesProxy.MapsScheduleService = new MapsScheduleService(ServicesProxy);
        }

        [HttpGet]
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult AssignedTo()
        {
            return View(new AssignedToReportModel(ServicesProxy));
        }

        [HttpPost]
        public JsonResult AssignedTo(AssignedToReportParams ReportParams)
        {
            return Json(ServicesProxy.ReportService.AssignedToReport(ReportParams), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ForAction(string id)
        {
            return View(new ForActionReportModel(ServicesProxy, id));
        }

        [HttpPost]
        public JsonResult ForAction(ForActionReportParams ReportParams)
        {
            return Json(ServicesProxy.ReportService.ForActionReport(ReportParams), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [ClaimsAuthorize("UserAccess", "CanCloseTicket")]
        public ActionResult ReadyToClose()
        {
            return View(new ReadyToCloseReportModel(ServicesProxy));
        }

        [HttpPost]
        [ClaimsAuthorize("UserAccess", "CanCloseTicket")]
        public JsonResult ReadyToClose(ReadyToCloseReportParams ReadyToCloseReportParams)
        {
            return Json(ServicesProxy.TicketService.Close(ReadyToCloseReportParams), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult LostFound()
        {
            ServicesProxy.SettingsService = new SettingsService();
            return View(new LostFoundReportModel(ServicesProxy));
        }

        [HttpPost]
        public JsonResult LostFound(LostFoundReportParams LostFoundReportParams)
        {
            ServicesProxy.SettingsService = new SettingsService();
            return Json(ServicesProxy.ReportService.LostFoundReport(LostFoundReportParams), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Rejected()
        {
            return View(new RejectedReportModel(ServicesProxy));
        }

        [HttpPost]
        public JsonResult Rejected(ReportParams RejectedReportParams)
        {
            return Json(ServicesProxy.ReportService.RejectedReport(RejectedReportParams), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult OpenTickets()
        {
            return View(new OpenTicketsReportModel(ServicesProxy));
        }

        [HttpPost]
        public JsonResult OpenTickets(ReportParams OpenTicketsReportParams)
        {
            return Json(ServicesProxy.ReportService.OpenTicketsReport(OpenTicketsReportParams), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult OpenTicketsStatus()
        {
            return View(new OpenTicketsStatusReportModel(ServicesProxy));
        }

        [HttpPost]
        public JsonResult OpenTicketsStatus(OpenTicketStatusReportParams OpenTicketStatusReportParams)
        {
            return Json(ServicesProxy.ReportService.OpenTicketsStatusReport(OpenTicketStatusReportParams), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Productivity()
        {
            return View(new ProductivityReportModel(ServicesProxy));
        }

        [HttpPost]
        public JsonResult Productivity(ReportParams ReportParams)
        {
            return Json(ServicesProxy.ReportService.ProductivityReport(ReportParams), JsonRequestBehavior.AllowGet);
        }
    }
}