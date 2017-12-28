using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace ACTransit.Contracts.Data.Customer.Feedback
{
    /// <summary>
    /// File (attachment) details
    /// </summary>
    [DataContract]
    public class File
    {
        /// <summary>
        /// File name
        /// </summary>
        [MaxLength(128)]
        [DataMember]
        public string Filename { get; set; }

        /// <summary>
        /// Length of file
        /// </summary>
        [DataMember]
        public int Filesize { get; set; }

        /// <summary>
        /// MIME Content Type 
        /// </summary>
        [MaxLength(128)]
        [DataMember]
        public string ContentType { get; set; }

        /// <summary>
        /// Base 64 encoded file data
        /// </summary>
        [DataMember]
        public string Base64Data { get; set; }
    }
}