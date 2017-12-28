using System.Runtime.Serialization;

namespace ACTransit.Contracts.Data.Schedules.PublicSite
{
    /// <summary>
    ///     Settings for the Schedules feature
    /// </summary>
    public class FeatureSettings
    {
        /// <summary>
        ///     Current version number.
        /// </summary>
        [DataMember]
        public int Version { get; set; }

        /// <summary>
        ///     URL of main website
        /// </summary>
        [DataMember]
        public string MainSiteUrl { get; set; }
    }
}