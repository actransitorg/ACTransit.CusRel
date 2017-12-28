using System;
using System.Web.Http;
using System.Web.Http.Description;
using WebApi.OutputCache.V2;

namespace ACTransit.CusRel.Public.API.Controllers.Api
{
    /// <summary>
    /// Test OAuth2 bearer access token authentication.  For client support, refer to http://www.asp.net/web-api/overview/security/individual-accounts-in-web-api
    /// </summary>
    [Route("testauth")]
    [Authorize]
    public class TestAuthController : ApiController
    {
        /// <summary>
        /// Requires authentication to call
        /// </summary>
        /// <returns>DateTime</returns>
        [ResponseType(typeof(DateTime))]
        [HttpGet]
        [CacheOutput(ClientTimeSpan = 0, ServerTimeSpan = 0)]
        public string Get()
        {
            return DateTimeOffset.Now.ToString();
        }
    }
}
