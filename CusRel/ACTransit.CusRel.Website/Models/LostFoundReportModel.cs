using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web.Mvc;
using ACTransit.Contracts.Data.CusRel.LookupContract;
using ACTransit.Contracts.Data.CusRel.ReportContract;
using ACTransit.Contracts.Data.CusRel.ReportContract.Params;
using ACTransit.Contracts.Data.CusRel.ReportContract.Result;
using ACTransit.CusRel.Services;
using ACTransit.CusRel.Services.Extensions;
using Newtonsoft.Json;

namespace ACTransit.CusRel.Models
{
    [DataContract, JsonObject(MemberSerialization.OptIn)]
    public class LostFoundReportModel: LostFoundReportResult
    {
        private readonly ServicesProxy servicesProxy;
        private DateTimeExtensions dateTimeExtensions;

        #region SendToClient

        [DataMember]
        public LostFoundReportParams ReportParams { get; set; }

        [DataMember]
        public LostFoundReportParams PostReportParams { get; set; }

        [DataMember]
        public LostFoundReportTableItem Header { get; set; }

        [DataMember]
        public LostFoundReportResult Result { get; set; }

        [DataMember]
        public SortedDictionary<string, string[]> LostItemNodes { get; set; }

        #endregion

        #region ServerSideOnly

        public IEnumerable<SelectListItem> EmptySelectList
        {
            get { return new List<SelectListItem>(); }
        }

        //public IEnumerable<SelectListItem> LostItemCategories
        //{
        //    get { return new SelectList(servicesProxy.SettingsService.GetSetting(new Setting{Name = "LostItemCategories"}).Setting.Value.Split(';').Select(a => a.Trim()), ReportParams.LostItemCategory); }
        //}

        //public IEnumerable<SelectListItem> LostItemTypes
        //{
        //    get { return new SelectList(servicesProxy.SettingsService.GetSetting(new Setting { Name = "LostItemTypes" }).Setting.Value.Split(';').Select(a => a.Trim()), ReportParams.LostItemType); }
        //}

        #endregion

        #region Constructors

        public LostFoundReportModel(ServicesProxy servicesProxy)
        {
            this.servicesProxy = servicesProxy;
            dateTimeExtensions = new DateTimeExtensions(servicesProxy.MapsScheduleService);
            Header = new LostFoundReportTableItem();
            PostReportParams = new LostFoundReportParams();
            ReportParams = new LostFoundReportParams
            {
                RangeStart = dateTimeExtensions.PreviousWorkDay(DateTime.Now.Date),
                RangeEnd = DateTime.Now.Date.AddDays(1).AddSeconds(-1),
                LostItemCategory = null,
                LostItemType = null
            };
            Result = servicesProxy.ReportService.LostFoundReport(ReportParams);
            LostItemNodes = servicesProxy.SettingsService.GetLostItemNodes();
        }

        #endregion

    }
}