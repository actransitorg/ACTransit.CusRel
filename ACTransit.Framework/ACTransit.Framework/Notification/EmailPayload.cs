using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Mail;

namespace ACTransit.Framework.Notification
{
    public class EmailPayload
    {
        public string FromEmailAddress { get; set; }

        public List<string> To { get; set; }

        public List<string> BCC { get; set; }

        public List<string> CC { get; set; } 

        public string Body { get; set; }
        
        public string Subject { get; set; }

        //public List<byte[]> Attachments { get; set;  }
        public List<Attachment> Attachments { get; set; }

        public NameValueCollection Headers { get; set; }

        public MailAddressCollection ReplyToList { get; set; }

        /// <summary>
        /// Can be set in config file's mailSettings section
        /// </summary>
        public string LoginName { get; set; }

        /// <summary>
        /// Can be set in config file's mailSettings section
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Nullable as this is a transport feature
        /// </summary>
        public bool? UseSSL { get; set; }

        public bool IsBodyHtml { get; set; }

        public List<AlternateView> alternateViews = new List<AlternateView>();
        public List<AlternateView> AlternateViews { get { return alternateViews; } set { alternateViews = value; } }

        public EmailPayload()
        {
            To = new List<string>();
            BCC = new List<string>();
            CC = new List<string>();
        }

        public const string DefaultEmail = @"From: your.company Website - do not reply <do-not-reply@your.company.dns>";
    }
}
