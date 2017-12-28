using System.Security.Claims;
using ACTransit.CusRel.Services;

namespace ACTransit.CusRel.Infrastructure
{
    /// <summary>
    /// for more information, refer to: http://brockallen.com/2013/01/17/adding-custom-roles-to-windows-roles-in-asp-net-using-claims/
    /// </summary>

    public class UserRoles
    {
        public static void SetRoles(ClaimsIdentity id, string key = null)
        {
            if (key == null) key = id.Name;
            var roles = AppCache.Cache.Get(key) as string[];
            if (roles == null)
            {
                roles = new UserService().GetUser(key).Roles;
                AppCache.Set(key, roles, 10);
            }
            foreach (var role  in roles)
                id.AddClaim(new Claim(ClaimTypes.Role, role));
        }
    }
}