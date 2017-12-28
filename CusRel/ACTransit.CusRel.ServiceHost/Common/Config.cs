using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace ACTransit.CusRel.ServiceHost.Common
{
    public sealed class Config
    {
        private Configuration configuration;
        private readonly ILog log = LogManager.GetLogger(nameof(Config));

        static Config() { }

        private Config()
        {
            try
            {
                try
                {
                    if (configuration != null)
                        log.Debug($"Configuration read from: {configuration.FilePath}");
                    configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    if (configuration != null)
                        log.Debug($"Configuration now reading from: {configuration.FilePath}");
                }
                catch (ArgumentException e)
                {
                }
            }
            catch (Exception e)
            {
            }

        }

        #region Public Interface

        public static Config Instance { get; } = new Config();

        public void SetConfig(Configuration configuration)
        {
            this.configuration = configuration;
            log.Debug($"Changed config to read from: {configuration.FilePath}");
        }

        public DateTime? VirusScanTaskLastRunDate
        {
            get
            {
                var result = getAppSetting("VirusScanTaskLastRunDate");
                return !string.IsNullOrEmpty(result) ? (DateTime?)DateTime.Parse(result) : null;
            }
            set
            {
                addOrUpdateAppSettings("VirusScanTaskLastRunDate", value.ToString());
            }
        }


        public int VirusScanTaskRunEverySecs
        {
            get
            {
                var result = getAppSetting("VirusScanTaskRunEverySecs");
                return !string.IsNullOrEmpty(result) ? int.Parse(result) : 60 * 60 * 2;
            }
            set
            {
                addOrUpdateAppSettings("VirusScanTaskRunEverySecs", value.ToString());
            }
        }

        public string VirusScanTaskPostUrl
        {
            get
            {
                var result = getAppSetting("VirusScanTaskPostUrl");
                return !string.IsNullOrEmpty(result) ? result : null;
            }
            set
            {
                addOrUpdateAppSettings("VirusScanTaskPostUrl", value);
            }
        }

        public int VirusScanTaskPostCountLimit
        {
            get
            {
                var result = getAppSetting("VirusScanTaskPostCountLimit");
                return !string.IsNullOrEmpty(result) ? int.Parse(result) : 0;
            }
            set
            {
                addOrUpdateAppSettings("VirusScanTaskPostCountLimit", value.ToString());
            }
        }

        public string VirusScanTaskScanReportUrl
        {
            get
            {
                var result = getAppSetting("VirusScanTaskScanReportUrl");
                return !string.IsNullOrEmpty(result) ? result : null;
            }
            set
            {
                addOrUpdateAppSettings("VirusScanTaskScanReportUrl", value);
            }
        }

        public int VirusScanTaskScanReportCountLimit
        {
            get
            {
                var result = getAppSetting("VirusScanTaskScanReportCountLimit");
                return !string.IsNullOrEmpty(result) ? int.Parse(result) : 0;
            }
            set
            {
                addOrUpdateAppSettings("VirusScanTaskScanReportCountLimit", value.ToString());
            }
        }

        public string VirusScanTaskApiKey
        {
            get
            {
                var result = getAppSetting("VirusScanTaskApiKey");
                return !string.IsNullOrEmpty(result) ? result : null;
            }
            set
            {
                addOrUpdateAppSettings("VirusScanTaskApiKey", value);
            }
        }

        public string DateFormat
        {
            get
            {
                var result = getAppSetting("DateFormat");
                return !string.IsNullOrEmpty(result) ? result : null;
            }
            set
            {
                addOrUpdateAppSettings("DateFormat", value);
            }
        }

        public string TimeFormat
        {
            get
            {
                var result = getAppSetting("TimeFormat");
                return !string.IsNullOrEmpty(result) ? result : null;
            }
            set
            {
                addOrUpdateAppSettings("TimeFormat", value);
            }
        }

        public string EmailErrorMessageTo
        {
            get
            {
                var result = getAppSetting("EmailErrorMessageTo");
                return !string.IsNullOrEmpty(result) ? result : null;
            }
            set
            {
                addOrUpdateAppSettings("EmailErrorMessageTo", value);
            }
        }

        public string EmailTooManyRetriesTo
        {
            get
            {
                var result = getAppSetting("EmailTooManyRetriesTo");
                return !string.IsNullOrEmpty(result) ? result : null;
            }
            set
            {
                addOrUpdateAppSettings("EmailTooManyRetriesTo", value);
            }
        }

        public string EmailTooManyRetriesSubject
        {
            get
            {
                var result = getAppSetting("EmailTooManyRetriesSubject");
                return !string.IsNullOrEmpty(result) ? result : null;
            }
            set
            {
                addOrUpdateAppSettings("EmailTooManyRetriesSubject", value);
            }
        }

        public int? EmailTooManyRetriesCount
        {
            get
            {
                var result = getAppSetting("EmailTooManyRetriesCount");
                int i;
                return int.TryParse(result, out i) ? (int?)i : null;
            }
            set
            {
                addOrUpdateAppSettings("EmailTooManyRetriesTo", value.GetValueOrDefault().ToString());
            }
        }

        public string ForceCloseScanResult
        {
            get
            {
                var result = getAppSetting("ForceCloseScanResult");
                return !string.IsNullOrEmpty(result) ? result : null;
            }
            set
            {
                addOrUpdateAppSettings("ForceCloseScanResult", value);
            }
        }

        #endregion
        #region Private Methods

        private string getAppSetting(string key)
        {
            try
            {
                var settings = configuration.AppSettings.Settings;
                return settings.AllKeys.Contains(key) ? settings[key].Value : null;
            }
            catch (ConfigurationErrorsException)
            {
                log.Error("Error reading app setting: " + key);
            }
            return null;
        }

        private void addOrUpdateAppSettings(string key, string value)
        {
            try
            {
                var settings = configuration.AppSettings.Settings;
                if (settings[key] == null)
                    settings.Add(key, value);
                else
                    settings[key].Value = value;
                configuration.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configuration.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException)
            {
                log.Error("Error writing app settings for (" + key + ", " + value + ')');
            }
        }

        public static string AssemblyDirectory => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        #endregion

    }
}
