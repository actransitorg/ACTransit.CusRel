using System;
using System.Runtime.Serialization;
using ACTransit.Contracts.Data.CusRel.ReportContract;
using ACTransit.Contracts.Data.CusRel.ReportContract.Params;
using ACTransit.Contracts.Data.CusRel.ReportContract.Result;
using ACTransit.CusRel.Services;
using ACTransit.CusRel.Services.Extensions;
using Newtonsoft.Json;

namespace ACTransit.CusRel.Models
{
    [DataContract, JsonObject(MemberSerialization.OptIn)]
    public class OpenTicketsReportModel: OpenTicketsReportResult
    {
        #region SendToClient

        [DataMember]
        public ReportParams ReportParams { get; set; }

        [DataMember]
        public ReportParams PostReportParams { get; set; }

        [DataMember]
        public OpenTicketsReportTableItem Header { get; set; }

        [DataMember]
        public OpenTicketsReportResult Result { get; set; }

        #endregion

        #region ServerSideOnly

        private DateTimeExtensions dateTimeExtensions;

        #endregion

        #region Constructors

        public OpenTicketsReportModel(ServicesProxy servicesProxy)
        {
            Header = new OpenTicketsReportTableItem();
            dateTimeExtensions = new DateTimeExtensions(servicesProxy.MapsScheduleService);
            PostReportParams = new ReportParams();
            ReportParams = new ReportParams
            {
                RangeStart = DateTime.Parse("1/1/2012"),
                RangeEnd = DateTime.Now.Date.AddDays(1).AddSeconds(-1)
            };
            Result = servicesProxy.ReportService.OpenTicketsReport(ReportParams);
        }

        #endregion

    }
}