using System.Linq;
using System.Web.Mvc;
using ACTransit.Contracts.Data.CusRel.Common;
using ACTransit.Contracts.Data.CusRel.TicketContract;
using ACTransit.Contracts.Data.CusRel.TicketContract.Params;
using ACTransit.CusRel.Infrastructure;
using ACTransit.CusRel.Models;
using ACTransit.CusRel.Services;
using System;

namespace ACTransit.CusRel.Controllers
{
    [Authorize]
    public class TicketController : BaseController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected override void OnAuthorization(AuthorizationContext context)
        {
            log.Debug("Begin TicketController.OnAuthorization");
            try
            {
                base.OnAuthorization(context);
                ServicesProxy.TicketService = new TicketService(ServicesProxy);
                ServicesProxy.SettingsService = new SettingsService();
                ServicesProxy.MapsScheduleService = new MapsScheduleService(ServicesProxy);
                ServicesProxy.EmailService = new EmailService();
            }
            finally
            {
                log.Debug("End TicketController.OnAuthorization");
            }
        }

        #region Public Methods

        // GET: Ticket
        public ActionResult Index()
        {
            return RedirectToAction("Search");
        }

        public ViewResult GetComplaintCodes()
        {
            return View(ServicesProxy.TicketService.GetComplaintCodes());
        }

        public JsonResult GetGroupContacts()
        {
            return Json(ServicesProxy.TicketService.GetGroupContacts(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNew()
        {
            return Json(ServicesProxy.TicketService.GetEmptyTicket(Ticket.EmptyTicketEnum.NewTicket), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEmpty()
        {
            return Json(ServicesProxy.TicketService.GetEmptyTicket(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAddressStaticLists()
        {
            return Json(ServicesProxy.TicketService.GetAddressStaticLists(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult Get(int Id)
        {
            log.Debug(string.Format("Begin Get({0})", Id));
            try
            {
                var result = ServicesProxy.TicketService.Get(Id);
                log.Debug(string.Format("Call TicketService.Get({0}): {1}", Id, result));
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                log.Debug(string.Format("End Get({0})", Id));
            } 
        }

        public JsonResult GetPrevious(int Id)
        {
            return Json(ServicesProxy.TicketService.GetPrevious(Id), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNext(int Id)
        {
            return Json(ServicesProxy.TicketService.GetNext(Id), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLast()
        {
            return Json(ServicesProxy.TicketService.GetLast(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRouteInfo(string id, DateTime? incidentDateTime = null)
        {
            var result = TicketModel.GetRouteInfo(id, incidentDateTime, ServicesProxy);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Search()
        {
            log.Debug("Begin Search()");
            try
            {
                var result = new TicketSearchModel(ServicesProxy);
                log.Debug(string.Format("{0} new TicketSearchModel: {1}", result.OK ? "Call" : "Fail", result.Errors != null && result.Errors.Count > 0 ? result.Errors.Aggregate((i,j) => i + "." + j) : null));
                return View(result);
            }
            finally
            {
                log.Debug("End Search()");
            }            
        }

        [HttpPost]
        public JsonResult Search(TicketSearchParams Criteria)
        {
            log.Debug("Begin Search");
            try
            {
                var result = ServicesProxy.TicketService.Search(Criteria);
                log.Debug(string.Format("{0} TicketService.Search: {1}", result.OK ? "Call" : "Fail", result.Errors != null && result.Errors.Count > 0 ? result.Errors.Aggregate((i, j) => i + "." + j) : null));
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                log.Debug("End Search");
            }
        }

        public JsonResult UpdateTicket(Ticket Ticket)
        {
            log.Debug(string.Format("Begin UpdateTicket({0})", Ticket != null ? Ticket.Id : -1));
            try
            {
                var result = ServicesProxy.TicketService.Update(Ticket);
                log.Debug(string.Format("{0} TicketService.Update: {1}", result.OK ? "Call" : "Fail", result));
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                log.Debug(string.Format("End UpdateTicket({0})", Ticket != null ? Ticket.Id : -1));
            }
        }

        [HttpGet]
        public ActionResult Create()
        {
            log.Debug("Begin Create()");
            try
            {
                ViewBag.Title = "Create";
                ViewBag.Breadcrumb = "Tickets > Create";
                var model = new TicketModel(ServicesProxy);
                return View("Details", model);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw;
            }
            finally
            {
                log.Debug("End Create()");
            }
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Create(Ticket ticket)
        {
            log.Debug(string.Format("Begin Create(ticket:{0})", ticket.Id));
            try
            {
                ViewBag.Title = "Create";
                ViewBag.Breadcrumb = "Tickets > Create";

                var model = new TicketModel(ServicesProxy, ticket, ModelState);

                if (!model.IsValid)
                {
                    log.Debug(string.Format("Ticket Model is Invalid"));
                    return View("Details", model);
                }

                var result = ServicesProxy.TicketService.Update(ticket, Ticket.TicketAction.Create);
                log.Debug(string.Format("{0} TicketService.Update: {1}", result.OK ? "Call" : "Fail", result));

                if (!result.OK)
                    return View("Details", model);

                return RedirectToAction("Update", "Ticket", new {id = ticket.Id});
            }
            finally
            {
                log.Debug(string.Format("End Create(ticket:{0})", ticket.Id));
            }
        }

        [HttpGet]
        public ActionResult Update(int? id = null)
        {
            log.Debug(string.Format("Begin Update({0})", id));
            try
            {
                if (id == null)
                {
                    log.Debug("Id is null");
                    return RedirectToAction("Index");
                }
                var model = CreateTicketModel(id.Value);
                log.Debug(string.Format("{0} CreateTicketModel: {1}", model.OK ? "Call" : "Fail", model));

                if (!model.OK)
                    return RedirectToAction("Index");
                    
                ViewBag.Title = id + " - Update";
                ViewBag.Breadcrumb = "Tickets > Update";
                return View("Details", model);
            }
            finally
            {
                log.Debug(string.Format("End Update({0})", id));
            }
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Update(Ticket ticket, bool isNew = false)
        {
            log.Debug(string.Format("Begin Update({0}, {1})", ticket.Id, isNew));
            try
            {
                ViewBag.Title = ticket.Id + " - Update";
                ViewBag.Breadcrumb = "Tickets > Update";
                var model = new TicketModel(ServicesProxy, ticket, ModelState);
                if (!model.IsValid)
                {
                    log.Debug(string.Format("Ticket Model is Invalid"));                    
                    return View("Details", model);
                }
                if (!isNew) ticket.Attachments = null;

                var result = ServicesProxy.TicketService.Update(ticket);
                log.Debug(string.Format("{0} TicketService.Update: {1}", result.OK ? "Call" : "Fail", result));

                model = CreateTicketModel(ticket.Id);
                model.MergeResults(result);

                log.Debug(string.Format("{0} CreateTicketModel: {1}", model.OK ? "Call" : "Fail", model));

                return View("Details", model);
            }
            finally
            {
                log.Debug(string.Format("End Update({0}, {1})", ticket.Id, isNew));
            }
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult AddResearchHistory(int id, DateTime? dateResearchHistory, string commentResearchHistory)
        {
            return Json(ServicesProxy.TicketService.AddResearchHistory(id, dateResearchHistory, commentResearchHistory), JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult AddResponseHistory(int id, DateTime? dateResponseHistory, string commentResponseHistory, ResponseHistoryVia? ViaResponseHistory, bool? sendAsEmail, string emailCc, string emailBcc)
        {
            return Json(ServicesProxy.TicketService.AddResponseHistory(
                new TicketService.AddResponseDetails
                {
                    id = id,
                    dateResponseHistory = dateResponseHistory,
                    commentResponseHistory = commentResponseHistory,
                    ViaResponseHistory = ViaResponseHistory,
                    sendAsEmail = sendAsEmail,
                    emailSender = ServicesProxy.RequestState.UserDetails.Email,
                    emailCc = emailCc,
                    emailBcc = emailBcc
                }), JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult LinkTicket(int id, int linkedId)
        {
            return Json(ServicesProxy.TicketService.AddOrUpdateLinkedTicket(id, linkedId), JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult UnlinkTicket(int id, int linkedId)
        {
            return Json(ServicesProxy.TicketService.DeleteLinkedTicket(id, linkedId), JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult UploadFiles(Ticket ticket)
        {
            return Json(TicketModel.UploadFiles(Request.Files, ServicesProxy, ticket), JsonRequestBehavior.AllowGet);
        }

        public ActionResult DownloadAttachment(int id, int AttachmentId)
        {
            var result = ServicesProxy.TicketService.GetTicketAttachment(id, AttachmentId);
            if (!result.OK)
                return Json(result);

            var cd = new System.Net.Mime.ContentDisposition
            {
                FileName = result.Attachment.Filename,
                Inline = false,
            };
            Response.AppendHeader("Content-Disposition", cd.ToString());
            return File(result.Attachment.Data, System.Net.Mime.MediaTypeNames.Application.Octet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult AddAttachment(int id, Attachment Attachment)
        {
            log.Debug(string.Format("Begin AddAttachment({0}, {1})", id, Attachment.Filename));
            try
            {
                var result = ServicesProxy.TicketService.AddOrUpdateAttachment(id, Attachment);
                log.Debug(string.Format("{0} TicketService.AddOrUpdateAttachment: {1}", result.OK ? "Call" : "Fail", result));
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                log.Debug(string.Format("End AddAttachment({0}, {1})", id, Attachment.Filename));
            }
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult DeleteAttachment(int id, int AttachmentId)
        {
            return Json(ServicesProxy.TicketService.DeleteAttachment(id, AttachmentId), JsonRequestBehavior.AllowGet);
        }

        public string StopStreetIntersection(int id)
        {
            return ServicesProxy.MapsScheduleService.GetStopStreetIntersection(id);
        }

        public string StopCity(int id)
        {
            return ServicesProxy.MapsScheduleService.GetStopCity(id, Config.ReverseGeocodeUrl);
        }

        #endregion

        #region Private Methods

        private TicketModel CreateTicketModel(int id)
        {
            return new TicketModel(ServicesProxy, ServicesProxy.TicketService.Get(id));
        }

        #endregion

    }
}