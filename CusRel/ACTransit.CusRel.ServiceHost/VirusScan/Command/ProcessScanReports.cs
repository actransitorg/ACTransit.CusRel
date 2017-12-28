using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACTransit.CusRel.ServiceHost.Common;
using ACTransit.CusRel.ServiceHost.Contract;
using ACTransit.CusRel.ServiceHost.Request;
using ACTransit.CusRel.ServiceHost.VirusScan.Request;
using ACTransit.Entities.CustomerRelations;
using log4net;
using Newtonsoft.Json;

namespace ACTransit.CusRel.ServiceHost.VirusScan.Command
{
    public class ProcessScanReports: VirusScanRequest
    {
        private static readonly ILog log = LogManager.GetLogger(nameof(ProcessScanReports));
        public ProcessScanReports(RequestState state = null) : base(state) { }

        public override void Execute()
        {
            base.Execute();
            log.Debug("Begin Execute");
            try
            {
                var api = new ApiClient();
                var count = 0;

                foreach (var item in (List<tblAttachmentsTemp>)State.Data)
                {
                    log.Debug($"Checking reports for {item.FileName}");
                    count++;
                    if (count == Config.Instance.VirusScanTaskScanReportCountLimit)
                    {
                        log.Debug("Scan Report Count Limit reached.");
                        return;
                    }

                    var boundary = "-----" + Guid.NewGuid().ToString().Replace("-", "");
                    var request = new ApiClient.Request
                    {
                        Url = Config.Instance.VirusScanTaskScanReportUrl,
                        Method = "POST",
                        Headers = new NameValueCollection
                        {
                            {"content-type", $"multipart/form-data; boundary={boundary}"},
                        },
                        Body = ApiClient.BuildMultipartForm(
                            boundary,
                            new List<KeyValuePair<string, string>>
                            {
                                new KeyValuePair<string, string>("apikey", Config.Instance.VirusScanTaskApiKey),
                                new KeyValuePair<string, string>("resource", item.ScanId)
                            }, 
                            null
                        ),
                        ResponseAsString = true
                    };
                    var response = api.ClientRequest(request);
                    var json = JsonConvert.DeserializeObject<VirusTotalScanReportResult>(response.ToString());
                    if (json.ResponseCode == 1)
                    {
                        var isOk = json.Positives == 0 ? "returned OK, " : "";
                        var result = $"Report {isOk} found  {json.Positives} positives out of {json.Total} scans";
                        log.Info(result);
                        item.ScanResult = json.Positives == 0 ? "OK" : result + "\n" + response;
                        if (item.ScanResult != "OK")
                        {
                            UpdateAttachmentTemp(item);
                            Results.Add(new Result(item.ScanId, response.StatusCode.ToString(), response.StatusDescription, null));
                            var email = new EmailError();
                            email.GenerateErrorMessageEmail($"{result}<br/> Filename: {item.FileName}<br/> ScanId: {item.ScanId}<br/> Returned JSON:<br/>{response.ToString()}");
                        }
                        else
                        {
                            var isSaved = AllowAttachment(item);
                            if (isSaved)
                            {
                                CloseAttachmentTemp(item);
                                Results.Add(new Result(item.ScanId, response.StatusCode.ToString(), response.StatusDescription, null));
                            }
                            else
                                throw new Exception($"Could not save create attachment for Id: {item.Id}");
                        }
                    }
                    else
                    {
                        var warning = $"Report not completed, JSON returned: {response}";
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
