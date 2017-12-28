using System;
using System.Web.Http;
using System.Web.Http.Description;
using ACTransit.Contracts.Data.Common;
using ACTransit.CusRel.Public.API.Models;
using WebApi.OutputCache.V2;

namespace ACTransit.CusRel.Public.API.Controllers.Api
{
    /// <summary>
    /// Test API version 1. To access, use 'test' URL with optional 'api-version:1' HTTP header (since this is default version).
    /// </summary>
    [VersionedRoute("test", 1)]
    public class TestController : ApiController
    {
        /// <summary>
        /// Test API call, returns current date and time.
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


    /// <summary>
    /// Test API, version 2. To access, use 'v2/test' URL, or use 'test' URL with required 'api-version:2' HTTP header.
    /// </summary>
    [VersionedRoute("test", 2)]
    [Route("v2/test")]
    public class TestV2Controller : ApiController
    {
        /// <summary>
        /// Test API call, returns ResponseResult with OK set to true.
        /// </summary>
        /// <returns>ResponseResult</returns>
        [ResponseType(typeof(DateTime))]
        [HttpGet]
        [CacheOutput(ClientTimeSpan = 0, ServerTimeSpan = 0)]
        public ResponseResult Get()
        {
            return new ResponseResult { OK = true };
        }
    }
}
