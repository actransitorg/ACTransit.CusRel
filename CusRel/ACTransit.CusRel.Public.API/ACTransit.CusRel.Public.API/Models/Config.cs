using System;
using System.Configuration;

namespace ACTransit.CusRel.Public.API.Models
{
    public class Config
    {
        public static readonly string MainSiteUrl = ConfigurationManager.AppSettings["MainSiteUrl"];
        public static readonly string ApiSiteUrl = ConfigurationManager.AppSettings["ApiSiteUrl"];
        public static readonly string RequestSourcePostUrl = ConfigurationManager.AppSettings["RequestSourcePostUrl"];
        public static readonly string ResponseTargetPostUrl = ConfigurationManager.AppSettings["ResponseTargetPostUrl"];
        public static readonly string FeedbackToEmail = ConfigurationManager.AppSettings["FeedbackToEmail"];
        public static readonly string DefaultApiVersion = ConfigurationManager.AppSettings["DefaultApiVersion"];
        public static readonly string MaxListArticleAge = ConfigurationManager.AppSettings["MaxListArticleAge"];
        public static readonly ConnectionStringSettingsCollection ConnectionStringSettings = ConfigurationManager.ConnectionStrings;
        public static readonly bool FeedbackWebClientMode = Convert.ToBoolean(ConfigurationManager.AppSettings["FeedbackWebClientMode"]);
        public static readonly string ContentSearchReplaceFile = ConfigurationManager.AppSettings["ContentSearchReplaceFile"];
        public static readonly bool HelpPagesEnabled = Convert.ToBoolean(ConfigurationManager.AppSettings["HelpPagesEnabled"]);
    }
}