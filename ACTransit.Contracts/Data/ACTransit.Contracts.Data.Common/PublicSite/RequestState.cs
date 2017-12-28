using System.Configuration;
using ACTransit.Framework.Infrastructure.FileSystem;

namespace ACTransit.Contracts.Data.Common.PublicSite
{
    /// <summary>
    ///     Current state of web request
    /// </summary>
    public class RequestState
    {
        public string MainSiteUrl { get; set; }
        public string RequestSourcePostUrl { get; set; }
        public string ResponseTargetPostUrl { get; set; }
        public string FeedbackToEmail { get; set; }
        public int MaxListArticleAge { get; set; }
        public ConnectionStringSettingsCollection ConnectionStrings { get; set; }
        public SearchReplaceList ContentSearchReplaceList { get; set; }
        public string RedirectionItemUrl { get; set; }
    }
}