using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Configuration;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;
using ACTransit.Framework.Notification;

namespace ACTransit.Framework.Web.Infrastructure
{
    public static class WebMailer
    {
        private static readonly SmtpSection SmtpSec;
        private static readonly bool EmailEnabled;
        private static readonly WebLogger WebLogger;
        static WebMailer()
        {
            SmtpSec = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
            EmailEnabled = MailSettings.EmailEnabled;
            WebLogger = new WebLogger(typeof(WebMailer));
        }
        public static void EmailError(Exception ex)
        {
            try
            {
                if (!EmailEnabled) return;

                string mailSubject = "ACTransit Application Exception - " + HttpContext.Current.Request.Url.Host + " - " + DateTime.Now;
                var lstEmailTo = MailSettings.ExceptionAlertToEmails.ToList();
                var lstEmailCc = MailSettings.ExceptionAlertCcEmails.ToList();
                var lstEmailBCc = MailSettings.ExceptionAlertBccEmails.ToList();

                var sbMailContent = new StringBuilder();
                var sbHeader = new StringBuilder();
                var sbItem = new StringBuilder();
                var sbBlankItem = new StringBuilder();

                sbHeader.Append("   <tr>");
                sbHeader.Append("       <td colspan='3'><h5><u>$Header</u></h5></td>");
                sbHeader.Append("   </tr>");

                sbItem.Append("   <tr>");
                sbItem.Append("       <td style='width:16%;' valign='top'><b>$ItemHeader</b></td>");
                sbItem.Append("       <td valign='top'>: </td>");
                sbItem.Append("       <td valign='top'>$ItemValue</td>");
                sbItem.Append("   </tr>");

                sbBlankItem.Append("   <tr>");
                sbBlankItem.Append("       <td colspan='3'>&nbsp;</td>");
                sbBlankItem.Append("   </tr>");

                sbMailContent.Append("<table cellpadding='10pt' cellspacing='10pt' style='font-family:Verdana; font-size:9pt;'>");

                //********************
                //Application Details
                //********************

                //Application Details Header
                sbMailContent.Append(sbHeader);
                sbMailContent.Replace("$Header", "Application Details");

                //Host
                sbMailContent.Append(sbItem);
                sbMailContent.Replace("$ItemHeader", "Host");
                sbMailContent.Replace("$ItemValue", HttpContext.Current.Request.Url.Host);

                //URL
                sbMailContent.Append(sbItem);
                sbMailContent.Replace("$ItemHeader", "URL");
                sbMailContent.Replace("$ItemValue", HttpContext.Current.Request.Url.OriginalString);

                //URL Referrer
                sbMailContent.Append(sbItem);
                sbMailContent.Replace("$ItemHeader", "URL Referrer");
                sbMailContent.Replace("$ItemValue", (HttpContext.Current.Request.UrlReferrer == null ? "" : HttpContext.Current.Request.UrlReferrer.OriginalString));

                //Browser
                sbMailContent.Append(sbItem);
                sbMailContent.Replace("$ItemHeader", "Browser");
                sbMailContent.Replace("$ItemValue", HttpContext.Current.Request.Browser.Browser);

                //User
                sbMailContent.Append(sbItem);
                sbMailContent.Replace("$ItemHeader", "User");
                sbMailContent.Replace("$ItemValue", HttpContext.Current.User.Identity.Name);

                //Date
                sbMailContent.Append(sbItem);
                sbMailContent.Replace("$ItemHeader", "Date");
                sbMailContent.Replace("$ItemValue", DateTime.Now.ToString());

                //Blank Row
                sbMailContent.Append(sbBlankItem);

                //****************
                //Session Details
                //****************

                //Session Details Header
                sbMailContent.Append(sbHeader);
                sbMailContent.Replace("$Header", "Session Details");

                //Session Items
                if (HttpContext.Current.Session != null)
                {
                    foreach (string strSessionKey in HttpContext.Current.Session.Keys)
                    {
                        sbMailContent.Append(sbItem);
                        sbMailContent.Replace("$ItemHeader", strSessionKey);
                        sbMailContent.Replace("$ItemValue", (HttpContext.Current.Session[strSessionKey] == null ? "" : HttpContext.Current.Session[strSessionKey].ToString()));
                    }
                }

                //Blank Row
                sbMailContent.Append(sbBlankItem);

                //*************
                //Form Details
                //*************

                //Form Details Header
                sbMailContent.Append(sbHeader);
                sbMailContent.Replace("$Header", "Form Details");

                //Form Collection

                var ignoreKeys = new[] { "password", "pwd" };
                var keys =
                    (from object key in HttpContext.Current.Request.Form.Keys
                     where ignoreKeys.All(k => key.ToString().IndexOf(k, StringComparison.OrdinalIgnoreCase) == -1)
                     select key.ToString()).ToList();

                foreach (var strFormKey in keys)
                {
                    sbMailContent.Append(sbItem);
                    sbMailContent.Replace("$ItemHeader", strFormKey);
                    sbMailContent.Replace("$ItemValue", (HttpContext.Current.Request.Form[strFormKey] ?? ""));
                }

                //Blank Row
                sbMailContent.Append(sbBlankItem);

                //******************
                //Exception Details
                //******************
                sbMailContent.Append(sbHeader);
                sbMailContent.Replace("$Header", "Exception Details");

                //Message
                sbMailContent.Append(sbItem);
                sbMailContent.Replace("$ItemHeader", "Message");
                sbMailContent.Replace("$ItemValue", ex.Message);

                //Source
                sbMailContent.Append(sbItem);
                sbMailContent.Replace("$ItemHeader", "Source");
                sbMailContent.Replace("$ItemValue", ex.Source);

                //TargetSite
                sbMailContent.Append(sbItem);
                sbMailContent.Replace("$ItemHeader", "TargetSite");
                sbMailContent.Replace("$ItemValue", ex.TargetSite.ToString());

                //StackTrace
                sbMailContent.Append(sbItem);
                sbMailContent.Replace("$ItemHeader", "StackTrace");
                sbMailContent.Replace("$ItemValue", ex.StackTrace);

                //HelpLink
                sbMailContent.Append(sbItem);
                sbMailContent.Replace("$ItemHeader", "HelpLink");
                sbMailContent.Replace("$ItemValue", ex.HelpLink);

                //Blank Row
                sbMailContent.Append(sbBlankItem);

                //************************
                //Inner Exception Details
                //************************
                if (ex.InnerException != null)
                {
                    sbMailContent.Append(sbHeader);
                    sbMailContent.Replace("$Header", "Inner Exception Details");

                    //Message
                    sbMailContent.Append(sbItem);
                    sbMailContent.Replace("$ItemHeader", "Message");
                    sbMailContent.Replace("$ItemValue", ex.InnerException.Message);

                    //Source
                    sbMailContent.Append(sbItem);
                    sbMailContent.Replace("$ItemHeader", "Source");
                    sbMailContent.Replace("$ItemValue", ex.InnerException.Source);

                    //TargetSite
                    sbMailContent.Append(sbItem);
                    sbMailContent.Replace("$ItemHeader", "TargetSite");
                    sbMailContent.Replace("$ItemValue", ex.InnerException.TargetSite.ToString());

                    //StackTrace
                    sbMailContent.Append(sbItem);
                    sbMailContent.Replace("$ItemHeader", "StackTrace");
                    sbMailContent.Replace("$ItemValue", ex.InnerException.StackTrace);

                    //HelpLink
                    sbMailContent.Append(sbItem);
                    sbMailContent.Replace("$ItemHeader", "HelpLink");
                    sbMailContent.Replace("$ItemValue", ex.InnerException.HelpLink);

                    //Blank Row
                    sbMailContent.Append(sbBlankItem);
                }

                sbMailContent.Append("</table>");

                // Send a Exception Mail
                var emailService = new SMTPEmailService(SmtpSec.Network.Host);
                emailService.Send(new EmailPayload
                {
                    FromEmailAddress = SmtpSec.From,
                    To = lstEmailTo,
                    CC = lstEmailCc,
                    BCC = lstEmailBCc,
                    Subject = mailSubject,
                    Body = sbMailContent.ToString(),
                    IsBodyHtml = true
                });
            }
            catch (Exception e)
            {
                WebLogger.Error(e.Message);
            }
        }
    }
}