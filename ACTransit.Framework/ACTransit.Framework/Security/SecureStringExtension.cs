//using System;
//using System.Runtime.InteropServices;
//using System.Security;

//namespace ACTransit.Framework.Security
//{
//    public static class SecureStringExtension
//    {
//        public static string ToUnsecureString(this SecureString securePassword)
//        {
//            if (securePassword == null)
//                throw new ArgumentNullException("securePassword");

//            IntPtr unmanagedString = IntPtr.Zero;
//            try
//            {
//                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(securePassword);
//                return Marshal.PtrToStringUni(unmanagedString);
//            }
//            finally
//            {
//                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
//            }
//        }

//        public static SecureString ToSecureString(this string password)
//        {
//            if (password == null)
//                throw new ArgumentNullException("password");

//            var securePassword = new SecureString();
//            foreach (var c in password)
//                securePassword.AppendChar(c);
//            securePassword.MakeReadOnly();
//            return securePassword;
//        }
//    }
//}
