using System;
using System.IdentityModel.Services;
using System.IdentityModel.Tokens;
using ACTransit.CusRel.Infrastructure;
using ACTransit.CusRel.Repositories;

namespace ACTransit.CusRel.Security
{
    public static class PassiveSessionConfiguration
    {
        /// <summary>
        /// Implement sliding sessions (refresh SessionToken if less than 1/2 time remaining)
        /// </summary>
        public static void EnableSlidingSessionExpirations()
        {
            var sam = FederatedAuthentication.SessionAuthenticationModule;
            if (sam == null) throw new ArgumentException("SessionAuthenticationModule is null");

            sam.IsReferenceMode = Config.IsReferenceMode;

            sam.SessionSecurityTokenReceived +=
                delegate(object sender, SessionSecurityTokenReceivedEventArgs e)
                {
                    var token = e.SessionToken;
                    token.IsReferenceMode = Config.IsReferenceMode;
                    var duration = token.ValidTo.Subtract(token.ValidFrom);
                    if (duration <= TimeSpan.Zero) return;

                    var diff = token.ValidTo.Add(sam.FederationConfiguration.IdentityConfiguration.MaxClockSkew).Subtract(DateTime.UtcNow);
                    if (diff <= TimeSpan.Zero) return;

                    var halfWay = duration.Add(sam.FederationConfiguration.IdentityConfiguration.MaxClockSkew).TotalMinutes / 2;
                    var timeLeft = diff.TotalMinutes;
                    if (!(timeLeft <= halfWay)) return;
                    // set duration not from original token, but from current app configuration
                    var handler = sam.FederationConfiguration.IdentityConfiguration.SecurityTokenHandlers[typeof(SessionSecurityToken)] as SessionSecurityTokenHandler;
                    if (handler != null) duration = handler.TokenLifetime;

                    e.ReissueCookie = true;
                    e.SessionToken =
                        new SessionSecurityToken(
                            token.ClaimsPrincipal,
                            token.Context,
                            DateTime.UtcNow,
                            DateTime.UtcNow.Add(duration))
                        {
                            IsPersistent = token.IsPersistent,
                            IsReferenceMode = token.IsReferenceMode
                        };
                };
        }

        public static void ConfigureSessionCache(ITokenCacheRepository tokenCacheRepository)
        {
            if (!Config.IsReferenceMode) return;
            if (!(FederatedAuthentication.FederationConfiguration.IdentityConfiguration.Caches.SessionSecurityTokenCache is PassiveRepositorySessionSecurityTokenCache))
                FederatedAuthentication.FederationConfiguration.IdentityConfiguration.Caches.SessionSecurityTokenCache = new PassiveRepositorySessionSecurityTokenCache(tokenCacheRepository);
        }
    }

}