using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Web.Security;

namespace ACTransit.CusRel.Security
{
    public class WindowsImpersonation
    {
        public static bool Login(string Username, string Password)
        {
            var userName = GetUsername(Username);
            return Membership.ValidateUser(userName, Password);
        }

        //public static bool Login(string Username, string Password)
        //{
        //    var token = IntPtr.Zero;
        //    var userName = GetUsername(Username);
        //    var domainName = GetDomainName(Username);
        //    if (string.IsNullOrEmpty(domainName))
        //        domainName = "ACTRANSIT";
        //    var result = LogonUser(userName, domainName, Password, 3, 0, ref token); // 3: LOGON32_LOGON_NETWORK
        //    if (!result)
        //        result = LogonUser(userName, domainName, Password, 2, 0, ref token); // 2: LOGON32_LOGON_INTERACTIVE
        //    return result;
        //}

        [DllImport("ADVAPI32.dll", EntryPoint = "LogonUserW", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int DuplicateToken(IntPtr hToken, int impersonationLevel, ref IntPtr hNewToken);

        ///
        /// A process should call the RevertToSelf function after finishing 
        /// any impersonation begun by using the DdeImpersonateClient, 
        /// ImpersonateDdeClientWindow, ImpersonateLoggedOnUser, ImpersonateNamedPipeClient, 
        /// ImpersonateSelf, ImpersonateAnonymousToken or SetThreadToken function.
        /// If RevertToSelf fails, your application continues to run in the context of the client,
        /// which is not appropriate. You should shut down the process if RevertToSelf fails.
        /// RevertToSelf Function: http://msdn.microsoft.com/en-us/library/aa379317(VS.85).aspx
        ///
        /// A boolean value indicates the function succeeded or not.
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool RevertToSelf();

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool CloseHandle(IntPtr handle);

        /// <summary>
        /// Parses the string to pull the domain name out.
        /// </summary>
        /// <param name="usernameDomain">The string to parse that must 
        /// contain the domain in either the domain\username or UPN format 
        /// username@domain</param>
        /// <returns>The domain name or "" if not domain is found.</returns>
        public static string GetDomainName(string usernameDomain)
        {
            if (string.IsNullOrEmpty(usernameDomain))
                throw (new ArgumentException("Argument can't be null.", "usernameDomain"));
            if (!usernameDomain.Contains("\\"))
                return usernameDomain.Contains("@")
                    ? usernameDomain.Substring(usernameDomain.IndexOf("@") + 1)
                    : "";
            var index = usernameDomain.IndexOf("\\");
            return usernameDomain.Substring(0, index);
        }

        /// <summary>
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

        [StructLayout(LayoutKind.Sequential)]
        public struct ProfileInfo
        {
            ///
            /// Specifies the size of the structure, in bytes.
            ///
            public int dwSize;

            ///
            /// This member can be one of the following flags: 
            /// PI_NOUI or PI_APPLYPOLICY
            ///
            public int dwFlags;

            ///
            /// Pointer to the name of the user.
            /// This member is used as the base name of the directory 
            /// in which to store a new profile.
            ///
            public string lpUserName;

            ///
            /// Pointer to the roaming user profile path.
            /// If the user does not have a roaming profile, this member can be NULL.
            ///
            public string lpProfilePath;

            ///
            /// Pointer to the default user profile path. This member can be NULL.
            ///
            public string lpDefaultPath;

            ///
            /// Pointer to the name of the validating domain controller, in NetBIOS format.
            /// If this member is NULL, the Windows NT 4.0-style policy will not be applied.
            ///
            public string lpServerName;

            ///
            /// Pointer to the path of the Windows NT 4.0-style policy file. 
            /// This member can be NULL.
            ///
            public string lpPolicyPath;

            ///
            /// Handle to the HKEY_CURRENT_USER registry key.
            ///
            public IntPtr hProfile;
        }

        [DllImport("userenv.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool LoadUserProfile(IntPtr hToken, ref ProfileInfo lpProfileInfo);

        [DllImport("Userenv.dll", CallingConvention = CallingConvention.Winapi, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool UnloadUserProfile(IntPtr hToken, IntPtr lpProfileInfo);

        public const int LOGON32_LOGON_INTERACTIVE = 2;
        public const int LOGON32_PROVIDER_DEFAULT = 0;

        private static WindowsImpersonationContext m_ImpersonationContext = null;

        public static void DoImpersonation(string userName, string password)
        {
            var token = IntPtr.Zero;
            var tokenDuplicate = IntPtr.Zero;
            const int SecurityImpersonation = 2;
            const int TokenType = 1;

            try
            {
                if (!RevertToSelf()) return;


                if (!LogonUser(userName, Environment.MachineName, password, LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, ref token))
                    return;
                if (DuplicateToken(token, SecurityImpersonation, ref tokenDuplicate) != 0)
                {
                    var m_ImpersonatedUser = new WindowsIdentity(tokenDuplicate);
                    using (m_ImpersonationContext = m_ImpersonatedUser.Impersonate())
                    {
                        if (m_ImpersonationContext == null) return;
                        Console.WriteLine("After Impersonation succeeded: " + Environment.NewLine +
                                          "User Name: " +
                                          WindowsIdentity.GetCurrent(TokenAccessLevels.MaximumAllowed).Name +
                                          Environment.NewLine +
                                          "SID: " +
                                          WindowsIdentity.GetCurrent(TokenAccessLevels.MaximumAllowed).User.
                                              Value);

                        #region LoadUserProfile
                        // Load user profile
                        var profileInfo = new ProfileInfo();
                        profileInfo.dwSize = Marshal.SizeOf(profileInfo);
                        profileInfo.lpUserName = userName;
                        profileInfo.dwFlags = 1;
                        var loadSuccess = LoadUserProfile(tokenDuplicate, ref profileInfo);

                        if (!loadSuccess)
                        {
                            Console.WriteLine("LoadUserProfile() failed with error code: " +
                                              Marshal.GetLastWin32Error());
                            throw new Win32Exception(Marshal.GetLastWin32Error());
                        }

                        if (profileInfo.hProfile == IntPtr.Zero)
                        {
                            Console.WriteLine(
                                "LoadUserProfile() failed - HKCU handle was not loaded. Error code: " +
                                Marshal.GetLastWin32Error());
                            throw new Win32Exception(Marshal.GetLastWin32Error());
                        }
                        #endregion

                        CloseHandle(token);
                        CloseHandle(tokenDuplicate);

                        // Unload user profile
                        // MSDN remarks http://msdn.microsoft.com/en-us/library/bb762282(VS.85).aspx 
                        // Before calling UnloadUserProfile you should ensure that all handles to keys that you have opened in the 
                        // user's registry hive are closed. If you do not close all open registry handles, the user's profile fails 
                        // to unload. For more information, see Registry Key Security and Access Rights and Registry Hives.
                        UnloadUserProfile(tokenDuplicate, profileInfo.hProfile);

                        // Undo impersonation
                        m_ImpersonationContext.Undo();
                    }
                }
                else
                {
                    Console.WriteLine("DuplicateToken() failed with error code: " + Marshal.GetLastWin32Error());
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
            }
            catch (Win32Exception we)
            {
                throw we;
            }
            catch
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            finally
            {
                if (token != IntPtr.Zero) CloseHandle(token);
                if (tokenDuplicate != IntPtr.Zero) CloseHandle(tokenDuplicate);

                Console.WriteLine("After finished impersonation: " + WindowsIdentity.GetCurrent().Name);
            }
        }

    }
}