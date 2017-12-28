using System.Linq;
using WebApiThrottle;

namespace ACTransit.CusRel.Public.API.Models
{
    public class CustomThrottlingHandler : ThrottlingHandler
    {
        protected RequestIdentity SetIndentity(System.Net.Http.HttpRequestMessage request)
        {
            return new RequestIdentity()
            {
                ClientKey = request.Headers.Contains("Authorization") 
                    ? request.Headers.GetValues("Authorization").First()
                    : (request.Headers.Contains("Authorization-Key") 
                        ? request.Headers.GetValues("Authorization-Key").First()
                        : "anon"),
                ClientIp = GetClientIp(request).ToString(),
                Endpoint = request.RequestUri.AbsolutePath
            };
        }
    }
}