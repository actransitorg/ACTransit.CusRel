using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using ACTransit.Contracts.Data.CusRel.Common;
using ACTransit.Contracts.Data.CusRel.TicketContract;
using ACTransit.Framework.Notification;
using ACTransit.Framework.Web.Infrastructure;

namespace ACTransit.CusRel.Services
{
    public class EmailService
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Dictionary<string, string[]> smtpSettings;
        private readonly ServicesProxy servicesProxy;

        public EmailService()
        {
            servicesProxy = new ServicesProxy() { SettingsService = new SettingsService(), UserService = new UserService() };
            smtpSettings = servicesProxy.SettingsService.ParseMultipleSettings("SmtpSettings");
        }

        public Result EmailHighPriorityChange(Ticket Ticket)
        {
            var result = new Result();

            if (Ticket != null)
            {
                if (IsValidEmail(Ticket.Assignment.GroupContact.Email))
                {
                    var emailDeptChange = servicesProxy.SettingsService.GetSetting("EmailHighPriorityChange").Setting.Value;
                    var emailCc = Regex.Match(emailDeptChange, "CC:([^|]*)").Groups[1].Value;
                    var emailBcc = Regex.Match(emailDeptChange, "BCC:([^|]*)").Groups[1].Value;
                    var emailSubject = Regex.Match(emailDeptChange, "Subject:([^|]*)").Groups[1].Value;
                    emailSubject = emailSubject.Replace("{Ticket.Id}", Ticket.Id.ToString());
                    var emailBody = Regex.Match(emailDeptChange, "Body:([^|]*)").Groups[1].Value;
                    emailBody = emailBody.Replace("{Ticket.Reasons}", Ticket.Reasons != null ? string.Join(", ", Ticket.Reasons.ToArray()) : "");
                    emailBody = emailBody.Replace("{Ticket.Id}", Ticket.Id.ToString());
                    emailBody = emailBody.Replace("{Ticket.Comments}", Ticket.Comments);
                    emailBody = emailBody.Replace("<br />", "<br />\r\n");

                    try
                    {
                        var emailService = new SMTPEmailService(smtpSettings["HOST"].FirstOrDefault());

                        string[] overrideToList;
                        smtpSettings.TryGetValue("OVERRIDETO", out overrideToList);

                        var payload = new EmailPayload
                        {
                            FromEmailAddress = smtpSettings["FROM"].FirstOrDefault(),
                            To = overrideToList != null && overrideToList.Length > 0 ? overrideToList.ToList() : new List<string> { Ticket.Assignment.GroupContact.Email },
                            Subject = emailSubject,
                            Body = emailBody,
                            CC = overrideToList != null && overrideToList.Length > 0 ? null : (!string.IsNullOrEmpty(emailCc) ? emailCc.Split(';').Select(a => a.Trim()).ToList() : null),
                            BCC = overrideToList != null && overrideToList.Length > 0 ? null : (!string.IsNullOrEmpty(emailBcc) ? emailBcc.Split(';').Select(a => a.Trim()).ToList() : null),
                            IsBodyHtml = true,
                        };

                        emailService.Send(payload);
                    }
                    catch (Exception e)
                    {
                        result.SetFail(e);
                    }
                }
                else
                {
                    result.SetFail(string.Format("User {0} does not have a valid email. Unable to send notification.", Ticket.Assignment.Employee.Username));
                }
            }

            return result;
        }

        public Result EmailTicketAssigned(Ticket ticket)
        {
            var result = new Result();

            if (ticket != null)
            {
                var userResult = servicesProxy.UserService.GetUser(ticket.Assignment.Employee.Username);

                if (userResult.OK && userResult.User != null && userResult.User.GetsNotificationOnAssignment && IsValidEmail(userResult.User.Email))
                {
                    string emailSettings = servicesProxy.SettingsService.GetSetting("EmailTicketAssigned").Setting.Value;
                    string emailCc = Regex.Match(emailSettings, "CC:([^|]*)").Groups[1].Value;
                    string emailBcc = Regex.Match(emailSettings, "BCC:([^|]*)").Groups[1].Value;
                    string emailSubject = Regex.Match(emailSettings, "SUBJECT:([^|]*)").Groups[1].Value;
                    string emailBody = Regex.Match(emailSettings, "BODY:([^|]*)").Groups[1].Value;

                    string[] overrideToList;

                    smtpSettings.TryGetValue("OVERRIDETO", out overrideToList);

                    try
                    {
                        var emailService = new SMTPEmailService(smtpSettings["HOST"].FirstOrDefault());

                        var payload = new EmailPayload
                        {
                            FromEmailAddress = smtpSettings["FROM"].FirstOrDefault(),
                            To = overrideToList != null && overrideToList.Length > 0 ? overrideToList.ToList() : new List<string> { userResult.User.Email },
                            Subject = emailSubject.Replace("{Ticket.Id}", ticket.Id.ToString()),
                            Body = emailBody.
                                    Replace("{Ticket.Id}", ticket.Id.ToString()).
                                    Replace("{Ticket.Priority}", ticket.Priority.HasValue ? ticket.Priority.Value.ToString() : "no priority").
                                    Replace("<br />", "<br />\r\n"),
                            CC = overrideToList != null && overrideToList.Length > 0 ? null : (!string.IsNullOrEmpty(emailCc) ? emailCc.Split(';').Select(a => a.Trim()).ToList() : null),
                            BCC = overrideToList != null && overrideToList.Length > 0 ? null : (!string.IsNullOrEmpty(emailBcc) ? emailBcc.Split(';').Select(a => a.Trim()).ToList() : null),
                            IsBodyHtml = true,
                        };

                        emailService.Send(payload);
                    }
                    catch (Exception e)
                    {
                        result.SetFail(e);
                    }
                }
                else
                {
                    result.SetFail(string.Format("User {0} does not have a valid email or is not configured to receive notifications. Notification not sent.", ticket.Assignment.Employee.Username));
                }
            }

            return result;
        }

        private bool IsValidEmail(string value)
        {
            var re = new Regex(@"\b[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,6}\b", RegexOptions.IgnoreCase);
            return (!string.IsNullOrEmpty(value) && re.IsMatch(value));
        }
    }
}
