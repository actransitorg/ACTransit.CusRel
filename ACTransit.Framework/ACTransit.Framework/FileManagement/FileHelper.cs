using System;
using System.IO;
using System.Linq;

namespace ACTransit.Framework.FileManagement
{
    public static class FileHelper
    {

        public static string FileToBase64(string filePath)
        {
            string base64String;

            try
            {
                var bytes = File.ReadAllBytes(filePath);
                base64String = Convert.ToBase64String(bytes);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in FileToBase64", ex);
            }

            return base64String;
        }

        public static void Base64ToFile(string base64Str, string filePath)
        {
            try
            {
                Byte[] bytes = Convert.FromBase64String(base64Str);
                File.WriteAllBytes(filePath, bytes);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in Base64ToFile", ex);
            }
        }

        public static string GetMimeByExtension(string extension)
        {
            return MimeTypeMap.List.MimeTypeMap.GetMimeType(extension).FirstOrDefault();
        }

        public static string GetExtensionByMime(string mimeType)
        {
            return MimeTypeMap.List.MimeTypeMap.GetExtension(mimeType).FirstOrDefault();
        }
    }
}
