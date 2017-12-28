using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Runtime.Serialization;

namespace ACTransit.Contracts.Data.Customer.Feedback
{
    /// <summary>
    ///     Feedback form to be submitted to API
    /// </summary>
    [DataContract]
    public abstract class Form
    {
        private const string SerializeFormatString =
            "first_name={1}&last_name={2}&address={3}&city={4}&state={5}&zip_code={6}&home_phone_ac={7}&home_phone_3dig={8}&home_phone_4dig={9}&work_phone_ac={10}&work_phone_3dig={11}&work_phone_4dig={12}&fax_ac={13}&fax_3dig={14}&fax_4dig={15}&email={16}&incident_desc={17}&response_req=0&Submit=Submit&first_name_tp=&last_name_tp=&start_address=&start_city=&end_address=&end_city=&email_tp=&travel_mon_s={18}&travel_day_s={19}&travel_year_s={20}&service_type_s={21}&when_s=leave+at&hour_s=1&minutes_s=0&ampm_s=AM&travel_mon_r=&travel_day_r=&travel_year_r=&service_type_r=&when_r=&hour_r=&minutes_r=&ampm_r=&trip_other_details=";

        protected const string InYourOwnWords = @"===================== IN YOUR OWN WORDS =====================";

        private const string AsEmailFormatString = @"===================== FEEDBACK =================

Feedback ID:  {0}
Date/Time:  {1}
Name:  {2}
Address:  {3}
City:  {4}
State:  {5}
Zip:  {6}
Home Phone:  {7}
Work Phone:  {8}
Cell:  {9}
Email:  {10}

{11}
Description:  {12}

Request a Response?:  {13}

";

        /// <summary>
        ///     Type of Feedback
        /// </summary>
        [DataMember]
        public ActionEnum ActionEnum { get; set; }

        /// <summary>
        ///     Customer Contact info
        /// </summary>
        [DataMember]
        public Contact Contact { get; set; }

        /// <summary>
        ///     Customer Comments
        /// </summary>
        [Required(ErrorMessage = "Comments field is empty.")]
        //[MaxLength(8000, ErrorMessage = "Comments field must be less then or equal to 8000 characters.")]  TODO: Add back for move to .NET 4.5
        [DataMember]
        public string Comments { get; set; }

        /// <summary>
        ///     Request a response from AC Transit?
        /// </summary>
        [DataMember]
        public bool RequestResponse { get; set; }

        /// <summary>
        /// File (attachment)
        /// </summary>
        [DataMember]
        public File File { get; set; }

        public virtual string Serialize()
        {
            if (Contact == null)
                throw new ArgumentNullException("Contact", "Contact cannot be null.");
            if (Contact.HomePhone == null || Contact.HomePhone.Length != 10)
                throw new ValidationException("HomePhone is invalid.");
            if (Contact.WorkPhone == null || Contact.WorkPhone.Length != 10)
                throw new ValidationException("WorkPhone is invalid.");
            if (Contact.MobilePhone == null || Contact.MobilePhone.Length != 10)
                throw new ValidationException("HomePhone is invalid.");
            return string.Format(SerializeFormatString, null, Uri.EscapeUriString(Contact.FirstName),
                Uri.EscapeUriString(Contact.LastName), Uri.EscapeUriString(Contact.Address),
                Uri.EscapeUriString(Contact.City), Uri.EscapeUriString(Contact.State),
                Uri.EscapeUriString(Contact.ZipCode),
                Uri.EscapeUriString(Contact.HomePhone.Substring(0, 3)),
                Uri.EscapeUriString(Contact.HomePhone.Substring(3, 3)),
                Uri.EscapeUriString(Contact.HomePhone.Substring(6, 4)),
                Uri.EscapeUriString(Contact.WorkPhone.Substring(0, 3)),
                Uri.EscapeUriString(Contact.WorkPhone.Substring(3, 3)),
                Uri.EscapeUriString(Contact.WorkPhone.Substring(6, 4)),
                Uri.EscapeUriString(Contact.MobilePhone.Substring(0, 3)),
                Uri.EscapeUriString(Contact.MobilePhone.Substring(3, 3)),
                Uri.EscapeUriString(Contact.MobilePhone.Substring(6, 4)),
                Uri.EscapeUriString(Contact.EmailAddress), Uri.EscapeUriString(Comments),
                DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Year, "Weekday");
        }

        public virtual string AsEmail(string FeedbackId, string EmailHeader = null)
        {
            return string.Format(AsEmailFormatString, FeedbackId,
                DateTime.Now,
                Contact.FirstName + " " + Contact.LastName,
                Contact.Address,
                Contact.City,
                Contact.State,
                Contact.ZipCode,
                Contact.HomePhone,
                Contact.WorkPhone,
                Contact.MobilePhone,
                Contact.EmailAddress,
                InYourOwnWords,
                Comments,
                RequestResponse ? "Yes" : "No");
        }
    }
}