using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using ACTransit.Contracts.Data.Common;
using ACTransit.Contracts.Data.Common.PublicSite;
using ACTransit.Contracts.Data.Customer.Feedback;
using ACTransit.Framework.Extensions;
using ACTransit.Framework.Notification;
using ACTransit.CusRel.Public.Domain.Repositories.CustomerRelations.DAL;

namespace ACTransit.CusRel.Public.Domain.Services.Customer.Feedback
{
    /// <summary>
    /// Handle rider feedback submits
    /// </summary>
    public class FeedbackService
    {
        public string Uri { get; private set; }
        public string OkResponse { get; private set; }

        private readonly Form form;
        private readonly RequestState requestState;

        public FeedbackService(Form form, RequestState requestState)
        {
            this.form = form;
            this.requestState = requestState;
        }

        public FeedbackService(Form form, string Uri, string OkResponse)
        {
            this.form = form;
            this.Uri = Uri;
            this.OkResponse = OkResponse;
        }

        /// <summary>
        /// Add Customer Contact to database, then send Customer Feedback email
        /// </summary>
        /// <returns></returns>
        public ResponseResult SendToCusRelEntity()
        {
            var result = new ResponseResult();
            try
            {
                var addCustomerResult = Repository.Context(requestState).AddCustomerContact(form);
                if (string.IsNullOrEmpty(requestState.FeedbackToEmail))
                {
                    result.OK = true;
                    result.StatusCode = 200;
                }
                else if (addCustomerResult.OK)
                {
                    var smtp = new SMTPEmailService();
                    var emailHeader = form.ActionEnum.Description().ToUpper();
                    var email = new EmailPayload
                    {
                        FromEmailAddress = form.Contact.SmtpEmail,
                        To = new List<string> { requestState.FeedbackToEmail },
                        Subject = "AC Transit Public Customer Feedback " + emailHeader,
                        Body = form.AsEmail(addCustomerResult.ID.ToString(CultureInfo.InvariantCulture), emailHeader),
                    };
                    smtp.Send(email);
                    result.OK = true;
                    result.StatusCode = 200;
                }
            }
            catch (Exception e)
            {
                result = new ResponseResult(e) { StatusCode = 500, OK = false};
            }
            return result;
        }

        /// <summary>
        /// Post feedback form data to main website
        /// </summary>
        /// <returns></returns>
        public ResponseResult PostFeedback()
        {
            var result = new ResponseResult();
            try
            {
                using (var wc = new CookieWebClient())
                {
                    // get postback cookies
                    wc.DownloadString(Uri);

                    // send feedback
                    wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                    var body = form.Serialize();
                    var htmlResult = wc.UploadString(Uri, body);
                    result.OK = htmlResult.Contains(OkResponse);
                    if (result.OK) result.StatusCode = 200;
                }
            }
            catch (Exception e)
            {
                result = new ResponseResult(e) { StatusCode = 500, OK = false };
            }
            return result;
        }

        #region WebClient

        public class CookieWebClient : WebClient
        {
            public CookieContainer CookieContainer { get; private set; }

            public CookieWebClient()
            {
                CookieContainer = new CookieContainer();
            }

            protected override WebRequest GetWebRequest(Uri address)
            {
                var request = base.GetWebRequest(address);
                if (request is HttpWebRequest)
                {
                    (request as HttpWebRequest).CookieContainer = CookieContainer;
                }
                return request;
            }
        }

        #endregion

    }
}
