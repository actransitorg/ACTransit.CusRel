using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace ACTransit.CusRel.Security
{
    public class CustomPrincipal: IPrincipal
    {
        public IIdentity Identity { get; private set; }
        public bool IsInRole(string role) { return false; }
        public CustomPrincipal(string username)
        {
            this.Identity = new GenericIdentity(username);
        }
    }

    
}