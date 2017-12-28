using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace ACTransit.Contracts.Data.Customer.Feedback
{
    /// <summary>
    ///     Employee (driver)
    /// </summary>
    [DataContract]
    public class Employee
    {
        /// <summary>
        ///     Badge Number
        /// </summary>
        [RegularExpression(@"^\d{1,5}$",
            ErrorMessage = "Employee badge field must only contain numbers and 5 or less in length.")]
        [DataMember]
        public string Badge { get; set; }

        /// <summary>
        ///     Description of employee
        /// </summary>
        [DataMember]
        public string Description { get; set; }
    }
}