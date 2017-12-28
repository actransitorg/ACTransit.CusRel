using System.Runtime.Serialization;
using ACTransit.Contracts.Data.CusRel.ReportContract;
using ACTransit.Contracts.Data.CusRel.ReportContract.Params;
using ACTransit.Contracts.Data.CusRel.ReportContract.Result;
using ACTransit.CusRel.Services;
using Newtonsoft.Json;

namespace ACTransit.CusRel.Models
{
    [DataContract, JsonObject(MemberSerialization.OptIn)]
    public class ReadyToCloseReportModel : ReadyToCloseReportResult
    {
        #region SendToClient

        [DataMember]
        public ReadyToCloseReportParams ReportParams { get; set; }

        [DataMember]
        public ReadyToCloseReportParams PostReportParams { get; set; }

        //[DataMember]
        //public List<ReadyToCloseReportParamItem> SelectedItems { get; set; }

        [DataMember]
        public ReadyToCloseReportTableItem Header { get; set; }

        [DataMember]
        public ReadyToCloseReportResult Result { get; set; }

        #endregion

        #region ServerSideOnly

        #endregion

        #region Constructors

        public ReadyToCloseReportModel(ServicesProxy servicesProxy)
        {
            Header = new ReadyToCloseReportTableItem();
            PostReportParams = new ReadyToCloseReportParams();
            ReportParams = new ReadyToCloseReportParams();
            //SelectedItems = ReportParams.Items;
            Result = servicesProxy.ReportService.ReadyToCloseReport(ReportParams);
        }

        #endregion

    }
}