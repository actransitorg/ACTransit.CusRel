using System;
using System.Runtime.Serialization;

namespace ACTransit.Contracts.Data.Customer.Feedback
{
    /// <summary>
    ///     Report an Incident form to be submitted to API
    /// </summary>
    [DataContract]
    public class IncidentForm : Form
    {
        protected const string AdditionalFormatString =
            "&feedback_action={1}&incident_mon={2}&incident_day={3}&incident_year={4}&incident_hour={5}&incident_min={6}&incident_ampm={7}&vehicle_num={8}&route_num={9}&destination={10}&location={11}&employee_num={12}&employee_desc={13}";

        private const string AsEmailFormatString = @"===================== {0} =================
Date:  {1}
Time:  {2}
Bus/Vehicle Number:  {3}
Route Number:  {4}
Destination:  {5}
Location:  {6}

===================== EMPLOYEE DETAILS =====================
Badge Number:  {7}

Description:  {8}

";

        public IncidentForm()
        {
            ActionEnum = ActionEnum.report;
        }

        /// <summary>
        /// Incident info (Vehicle, Line, Location, etc.)
        /// </summary>
        [DataMember]
        public Incident Incident { get; set; }

        /// <summary>
        /// Employee (driver) infor
        /// </summary>
        [DataMember]
        public Employee Employee { get; set; }

        public override string Serialize()
        {
            return base.Serialize() +
                   string.Format(AdditionalFormatString, null, ActionEnum,
                       Incident.DateTime.Month, Incident.DateTime.Day, Incident.DateTime.Year, Incident.DateTime.Hour,
                       Incident.DateTime.Minute, Incident.DateTime.ToString("tt"),
                       Uri.EscapeUriString(Incident.Vehicle), Uri.EscapeUriString(Incident.Line),
                       Uri.EscapeUriString(Incident.Destination), Uri.EscapeUriString(Incident.Location),
                       Uri.EscapeUriString(Employee.Badge), Uri.EscapeUriString(Employee.Description));
        }

        public override string AsEmail(string FeedbackId, string EmailHeader = null)
        {
            string email = base.AsEmail(FeedbackId);
            string reportEmail = string.Format(AsEmailFormatString, EmailHeader,
                Incident.DateTime.ToShortDateString(),
                Incident.DateTime.ToShortTimeString(),
                Incident.Vehicle,
                Incident.Line,
                Incident.Destination,
                Incident.Location,
                Employee.Badge,
                Employee.Description);
            int index = email.IndexOf(InYourOwnWords, StringComparison.Ordinal);
            return email.Insert(index, reportEmail);
        }
    }
}