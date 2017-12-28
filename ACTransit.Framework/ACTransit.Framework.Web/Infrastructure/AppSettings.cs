using ACTransit.Framework.Configurations;

namespace ACTransit.Framework.Web.Infrastructure
{
    public static class AoppSettings
    {
        public static string UploadDirectory { get; private set; }

        static AoppSettings()
        {
            UploadDirectory = ConfigurationUtility.GetStringValue("UploadDirectory");
        }
    }
}