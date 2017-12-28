using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Configuration;
using ACTransit.Framework.Configurations;
using ACTransit.Framework.Extensions;
using ACTransit.Framework.Notification;
using log4net;

namespace ACTransit.CusRel.ServiceHost.Common
{
    public static class MailSettings
    {
        private static readonly ILog log = LogManager.GetLogger(nameof(MailSettings));

        public static bool EmailEnabled { get; private set; }

        /// <summary>
        /// List of emails addresses to send exception alerts to.
        /// </summary>
        public static IEnumerable<string> ExceptionAlertToEmails { get; private set; }

        /// <summary>
        /// List of cc email addresses to send exception alerts to.
        /// </summary>
        public static IEnumerable<string> ExceptionAlertCcEmails { get; private set; }

        /// <summary>
        /// List of Bcc email addresses to send exception alerts to.
        /// </summary>
        public static IEnumerable<string> ExceptionAlertBccEmails { get; private set; }

        /// <summary>
        /// List of emails addresses to override customer emails to.
        /// </summary>
        public static IEnumerable<string> EmailOverrideTo { get; private set; }

        /// <summary>
        /// Subject template for application's email subject (designed to use with string.Format())
        /// </summary>
        public static string EmailSubject { get; private set; }

        public static SmtpSection SmtpSection()
        {
            return (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
        }

        static MailSettings()
        {
            try
            {
                var emailEnabled = ConfigurationUtility.GetStringValue("EmailEnabled");
                var toEmails = ConfigurationUtility.GetStringValue("AppExceptionAlert_To");
                var ccEmails = ConfigurationUtility.GetStringValue("AppExceptionAlert_Cc");
                var bccEmails = ConfigurationUtility.GetStringValue("AppExceptionAlert_Bcc");
                var overrideTo = ConfigurationUtility.GetStringValue("EmailOverrideTo");
                var emailSubject = ConfigurationUtility.GetStringValue("EmailSubject");


                EmailEnabled = emailEnabled != null && emailEnabled.ToBool().GetValueOrDefault();
                ExceptionAlertToEmails = toEmails == null ? Enumerable.Empty<string>() : toEmails.ToEnumerable<string>(";");
                ExceptionAlertCcEmails = ccEmails == null ? Enumerable.Empty<string>() : ccEmails.ToEnumerable<string>(";");
                ExceptionAlertBccEmails = ccEmails == null ? Enumerable.Empty<string>() : bccEmails.ToEnumerable<string>(";");
                EmailOverrideTo = overrideTo == null ? Enumerable.Empty<string>() : overrideTo.ToEnumerable<string>(";");
                EmailSubject = emailSubject ?? "Email from ACTransit";
            }
            catch (Exception e)
            {
                log.Error(e);
            }
        }
    }

    public class EmailError
    {
        private readonly ILog log = LogManager.GetLogger(nameof(EmailError));

        public void GenerateRetryEmail(string ClientId)
        {
            try
            {
                log.Info("Generating Retry Email");
                var emailService = new SMTPEmailService(MailSettings.SmtpSection().Network.Host);
                var overrideToList = MailSettings.EmailOverrideTo != null ? MailSettings.EmailOverrideTo.ToList() : null;

                var payload = new EmailPayload
                {
                    FromEmailAddress = MailSettings.SmtpSection().From,
                    To = overrideToList != null && overrideToList.Count > 0 ? overrideToList : new List<string> { Config.Instance.EmailTooManyRetriesTo },
                    Subject = Config.Instance.EmailTooManyRetriesSubject,
                    Body = $"Retry count has exceeded {Config.Instance.EmailTooManyRetriesCount} from client {ClientId}.  Please review provided interface data.",
                    IsBodyHtml = true,
                };
                emailService.Send(payload);
            }
            catch (Exception e)
            {
                log.Error("Fail GenerateRetryEmail", e);
            }
        }

        public void GenerateErrorMessageEmail(string Message)
        {            
            try
            {
                log.Info("Generating ErrorMessage Email");
                var emailService = new SMTPEmailService(MailSettings.SmtpSection().Network.Host);
                var overrideToList = MailSettings.EmailOverrideTo != null ? MailSettings.EmailOverrideTo.ToList() : null;

                var payload = new EmailPayload
                {
                    FromEmailAddress = MailSettings.SmtpSection().From,
                    To = overrideToList.Count > 0 ? overrideToList : new List<string> { Config.Instance.EmailErrorMessageTo },
                    Subject = "CusRelService Error",
                    Body = Message,
                    IsBodyHtml = true,
                };
                emailService.Send(payload);
            }
            catch (Exception e)
            {
                log.Error("Fail ErrorMessage Email", e);
            }
        }
    }
}
