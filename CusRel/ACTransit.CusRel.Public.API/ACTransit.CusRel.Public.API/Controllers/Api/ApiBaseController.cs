using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Description;
using ACTransit.Contracts.Data.Common;
using ACTransit.Contracts.Data.Common.PublicSite;
using ACTransit.Framework.Infrastructure.FileSystem;
using ACTransit.CusRel.Public.API.Models;
using log4net;

namespace ACTransit.CusRel.Public.API.Controllers.Api
{
    public class ApiBaseController : ApiController
    {
        private static readonly ILog Log = LogManager.GetLogger("api");
        public RequestState RequestState { get; private set; }

        /// <summary>
        /// Setup request state for services
        /// </summary>
        [ApiExplorerSettings(IgnoreApi = true)]
        public void Prepare(RequestState RequestState = null)
        {
            if (RequestState == null) RequestState = new RequestState();
            if (this.RequestState != null) return;

            var path = !string.IsNullOrEmpty(HttpRuntime.AppDomainAppId) ? HttpRuntime.AppDomainAppPath : AppDomain.CurrentDomain.BaseDirectory;

            this.RequestState = new RequestState
            {
                MainSiteUrl = Scheme + "://" + Config.MainSiteUrl,
                RequestSourcePostUrl = RequestState.RequestSourcePostUrl ?? Config.RequestSourcePostUrl ?? Scheme + "://" + Config.MainSiteUrl,
                ResponseTargetPostUrl = RequestState.ResponseTargetPostUrl ?? Config.ResponseTargetPostUrl ?? Scheme + "://" + Config.ApiSiteUrl + "/article/",
                FeedbackToEmail = RequestState.FeedbackToEmail ?? Config.FeedbackToEmail,
                MaxListArticleAge = RequestState.MaxListArticleAge | Convert.ToInt32(Config.MaxListArticleAge),
                ConnectionStrings = RequestState.ConnectionStrings ?? Config.ConnectionStringSettings,
                ContentSearchReplaceList = RequestState.ContentSearchReplaceList ?? SearchReplaceList.Get(path + Config.ContentSearchReplaceFile),
                RedirectionItemUrl = RequestState.RedirectionItemUrl ?? (Request != null ? HttpUtility.ParseQueryString(Request.RequestUri.Query).Get("RedirUri") : null)
            };
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public string Scheme
        {
            get
            {
                return (Request != null && Request.RequestUri != null)
                    ? Request.RequestUri.Scheme
                    : "http";
            }
        }

        //public HttpResponseMessage Options()
        //{
        //    return new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
        //}

        public override Task<HttpResponseMessage> ExecuteAsync(HttpControllerContext controllerContext,CancellationToken cancellationToken)
        {
             return base
                .ExecuteAsync(controllerContext, cancellationToken)
                .ContinueWith(t => 
                {
                    Log.Info(string.Format("{0} {1}: {2}", t.Result.RequestMessage.RequestUri, t.Result.RequestMessage.Method, t.Result.ReasonPhrase));
                    return t.Result;
                }, cancellationToken);
        }
    }
}