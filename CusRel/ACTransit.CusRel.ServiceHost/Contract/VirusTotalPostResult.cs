using Newtonsoft.Json;

namespace ACTransit.CusRel.ServiceHost.Contract
{
    [JsonObject(MemberSerialization.OptIn)]
    public class VirusTotalPostResult
    {
        [JsonProperty("response_code")]
        public int ResponseCode { get; set; }

        [JsonProperty("verbose_msg")]
        public string VerboseMsg { get; set; }

        [JsonProperty("resource")]
        public string Resource { get; set; }

        [JsonProperty("scan_id")]
        public string ScanId { get; set; }

        [JsonProperty("permalink")]
        public string Permalink { get; set; }

        [JsonProperty("sha256")]
        public string Sha256 { get; set; }

        [JsonProperty("sha1")]
        public string Sha1 { get; set; }

        [JsonProperty("md5")]
        public string Md5 { get; set; }
    }
}
