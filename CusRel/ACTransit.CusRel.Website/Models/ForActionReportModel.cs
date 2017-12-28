using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web.Mvc;
using ACTransit.Contracts.Data.CusRel.LookupContract.Result;
using ACTransit.Contracts.Data.CusRel.ReportContract;
using ACTransit.Contracts.Data.CusRel.ReportContract.Params;
using ACTransit.Contracts.Data.CusRel.ReportContract.Result;
using ACTransit.Contracts.Data.CusRel.UserContract;
using ACTransit.CusRel.Services;
using Newtonsoft.Json;

namespace ACTransit.CusRel.Models
{
    [DataContract, JsonObject(MemberSerialization.OptIn)]
    public class ForActionReportModel : ForActionReportResult
    {
        #region model Builder Objects

        private readonly GroupContactsResult groupContactsResult;

        #endregion

        #region SendToClient

        [DataMember]
        public ForActionReportParams ReportParams { get; set; }

        [DataMember]
        public ForActionReportParams PostReportParams { get; set; }

        [DataMember]
        public GroupContact SelectedGroupContact { get; set; }

        [DataMember]
        public ForActionReportTableItem Header { get; set; }

        [DataMember]
        public ForActionReportResult Result { get; set; }

        #endregion

        #region ServerSideOnly

        public IEnumerable<SelectListItem> GroupContacts
        {
            get { return new SelectList(groupContactsResult.GroupContacts, "Value", "Description", SelectedGroupContactValue); }
        }

        public string SelectedGroupContactValue
        {
            get
            {
                return SelectedGroupContact != null ? SelectedGroupContact.Value : GroupContact.UnassignedValue;
            }
        }

        #endregion

        #region Constructors

        public ForActionReportModel(ServicesProxy servicesProxy, string id = null)
        {
            Header = new ForActionReportTableItem();
            PostReportParams = new ForActionReportParams();
            groupContactsResult = servicesProxy.TicketService.GetGroupContacts();
            groupContactsResult.GroupContacts = groupContactsResult.GroupContacts
                .Where(c => !c.IsUnassigned || (c.IsUnassigned && servicesProxy.RequestState.UserDetails.CanViewUnassigned))
                .OrderBy(c => c.Description).ToList();

            if (servicesProxy.RequestState != null && servicesProxy.RequestState.UserDetails != null)
            {
                servicesProxy.RequestState.UserDetails.GroupContact = groupContactsResult.GetGroupContact(servicesProxy.RequestState.UserDetails.GroupContact.Code);
                SelectedGroupContact = !string.IsNullOrEmpty(id) 
                    ? groupContactsResult.GetGroupContact(id) 
                    : servicesProxy.RequestState.UserDetails.GroupContact;
            }

            if (SelectedGroupContact == null)
                SelectedGroupContact = groupContactsResult.GetGroupContact("0");

            ReportParams = new ForActionReportParams
            {
                GroupContact = SelectedGroupContactValue
            };

            Result = servicesProxy.ReportService.ForActionReport(ReportParams);
        }

        #endregion

    }
}