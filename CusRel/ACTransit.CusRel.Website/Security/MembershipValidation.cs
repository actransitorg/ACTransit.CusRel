using System;
using System.Web.Security;

namespace ACTransit.CusRel.Security
{
    public class MembershipValidation
    {
        public static bool Login(string Username, string Password)
        {
            var userName = GetUsername(Username);
            return Membership.ValidateUser(userName, Password);
        }

        // <summary>
        /// Parses the string to pull the user name out.
        /// </summary>
        /// <param name="usernameDomain">The string to parse that must 
        /// contain the username in either the domain\username or UPN format 
        /// username@domain</param>
        /// <returns>The username or the string if no domain is found.</returns>
        public static string GetUsername(string usernameDomain)
        {
            if (string.IsNullOrEmpty(usernameDomain))
                throw (new ArgumentException("Argument can't be null.", "usernameDomain"));
            if (!usernameDomain.Contains("\\"))
                return usernameDomain.Contains("@")
                    ? usernameDomain.Substring(0, usernameDomain.IndexOf("@"))
                    : usernameDomain;
            var index = usernameDomain.IndexOf("\\");
            return usernameDomain.Substring(index + 1);
        }
    }
}