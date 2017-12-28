using System;
using System.IdentityModel.Services;

namespace ACTransit.CusRel.Models.Auth
{
    /// <summary>
    /// This corrects WIF error ID3206 "A SignInResponse message may only redirect within the current web application:"
    /// First Check if the request url doesn't end with a "/"
    /// </summary>
    public class FixedWsFederationAuthenticationModule : WSFederationAuthenticationModule
    {
        public override void RedirectToIdentityProvider(string uniqueId, string ReturnUrl, bool persist)
        {
            if (!ReturnUrl.EndsWith("/"))
            {
                if (String.Compare(System.Web.HttpContext.Current.Request.Url.LocalPath, ReturnUrl, StringComparison.InvariantCultureIgnoreCase) == 0)
                    ReturnUrl += "/";
            }
            base.RedirectToIdentityProvider(uniqueId, ReturnUrl, persist);
        }
    }
}