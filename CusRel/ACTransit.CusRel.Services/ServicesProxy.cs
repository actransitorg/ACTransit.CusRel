using ACTransit.Contracts.Data.CusRel.Common;

namespace ACTransit.CusRel.Services
{
    /// <summary>
    /// Reserved for future architecture changes (e.g. networked services)
    /// </summary>
    public class ServicesProxy
    {
        public RequestState RequestState;

        public ReportService ReportService;
        public TicketService TicketService;
        public UserService UserService;
        public SettingsService SettingsService;
        public MapsScheduleService MapsScheduleService;
        public EmailService EmailService;
    }
}
