using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using ACTransit.Framework.Security.Interface;

namespace ACTransit.Framework.Security
{
    /// <summary>
    /// Encrypt and Decrypt string values using AES
    /// </summary>
    public class AesEncryptionService : IEncryptionService
    {
        private readonly byte[] _key;
        private readonly byte[] _iv;

        public AesEncryptionService( string key, string iv )
        {
            _key = Convert.FromBase64String( key );
            _iv = Convert.FromBase64String( iv );
        }

        public static string GenerateIV()
        {
            var aes = new AesManaged();
            aes.GenerateIV();
            return Convert.ToBase64String( aes.IV );
        }

        public static string GenerateKey( int? keySize = null )
        {
            var aes = new AesManaged();
            if( keySize.HasValue )
            {
                aes.KeySize = keySize.Value;
            }

            aes.GenerateKey();

            return Convert.ToBase64String( aes.Key );
        }

        public string EncryptString( string value )
        {
            using( var aes = new AesCryptoServiceProvider() )
            {
                var encryptor = aes.CreateEncryptor( _key, _iv );

                var valueBytes = Encoding.UTF8.GetBytes( value );

                using( var memoryStream = new MemoryStream() )
                {
                    using( var cryptoStream = new CryptoStream( memoryStream, encryptor, CryptoStreamMode.Write ) )
                    {
                        cryptoStream.Write( valueBytes, 0, valueBytes.Length );
                        cryptoStream.FlushFinalBlock();
                        cryptoStream.Close();
                    }

                    var returnValue = Convert.ToBase64String( ( memoryStream.ToArray() ) );

                    memoryStream.Close();

                    return returnValue;
                }
            }
        }

        public string DecryptString( string value )
        {
            using( var aes = new AesCryptoServiceProvider() )
            {
                var decryptor = aes.CreateDecryptor( _key, _iv );

                var bytes = Convert.FromBase64String( value );

                using( var memoryStream = new MemoryStream() )
                {
                    using( var cryptoStream = new CryptoStream( memoryStream, decryptor, CryptoStreamMode.Write ) )
                    {
                        cryptoStream.Write( bytes, 0, bytes.Length );
                        cryptoStream.FlushFinalBlock();
                        cryptoStream.Close();
                    }

                    var returnValue = Encoding.UTF8.GetString( memoryStream.ToArray() );

                    memoryStream.Close();

                    return returnValue;
                }
            }
        }
    }
}