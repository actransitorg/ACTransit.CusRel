using System;
using System.IdentityModel.Tokens;
using System.Linq;

namespace ACTransit.CusRel.Security
{
    public class SimpleIssuerNameRegistry : ConfigurationBasedIssuerNameRegistry
    {
        public override string GetIssuerName(SecurityToken securityToken)
        {
            var x509SecurityToken = securityToken as X509SecurityToken;
            if (x509SecurityToken == null) throw new ApplicationException("The issue name can not be resolved");
            var cert = x509SecurityToken.Certificate.Thumbprint;
            if (ConfiguredTrustedIssuers.Any(c => c.Key == cert))
                return ((X509SecurityToken)securityToken).Certificate.Subject;
            throw new ApplicationException("Token not authorized.");
        }
    }
}