using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using System.Web.Mvc;
using ACTransit.Contracts.Data.CusRel.ReportContract;
using ACTransit.Contracts.Data.CusRel.ReportContract.Params;
using ACTransit.Contracts.Data.CusRel.ReportContract.Result;
using ACTransit.CusRel.Services;
using Newtonsoft.Json;

namespace ACTransit.CusRel.Models
{
    [DataContract, JsonObject(MemberSerialization.OptIn)]
    public class OpenTicketsStatusReportModel: OpenTicketsStatusReportResult
    {
        #region SendToClient

        [DataMember]
        public OpenTicketStatusReportParams ReportParams { get; set; }

        [DataMember]
        public OpenTicketStatusReportParams PostReportParams { get; set; }

        [DataMember]
        public OpenTicketsStatusReportTableItem Header { get; set; }

        [DataMember]
        public OpenTicketsStatusReportResult Result { get; set; }

        #endregion

        #region ServerSideOnly

        public bool? SelectedAda;

        private readonly List<KeyValuePair<bool?, string>> adaList = new List<KeyValuePair<bool?, string>>
        {
            new KeyValuePair<bool?, string>(null, "Both"),
            new KeyValuePair<bool?, string>(true, "Yes"),
            new KeyValuePair<bool?, string>(false, "No"),
        };

        public IEnumerable<SelectListItem> AdaSelectList
        {
            get
            {
                return new SelectList(adaList, "Key", "Value", SelectedAda);
            }
        }

        #endregion

        #region Constructors

        public OpenTicketsStatusReportModel(ServicesProxy servicesProxy)
        {
            Header = new OpenTicketsStatusReportTableItem();
            PostReportParams = new OpenTicketStatusReportParams();
            ReportParams = new OpenTicketStatusReportParams
            {
                IsAda = null,
                RangeStart = DateTime.Now.Date.AddYears(-1),
                RangeEnd = DateTime.Now.Date.AddDays(1).AddSeconds(-1)
            };

            Result = servicesProxy.ReportService.OpenTicketsStatusReport(ReportParams);
        }

        #endregion

    }
}