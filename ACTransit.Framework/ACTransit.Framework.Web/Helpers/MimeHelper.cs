using System;
using System.Reflection;
using System.Web;

namespace ACTransit.Framework.Web.Helpers
{
    public static class MimeHelper
    {
        private static readonly object Locker = new object();
        private static readonly object mimeMapping = null;
        private static readonly MethodInfo GetMimeMappingMethodInfo;
        static MimeHelper()
        {
            var mimeMappingType = Assembly.GetAssembly(typeof(HttpRuntime)).GetType("System.Web.MimeMapping");
            if (mimeMappingType == null)
                throw new SystemException("Couldnt find MimeMapping type");
            GetMimeMappingMethodInfo = mimeMappingType.GetMethod("GetMimeMapping",
                BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            if (GetMimeMappingMethodInfo == null)
                throw new SystemException("Couldnt find GetMimeMapping method");
            if (GetMimeMappingMethodInfo.ReturnType != typeof(string))
                throw new SystemException("GetMimeMapping method has invalid return type");
            if (GetMimeMappingMethodInfo.GetParameters().Length != 1
                && GetMimeMappingMethodInfo.GetParameters()[0].ParameterType != typeof(string))
                throw new SystemException("GetMimeMapping method has invalid parameters");
        }

        public static string GetMimeType(string filename)
        {
            lock (Locker)
                return (string)GetMimeMappingMethodInfo.Invoke(mimeMapping,
                    new object[] { filename });
        }

    }
}
