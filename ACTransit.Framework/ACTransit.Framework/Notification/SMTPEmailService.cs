using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using ACTransit.Framework.Notification.Interface;

namespace ACTransit.Framework.Notification
{
    public class SMTPEmailService : IEmailService
    {
        public string ServerAddress { get; set; }

        public int ServerPort { get; set; }

        public string ServerLogin { get; set; }

        public string ServerPassword { get; set; }

        public bool? ServerUseSsl { get; set; }

        public string ServerDefaultFrom { get; set; }

        public SMTPEmailService() { }

        public SMTPEmailService(string smtpServerAddress)
        {
            ServerAddress = smtpServerAddress;
        }

        public SMTPEmailService(string smtpServerAddress, int smtpServerPort)
        {
            ServerAddress = smtpServerAddress;
            ServerPort = smtpServerPort;
        }

        public SMTPEmailService(string smtpServerAddress, int smtpServerPort, string smtpServerLogin, string smtpServerPassword, bool? smtpServerUseSsl, string smtpServerDefaultFrom)
        {
            ServerAddress = smtpServerAddress;
            ServerPort = smtpServerPort;
            ServerLogin = smtpServerLogin;
            ServerPassword = smtpServerPassword;
            ServerUseSsl = smtpServerUseSsl;
            ServerDefaultFrom = smtpServerDefaultFrom;
        }

        public void Send(EmailPayload payload)
        {
            ValidatePayload(payload);

            using (var mailClient = new SmtpClient())
            {
                if (!string.IsNullOrEmpty(ServerAddress))
                    mailClient.Host = ServerAddress;

                if (ServerPort != 0)
                    mailClient.Port = ServerPort;

                if (ServerUseSsl.HasValue)
                    mailClient.EnableSsl = ServerUseSsl.Value;
                else if (payload.UseSSL.HasValue)
                    mailClient.EnableSsl = payload.UseSSL.Value;

                if (!string.IsNullOrEmpty(ServerLogin) && !string.IsNullOrEmpty(ServerPassword))
                    mailClient.Credentials = new NetworkCredential(ServerLogin, ServerPassword);
                else if (!string.IsNullOrEmpty(payload.LoginName) || !string.IsNullOrEmpty(payload.Password))
                    mailClient.Credentials = new NetworkCredential(payload.LoginName, payload.Password);

                var fromEmailAddress = !string.IsNullOrWhiteSpace(ServerDefaultFrom)
                    ? ServerDefaultFrom
                    : payload.FromEmailAddress;

                var message = new MailMessage
                {
                    Body = payload.Body,
                    Subject = payload.Subject,
                    From = new MailAddress(fromEmailAddress),
                    IsBodyHtml = payload.IsBodyHtml,
                };

                if (payload.AlternateViews.Count > 0)
                {
                    foreach (var alternateView in payload.AlternateViews)
                    {
                        message.AlternateViews.Add(alternateView);
                    }
                }

                if (payload.To != null && payload.To.Any())
                {
                    foreach (var s in payload.To)
                        message.To.Add(s);
                }

                if (payload.CC != null && payload.CC.Any())
                {
                    foreach (var s in payload.CC)
                        message.CC.Add(s);
                }

                if (payload.BCC != null && payload.BCC.Any())
                {
                    foreach (var s in payload.BCC)
                        message.Bcc.Add(s);
                }

                if (payload.ReplyToList != null && payload.ReplyToList.Any())
                    foreach (var item in payload.ReplyToList)
                        message.ReplyToList.Add(item);

                if (payload.Headers != null && payload.Headers.HasKeys())
                    message.Headers.Add(payload.Headers);

                if (payload.Attachments != null)
                    foreach (var attachment in payload.Attachments)
                        message.Attachments.Add(attachment);


                mailClient.Send(message);
            }
        }

        private void ValidatePayload( EmailPayload payload )
        {
            if( payload == null )
                throw new ArgumentNullException( "payload" );

            if( string.IsNullOrEmpty( payload.FromEmailAddress ) )
                throw new InvalidOperationException( "FromEmailAddress is a required field." );

            if( ( payload.To == null || !payload.To.Any() ) && ( payload.CC == null || !payload.CC.Any() ) && ( payload.BCC == null || !payload.BCC.Any() ) )
                throw new InvalidOperationException( "At least one email address must be in the To, CC or BCC fields." );
        }
    }
}