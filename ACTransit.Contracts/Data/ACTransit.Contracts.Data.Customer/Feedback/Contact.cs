using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace ACTransit.Contracts.Data.Customer.Feedback
{
    /// <summary>
    ///     User submitted contact information
    /// </summary>
    [DataContract]
    public class Contact
    {
        /// <summary>
        ///     First name
        /// </summary>
        [Required(ErrorMessage = "First name field is empty.")]
        [DataMember]
        public string FirstName { get; set; }

        /// <summary>
        ///     Last name
        /// </summary>
        [Required(ErrorMessage = "Last name field is empty.")]
        [DataMember]
        public string LastName { get; set; }

        /// <summary>
        ///     Address
        /// </summary>
        [DataMember]
        public string Address { get; set; }

        /// <summary>
        ///     City
        /// </summary>
        [DataMember]
        public string City { get; set; }

        /// <summary>
        ///     State (two letters)
        /// </summary>
        [DataMember]
        public string State { get; set; }

        /// <summary>
        ///     Zip code
        /// </summary>
        [DataMember]
        public string ZipCode { get; set; }

        /// <summary>
        ///     Mobile phone (10 digits)
        /// </summary>
        //[Required(ErrorMessage = "Mobile phone field is not complete.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Mobile phone field must only contain numbers.")]
        [DataMember]
        public string MobilePhone { get; set; }

        /// <summary>
        ///     Home phone (10 digits)
        /// </summary>
        //[Required(ErrorMessage = "Home phone field is not complete.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Home phone field must only contain numbers.")]
        [DataMember]
        public string HomePhone { get; set; }

        /// <summary>
        ///     Work phone (10 digits)
        /// </summary>
        //[Required(ErrorMessage = "Work phone field is not complete.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Work phone field must only contain numbers.")]
        [DataMember]
        public string WorkPhone { get; set; }

        /// <summary>
        ///     Email address
        /// </summary>
        [Required(ErrorMessage = "Email field is empty.")]
        //[EmailAddress(ErrorMessage = "Email field is not valid.")]  TODO: Add back for move to .NET 4.5
        [DataMember]
        public string EmailAddress { get; set; }

        [JsonIgnore]
        public string SmtpEmail
        {
            get { return string.Format("From: {0} {1} <{2}>", FirstName, LastName, EmailAddress); }
        }
    }
}