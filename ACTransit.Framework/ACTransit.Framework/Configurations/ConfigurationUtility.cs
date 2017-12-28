using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Configuration;
using System.Text.RegularExpressions;
using ACTransit.Framework.Extensions;

namespace ACTransit.Framework.Configurations
{
    public static class ConfigurationUtility
    {
        /// <summary>
        /// Retrieves a string value from the Configuration's AppSettings.
        /// </summary>
        public static string GetStringValue( string settingName )
        {
            return (ConfigurationManager.AppSettings[settingName] ?? string.Empty).ToString();
        }

        /// <summary>
        /// Retrieves a boolean value from the Configuration's AppSettings
        /// </summary>
        public static bool? GetBoolValue( string settingName )
        {
            return GetStringValue( settingName ).ToBool();
        }

        /// <summary>
        /// Retrieves an int value from the Configuration's AppSettings
        /// </summary>
        public static int? GetIntValue( string settingName )
        {
            return GetStringValue( settingName ).ToInt();
        }

        /// <summary>
        /// Retrieves a decimal value from the Configuration's AppSettings
        /// </summary>
        public static decimal? GetDecimalValue( string settingName )
        {
            return GetStringValue( settingName ).ToDecimal();
        }

        /// <summary>
        /// Retrieves a double value from the Configuration's AppSettings
        /// </summary>
        public static double? GetDoubleValue( string settingName )
        {
            return GetStringValue( settingName ).ToDouble();
        }

        /// <summary>
        /// Retrieves a list of values of the given type from the Configuration's AppSettings
        /// </summary>
        public static IEnumerable<T> GetListValues<T>( string settingName, string itemSeperator = "," )
        {
            return GetStringValue( settingName ).ToEnumerable<T>( itemSeperator );
        }

        /// <summary>
        /// Retrieves a dictionary of values of the given type from the Configuration's AppSettings
        /// </summary>
        public static IDictionary<TKey, TValue> GetDictionary<TKey, TValue>( string settingName, string keyValueSeperator = "|", string lineSeperator = "," )
        {
            return GetStringValue(settingName).ToDictionary<TKey, TValue>(keyValueSeperator, lineSeperator);
        }

        /// <summary>
        /// Retrieves a dictionary of values of the given type from the Configuration's AppSettings
        /// </summary>
        public static IDictionary<TKey, TValue> GetDictionaryTrim<TKey, TValue>(string settingName, string keyValueSeperator = "|", string lineSeperator = ",")
        {
            var value = Regex.Replace(GetStringValue(settingName), "[\n\r]+", "").Trim();
            return value.ToDictionaryTrim<TKey, TValue>(keyValueSeperator, lineSeperator);
        }

        /// <summary>
        /// Retrieves a string value from the Configuration's AppSettings.
        /// </summary>
        public static string GetConnectionStringValue(string settingName)
        {
            return ConfigurationManager.ConnectionStrings[settingName].ToString();
        }


        public static SmtpSection SmtpSection()
        {
            return (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
        }

    }
}