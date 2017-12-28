namespace ACTransit.Framework.Security.Interface
{
    public interface IEncryptionService
    {
        /// <summary>
        /// Takes a plain-text string and returns an encrypted value.
        /// </summary>
        string EncryptString(string value);

        /// <summary>
        /// Takes an encrypted string and returns the plain-text value.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">An encryption service which does not support decryption will throw an InvalidOperationException</exception>
        string DecryptString(string value);
    }
}