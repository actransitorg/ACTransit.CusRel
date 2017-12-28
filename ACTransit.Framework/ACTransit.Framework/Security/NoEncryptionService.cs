using ACTransit.Framework.Security.Interface;

namespace ACTransit.Framework.Security
{
    /// <summary>
    /// Does not perform any encryption or decryption.  Will simply return the given values.
    /// </summary>
    public class NoEncryptionService : IEncryptionService
    {
        public string EncryptString(string value)
        {
            return value;
        }

        public string DecryptString(string value)
        {
            return value;
        }
    }
}