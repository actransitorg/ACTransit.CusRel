using System;
using System.Runtime.Serialization;
using ACTransit.Contracts.Data.CusRel.ReportContract;
using ACTransit.Contracts.Data.CusRel.ReportContract.Params;
using ACTransit.Contracts.Data.CusRel.ReportContract.Result;
using ACTransit.CusRel.Services;
using Newtonsoft.Json;

namespace ACTransit.CusRel.Models
{
    [DataContract, JsonObject(MemberSerialization.OptIn)]
    public class RejectedReportModel: RejectedReportResult
    {
        #region SendToClient

        [DataMember]
        public ReportParams ReportParams { get; set; }

        [DataMember]
        public ReadyToCloseReportParams PostReportParams { get; set; }

        [DataMember]
        public RejectedReportTableItem Header { get; set; }

        [DataMember]
        public RejectedReportResult Result { get; set; }

        #endregion

        #region ServerSideOnly


        #endregion

        #region Constructors

        public RejectedReportModel(ServicesProxy servicesProxy)
        {
            Header = new RejectedReportTableItem();
            PostReportParams = new ReadyToCloseReportParams();
            ReportParams = new ReportParams
            {
                RangeStart = DateTime.Parse("1990-01-01"),
                RangeEnd = DateTime.Parse("2100-01-01")
            };
            Result = servicesProxy.ReportService.RejectedReport(ReportParams);
        }

        #endregion

    }
}