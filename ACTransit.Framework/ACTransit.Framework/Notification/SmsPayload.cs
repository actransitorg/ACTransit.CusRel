using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Net.Mail;
using ACTransit.Framework.Extensions;

namespace ACTransit.Framework.Notification
{
    [AttributeUsage(AttributeTargets.All)]
    public class SmsEmailAttribute : Attribute
    {
        public string Format { get; private set; }

        public SmsEmailAttribute(string format)
        {
            Format = format;
        }
    }

    [AttributeUsage(AttributeTargets.All)]
    public class MmsEmailAttribute : Attribute
    {
        public string Format { get; private set; }

        public MmsEmailAttribute(string format)
        {
            Format = format;
        }
    }


    public enum TelcomProvider
    {
        [Description("Alltel Wireless")]
        [SmsEmail("[Phone]@message.alltel.com"), MmsEmail("[Phone]@mms.alltelwireless.com")]
        Alltel,

        [Description("AT&T")]
        [SmsEmail("[Phone]@txt.att.net"), MmsEmail("[Phone]@mms.att.net")]
        Att,

        [Description("Boost Mobile")]
        [SmsEmail("[Phone]@myboostmobile.com"), MmsEmail("[Phone]@myboostmobile.com")]
        BoostMobile,

        [Description("Cricket Wireless")]
        [MmsEmail("[Phone]@mms.cricketwireless.net")]
        CricketWireless,

        [Description("ProjectFi")]
        [MmsEmail("[Phone]@msg.fi.google.com")]
        ProjectFi,

        [Description("RepublicWireless")]
        [SmsEmail("[Phone]@text.republicwireless.com")]
        RepublicWireless,

        [Description("Sprint")]
        [SmsEmail("[Phone]@messaging.sprintpcs.com"), MmsEmail("[Phone]@pm.sprint.com")]
        Sprint,

        [Description("TMobile")]
        [SmsEmail("[Phone]@tmomail.net"), MmsEmail("[Phone]@tmomail.net")]
        TMobile,

        [Description("U.S. Cellular")]
        [SmsEmail("[Phone]@email.uscc.net"), MmsEmail("[Phone]@mms.uscc.net")]
        UsCellular,

        [Description("Verizon")]
        [SmsEmail("[Phone]@vtext.com"), MmsEmail("[Phone]@vzwpix.com")]
        Verizon,

        [Description("Virgin Mobile")]
        [SmsEmail("[Phone]@vmobl.com"), MmsEmail("[Phone]@vmpix.com")]
        VirginMobile
    }

    public enum BodyFormat
    {
        Sms,
        Mms
    }

    public class PhoneProvider
    {
        public string PhoneNumber { get; private set; }

        public TelcomProvider Provider { get; private set; }

        public BodyFormat BodyFormat { get; private set; }

        public string To
        {
            get
            {
                var emailAddress = BodyFormat == BodyFormat.Sms
                    ? Provider.GetSingleAttribute<SmsEmailAttribute>().Format
                    : Provider.GetSingleAttribute<MmsEmailAttribute>().Format;
                return emailAddress.Replace("[Phone]", PhoneNumber);
            }
        }

        public PhoneProvider(string phoneNumber, TelcomProvider provider, BodyFormat bodyFormat = BodyFormat.Sms)
        {
            Init(phoneNumber, provider, bodyFormat);
        }

        public PhoneProvider(string phoneNumber, string provider)
        {
            var _provider = (TelcomProvider)Enum.Parse(typeof(TelcomProvider), provider);
            Init(phoneNumber, _provider);
        }
        public PhoneProvider(string phoneNumber, string provider, BodyFormat bodyFormat ):this(phoneNumber, (TelcomProvider)Enum.Parse(typeof(TelcomProvider), provider),bodyFormat){}

        public List<string> ProviderNames()
        {
            return Enum.GetNames(typeof(TelcomProvider)).ToList();
        }

        private void Init(string phoneNumber, TelcomProvider provider, BodyFormat bodyFormat = BodyFormat.Sms)
        {
            if (phoneNumber.Length != 10)
                throw new Exception("phoneNumber must be length 10");
            PhoneNumber = phoneNumber;
            Provider = provider;
            BodyFormat = bodyFormat;
        }
    }


    public class SmsPayload
    {
        public string FromEmailAddress { get; set; }

        public List<PhoneProvider> To { get; set; }

        public List<PhoneProvider> BCC { get; set; }

        public List<PhoneProvider> CC { get; set; } 

        public string Body { get; set; }
        
        public string Subject { get; set; }

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

        private List<AlternateView> _alternateViews;

        public List<AlternateView> AlternateViews
        {
            get
            {
                return _alternateViews ?? (_alternateViews = new List<AlternateView>());
                
            }
            set
            {
                _alternateViews = value;                
            }
        } 

        public SmsPayload()
        {
            To = new List<PhoneProvider>();
            BCC = new List<PhoneProvider>();
            CC = new List<PhoneProvider>();
        }

        public string DefaultEmail = @"From: your.company Website - do not reply <do-not-reply@your.company.dns>";

        public EmailPayload AsEmailPayload()
        {
            
            var toList = To.Select(item => item.To).ToList();
            var bccList = BCC.Select(item => item.To).ToList();
            var ccList = CC.Select(item => item.To).ToList();
            return new EmailPayload
            {
                FromEmailAddress = FromEmailAddress,
                To = toList,
                BCC = bccList,
                CC = ccList,
                Body = Body,
                Subject = Subject,
                Attachments = Attachments,
                Headers = Headers,
                ReplyToList = ReplyToList,
                LoginName = LoginName,
                Password = Password,
                UseSSL = UseSSL,
                IsBodyHtml = IsBodyHtml,
                AlternateViews = AlternateViews
            };
        }
    }
}
