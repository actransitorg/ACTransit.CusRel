using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using ACTransit.Contracts.Data.Common;
using ACTransit.Contracts.Data.Customer.Feedback;
using ACTransit.CusRel.Public.API.Models;
using ACTransit.CusRel.Public.Domain.Services.Customer.Feedback;
using WebApi.OutputCache.V2;

namespace ACTransit.CusRel.Public.API.Controllers.Api
{
    /// <summary>
    /// Submit customer feedback.
    /// </summary>
    [RoutePrefix("feedback")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class FeedbackController : ApiBaseController
    {
        private readonly string Uri;
        private const string OK = "Thank you for completing our form";

        /// <summary>
        /// Parameterless constructor.
        /// </summary>
        public FeedbackController()
        {
            Uri = "http://" + Config.MainSiteUrl + "/customer/customer-feedback";
        }

        /// <summary>
        /// "Report an Incident" feedback form submit.
        /// </summary>
        /// <param name="form">IncidentForm includes customer, incident and employee information</param>
        /// <returns>ResponseResult object</returns>
        [Route("incident")]
        [ResponseType(typeof(ResponseResult))]
        [HttpPost]
        [CacheOutput(ClientTimeSpan = 0, ServerTimeSpan = 0)]
        [ValidateModel]
        public async Task<HttpResponseMessage> Incident(IncidentForm form)
        {
            return await CreateResponse(form);
        }

        /// <summary>
        /// "Ask a Question" feedback form submit.
        /// </summary>
        /// <param name="form">AskForm includes customer information</param>
        /// <returns>ResponseResult object</returns>
        [Route("ask")]
        [ResponseType(typeof(ResponseResult))]
        [HttpPost]
        [CacheOutput(ClientTimeSpan = 0, ServerTimeSpan = 0)]
        [ValidateModel]
        public async Task<HttpResponseMessage> Ask(AskForm form)
        {
            return await CreateResponse(form);
        }

        /// <summary>
        /// "Make a Suggestion" feedback form submit.
        /// </summary>
        /// <param name="form">SuggestionForm includes customer information</param>
        /// <returns>ResponseResult object</returns>
        [Route("suggestion")]
        [ResponseType(typeof(ResponseResult))]
        [HttpPost]
        [CacheOutput(ClientTimeSpan = 0, ServerTimeSpan = 0)]
        [ValidateModel]
        public async Task<HttpResponseMessage> Suggestion(SuggestionForm form)
        {
            return await CreateResponse(form);
        }

        /// <summary>
        /// "Lost and Found" feedback form submit.
        /// </summary>
        /// <param name="form">LostFoundForm includes customer information</param>
        /// <returns>ResponseResult object</returns>
        [Route("lostfound")]
        [ResponseType(typeof(ResponseResult))]
        [HttpPost]
        [CacheOutput(ClientTimeSpan = 0, ServerTimeSpan = 0)]
        [ValidateModel]
        public async Task<HttpResponseMessage> LostFound(LostFoundForm form)
        {
            return await CreateResponse(form);
        }

        private async Task<HttpResponseMessage> CreateResponse(Form form)
        {
            Prepare();
            ResponseResult result;
            try
            {
                result = Config.FeedbackWebClientMode
                    ? new FeedbackService(form, Uri, OK).PostFeedback() 
                    : new FeedbackService(form, RequestState).SendToCusRelEntity();
            }
            catch (Exception e)
            {
                result = new ResponseResult(e) { StatusCode = 500 };
            }
            var response = Request.CreateResponse((HttpStatusCode)result.StatusCode, result);
            return response;
        }


        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //        Repository.Context.Dispose();
        //    base.Dispose(disposing);
        //}
    }
}
