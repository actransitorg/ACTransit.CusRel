using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using ACTransit.Contracts.Data.CusRel.ReportContract;
using ACTransit.Contracts.Data.CusRel.ReportContract.Params;
using ACTransit.Contracts.Data.CusRel.ReportContract.Result;
using ACTransit.Contracts.Data.CusRel.UserContract;
using ACTransit.Contracts.Data.CusRel.UserContract.Result;
using ACTransit.CusRel.Services;
using Newtonsoft.Json;

namespace ACTransit.CusRel.Models
{
    [DataContract, JsonObject(MemberSerialization.OptIn)]
    public class AssignedToReportModel: AssignedToReportResult
    {
        #region model Builder Objects

        private readonly UsersResult usersResult;

        #endregion

        #region SendToClient

        [DataMember]
        public AssignedToReportParams ReportParams { get; set; }

        [DataMember]
        public AssignedToReportParams PostReportParams { get; set; }

        [DataMember]
        public User SelectedEmployee { get; set; }

        [DataMember]
        public AssignedToReportTableItem Header { get; set; }

        [DataMember]
        public AssignedToReportResult Result { get; set; }

        #endregion

        #region ServerSideOnly

        public IEnumerable<SelectListItem> Employees
        {
            get { return new SelectList(usersResult.Users, "Id", "Username", SelectedEmployee.Id); }
        }

        #endregion

        #region Constructors

        public AssignedToReportModel(ServicesProxy servicesProxy, User defaultEmployee = null)
        {
            Header = new AssignedToReportTableItem();
            PostReportParams = new AssignedToReportParams();
            usersResult = servicesProxy.UserService.GetUsers();

            if (servicesProxy.RequestState != null && servicesProxy.RequestState.UserDetails != null)
                SelectedEmployee = defaultEmployee ?? usersResult.GetUser(servicesProxy.RequestState.UserDetails.Id);

            if (SelectedEmployee == null)
                SelectedEmployee = new User();

            ReportParams = new AssignedToReportParams
            {
                AssignedTo = SelectedEmployee
            };

            Result = servicesProxy.ReportService.AssignedToReport(ReportParams);
        }

        #endregion

    }
}