using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using ACTransit.Contracts.Data.CusRel.Common;

namespace ACTransit.CusRel.Models.Auth
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class ClaimsAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly string action;
        private readonly string[] resources;

        public ClaimsAuthorizeAttribute()
        {
        }

        public ClaimsAuthorizeAttribute(string action, params string[] resources)
        {
            this.action = action;
            this.resources = resources;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (!base.AuthorizeCore(httpContext)) return false;

            var prinicpal = (ClaimsPrincipal)Thread.CurrentPrincipal;
            if (!prinicpal.Identity.IsAuthenticated)
                return false;

            var claims = prinicpal.Claims.Select(c => c.Value).ToArray();
            var requestState = httpContext.Items["RequestState"] as RequestState;
            if (requestState == null) return false;
            var authorized = true;

            foreach (var resource in resources)
            {
                switch (resource)
                {
                    case "CanAccessAdmin":
                        authorized &= (claims.Any(c => c == "CanAccessAdmin") || requestState.UserDetails.CanAccessAdmin);
                        break;
                    case "CanAddTicketComments":
                        authorized &= (claims.Any(c => c == "CanAddTicketComments") || requestState.UserDetails.CanAddTicketComments);
                        break;
                    case "CanAssignTicket":
                        authorized &= (claims.Any(c => c == "CanAssignTicket") || requestState.UserDetails.CanAssignTicket);
                        break;
                    case "CanCloseTicket":
                        authorized &= (claims.Any(c => c == "CanCloseTicket") || requestState.UserDetails.CanCloseTicket);
                        break;
                }

            }
            return authorized;
        }
    }
}