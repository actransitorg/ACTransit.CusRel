using ACTransit.Contracts.Data.CusRel.ReportContract.Params;
using ACTransit.Contracts.Data.CusRel.ReportContract.Result;
using ACTransit.Contracts.Data.CusRel.UserContract.Result;
using ACTransit.CusRel.Repositories;

namespace ACTransit.CusRel.Services
{
    public class ReportService
    {
        private readonly ServicesProxy servicesProxy;
        private readonly ReportRepository reportRepository;

        public ReportService(ServicesProxy servicesProxy = null)
        {
            this.servicesProxy = servicesProxy;
            reportRepository = new ReportRepository(servicesProxy != null ? servicesProxy.RequestState : null);
        }

        public AssignedToReportResult AssignedToReport(AssignedToReportParams Params)
        {
            UserResult userResult = null;
            if (Params != null && Params.AssignedTo != null && (Params.AssignedTo.Id == null || Params.AssignedTo.Username == null))
            {
                userResult = servicesProxy.UserService.GetUser(Params.AssignedTo.Id);
                if (!userResult.OK)
                    userResult = servicesProxy.UserService.GetUser(Params.AssignedTo.Username);
                if (userResult.OK)
                    Params.AssignedTo = userResult.User;
            }
            var reportResult = reportRepository.AssignedToReport(Params);
            if (userResult != null)
                reportResult.MergeResults(userResult);
            return reportResult;
        }

        public ForActionReportResult ForActionReport(ForActionReportParams Params)
        {
            return reportRepository.ForActionReport(Params);
        }

        public ReadyToCloseReportResult ReadyToCloseReport(ReportParams Params)
        {
            return reportRepository.ReadyToCloseReport(Params);
        }

        public LostFoundReportResult LostFoundReport(LostFoundReportParams Params)
        {
            return reportRepository.LostFoundReport(Params);
        }

        public RejectedReportResult RejectedReport(ReportParams Params)
        {
            return reportRepository.RejectedReport(Params);
        }

        public OpenTicketsReportResult OpenTicketsReport(ReportParams Params)
        {
            return reportRepository.OpenTicketsReport(Params);
        }

        public OpenTicketsStatusReportResult OpenTicketsStatusReport(OpenTicketStatusReportParams Params)
        {
            return reportRepository.OpenTicketsStatusReport(Params);
        }

        public ProductivityReportResult ProductivityReport(ReportParams Params)
        {
            return reportRepository.ProductivityReport(Params);
        }

    }
}
