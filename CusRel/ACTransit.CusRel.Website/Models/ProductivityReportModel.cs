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
    public class ProductivityReportModel: ProductivityReportResult
    {
        #region SendToClient

        [DataMember]
        public ReportParams ReportParams { get; set; }

        [DataMember]
        public ReportParams PostReportParams { get; set; }

        [DataMember]
        public ProductivityReportTableItem Header { get; set; }

        [DataMember]
        public ProductivityReportResult Result { get; set; }

        #endregion

        #region ServerSideOnly


        #endregion

        #region Constructors

        public ProductivityReportModel(ServicesProxy servicesProxy)
        {
            Header = new ProductivityReportTableItem();
            PostReportParams = new ReportParams();
            ReportParams = new ReportParams
            {
                RangeStart = DateTime.Now.Date.AddMonths(-1),
                RangeEnd = DateTime.Now.Date.AddDays(1).AddSeconds(-1)
            };
            Result = servicesProxy.ReportService.ProductivityReport(ReportParams);
        }

        #endregion

    }
}