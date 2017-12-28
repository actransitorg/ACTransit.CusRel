using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace ACTransit.CusRel.Public.API.Controllers.Api
{
    /// <summary>
    /// Test throwing exceptions
    /// </summary>
    [System.Web.Mvc.RoutePrefix("testerror")]
    public class TestErrorController : ApiBaseController
    {
        [ResponseType(typeof(object))]
        [HttpGet]
        public async Task<IHttpActionResult> Index()
        {
            throw new Exception("Testing Exception");

            //try
            //{
            //    throw new Exception("Testing Exception");
            //}
            //catch (Exception e)
            //{
            //    return InternalServerError(e);
            //}
        }
    }
}