using System.Security.Principal;
using ACTransit.Contracts.Data.CusRel.UserContract;

namespace ACTransit.Contracts.Data.CusRel.Common
{
    public class RequestState
    {
        public IPrincipal Principal { get; set; }
        public IIdentity Identity { get; set; }
        public User UserDetails { get; set; }
        public int MaxSearchCount { get; set; }
    }
}