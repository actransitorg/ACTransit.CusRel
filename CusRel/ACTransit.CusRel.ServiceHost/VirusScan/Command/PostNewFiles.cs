using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ACTransit.Contracts.Data.Common;
using ACTransit.CusRel.ServiceHost.Common;
using ACTransit.CusRel.ServiceHost.Contract;
using ACTransit.CusRel.ServiceHost.Request;
using ACTransit.CusRel.ServiceHost.VirusScan.Request;
using ACTransit.Entities.CustomerRelations;
using log4net;
using Newtonsoft.Json;

namespace ACTransit.CusRel.ServiceHost.VirusScan.Command
{
    public class PostNewFiles : VirusScanRequest
    {
        private static readonly ILog log = LogManager.GetLogger(nameof(PostNewFiles));

        public PostNewFiles(RequestState state = null) : base(state) { }

        public override void Execute()
        {
            base.Execute();
            log.Debug("Begin Execute");
            try
            {
                var api = new ApiClient();
                var count = 0;

                foreach (var item in (List<tblAttachmentsTemp>) State.Data)
                {
                    log.Debug($"Uploading {item.FileName}");
                    count++;
                    if (count == Config.Instance.VirusScanTaskPostCountLimit)
                    {
                        log.Debug("Post Count Limit reached.");
                        return;
                    }

                    var boundary = "-----" + Guid.NewGuid().ToString().Replace("-", "");
                    var request = new ApiClient.Request
                    {
                        Url = Config.Instance.VirusScanTaskPostUrl,
                        Method = "POST",
                        Headers = new NameValueCollection
                        {
                            {"content-type", $"multipart/form-data; boundary={boundary}"},
                            {"protocol-version", "1.0"}
                        },
                        Body = ApiClient.BuildMultipartForm(
                            boundary,
                            new List<KeyValuePair<string, string>>
                            {
                                new KeyValuePair<string, string>("apikey", Config.Instance.VirusScanTaskApiKey)
                            }, 
                            new List<ApiClient.File>
                            {
                                new ApiClient.File
                                {
                                    Name = "file",
                                    FileName = item.FileName,
                                    ContentType = item.ContentType,
                                    Base64Data = item.Base64Data
                                }
                            } 
                        ),
                        ResponseAsString = true
                    };
                    var response = api.ClientRequest(request);
                    var json = JsonConvert.DeserializeObject<VirusTotalPostResult>(response.ToString());
                    if (json.ResponseCode == 1)
                    {
                        log.Info($"Uploaded {item.FileName}, ScanId: {json.ScanId}");
                        item.ScanId = json.ScanId;
                        item.Sha256 = json.Sha256;
                        item.DateScanStart = State.RunDate;
                        UpdateAttachmentTemp(item);
                        Results.Add(new Result(item.ScanId, response.StatusCode.ToString(), response.StatusDescription,
                            null));
                    }
                    else
                    {
                        var warning = $"Upload failed, JSON returned: {json}";
                        log.Error(warning);
                        Results.Add(new Result(null, response.StatusCode.ToString(), response.StatusDescription, warning));
                    }
                }
            }
            catch (Exception e)
            {
                log.Error(e);
                Results.Add(new Result(null, null, null, e.Message));

            }
            finally
            {
                log.Debug("End Execute");
            }
        }
    }
}

