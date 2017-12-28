using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using ACTransit.Framework.Extensions;

namespace ACTransit.Framework.Notification
{
    public class SmsEmailService
    {
        public string ServerAddress { get; private set; }

        public int ServerPort { get; set; }

        public string ServerLogin { get; set; }

        public string ServerPassword { get; set; }

        public bool? ServerUseSsl { get; set; }

        public string ServerDefaultFrom { get; set; }

        public SmsEmailService() { }

        public SmsEmailService(string smtpServerAddress)
        {
            ServerAddress = smtpServerAddress;
        }

        public SmsEmailService(string smtpServerAddress, int smtpServerPort, string smtpServerLogin, string smtpServerPassword, bool? smtpServerUseSsl, string smtpServerDefaultFrom)
        {
            ServerAddress = smtpServerAddress;
            ServerPort = smtpServerPort;
            ServerLogin = smtpServerLogin;
            ServerPassword = smtpServerPassword;
            ServerUseSsl = smtpServerUseSsl;
            ServerDefaultFrom = smtpServerDefaultFrom;
        }
        
        public void Send(SmsPayload payload)
        {
            var emailService = new SMTPEmailService(ServerAddress, ServerPort, ServerLogin, ServerPassword, ServerUseSsl, ServerDefaultFrom);
            var pl = payload.AsEmailPayload();
            emailService.Send(pl);
        }
    }
}