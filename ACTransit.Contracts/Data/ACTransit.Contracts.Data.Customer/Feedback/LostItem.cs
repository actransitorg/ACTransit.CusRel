using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace ACTransit.Contracts.Data.Customer.Feedback
{
    /// <summary>
    ///     Lost item details
    /// </summary>
    [DataContract]
    public class LostItem
    {
        /// <summary>
        ///    Category of lost item
        /// </summary>
        [RegularExpression(@"^.{0,50}$", ErrorMessage = "Category field must only contain numbers 50 or less in length.")]
        [DataMember]
        public string Category { get; set; }

        /// <summary>
        ///    Type of lost item
        /// </summary>
        [RegularExpression(@"^.{0,50}$", ErrorMessage = "Type field must only contain numbers 50 or less in length.")]
        [DataMember]
        public string Type { get; set; }

    }
}