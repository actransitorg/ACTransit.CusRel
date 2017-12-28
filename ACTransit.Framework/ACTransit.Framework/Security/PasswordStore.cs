using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Security;
using Microsoft.Win32;
using System.Security.AccessControl;
using System.Security.Principal;

namespace ACTransit.Framework.Security
{
    public class PasswordStore
    {
        static string defaultKey = @"Me0GRAv8ZUkr1I1VEvkb0DYhEx5GJHRzwPA6cSmrhv0=";
        static string defaultIV = @"7MlOoasrykvWUu0Op/Pe2Q==";
        static string rootKey = @"SOFTWARE\ACTransit\Credentials";
        //private DataProtectionScope scope;
        internal string key;
        internal string IV;

        public PasswordStore(string key = null, string IV = null)
        {
            this.key = key != null ? key : defaultKey;
            this.IV = IV != null ? IV : defaultIV;
        }

        public void RandomizeKeyIV()
        {
            AesManaged aes = new AesManaged();
            aes.GenerateKey();
            aes.GenerateIV();
            key = Convert.ToBase64String(aes.Key);
            IV = Convert.ToBase64String(aes.IV);
        }

        public bool CheckKeyIV(PasswordStore store)
        {
            return store.key != key || store.IV != IV;
        }

        public bool Save(string username, string password)
        {
            username = username.Replace("ACTRANSIT\\", "");
            var service = new AesEncryptionService(key, IV);
            var encryptString = service.EncryptString(EncodeBase(password));
            //var currentUser = WindowsIdentity.GetCurrent().Name;
            var userKey = rootKey + "\\" + username;
            RegistryKey baseKey = null;
            RegistryKey rk = null;
            try
            {
                baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                if (baseKey == null)
                    return false;
                rk = baseKey.OpenSubKey(
                        userKey,
                        RegistryKeyPermissionCheck.ReadWriteSubTree,
                        RegistryRights.ReadKey | RegistryRights.SetValue);
                //setupRegKeyRights(currentUser, baseKey);
            }
            catch // (Exception e)
            {

            }
            if (rk == null)
                return false;
            try
            {
                rk.SetValue(null, encryptString);
            }
            catch //(Exception e)
            {
                return false;
            }
            finally
            {
                rk.Close();
                baseKey.Close();
            }
            return true;
        }

        public string Load(string username)
        {
            var encryptString = default(string);
            username = username.Replace("ACTRANSIT\\", "");
            try
            {
                var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                //var currentUser = WindowsIdentity.GetCurrent().Name;
                var userKey = rootKey + "\\" + username;
                var rk = baseKey.OpenSubKey(
                        userKey,
                        RegistryKeyPermissionCheck.ReadSubTree,
                        RegistryRights.ReadKey);
                encryptString = rk.GetValue(null) as string;
                rk.Close();
                baseKey.Close();
            }
            catch (Exception e)
            {
                return e.Message;
            }
            var service = new AesEncryptionService(key, IV);
            var decryptString = DecodeBase(service.DecryptString(encryptString));
            return decryptString;
        }

        private bool setupRegKeyRights(string currentUser, RegistryKey baseKey)
        {
            try
            {
                var rights = RegistryRights.ChangePermissions | RegistryRights.ReadKey | RegistryRights.SetValue;
                var rk = baseKey.OpenSubKey(
                        rootKey,
                        RegistryKeyPermissionCheck.ReadWriteSubTree,
                        rights);
                if (rk == null)
                    return false;
                RegistrySecurity rs = new RegistrySecurity();
                // Create access rule gIVing full control to the current user.
                rs.AddAccessRule(
                    new RegistryAccessRule(currentUser,
                        RegistryRights.FullControl,
                        InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                        PropagationFlags.InheritOnly,
                        AccessControlType.Allow));
                // Apply the new access rule to this Registry Key.
                rk.SetAccessControl(rs);
                rk = baseKey.OpenSubKey(
                    rootKey,
                    RegistryKeyPermissionCheck.ReadWriteSubTree,
                    RegistryRights.FullControl); // Opens the key again with full control.
                rs.SetOwner(new NTAccount(currentUser));// Set the securits owner to be current user.
                rk.SetAccessControl(rs);// Set the key with the changed permission so current user is now owner.
            }
            catch //(Exception e)
            {
                return false;
            }
            return true;
        }

        private string EncodeBase(string value)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
        }

        private string DecodeBase(string value)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(value));
        }
    }
}
