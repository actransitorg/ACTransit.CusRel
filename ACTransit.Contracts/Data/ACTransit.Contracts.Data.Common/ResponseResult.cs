using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ACTransit.Contracts.Data.Common
{
    /// <summary>
    ///     Response result, including OK, StatusCode and list of Errors
    /// </summary>
    [DataContract]
    public class ResponseResult
    {
        /// <summary>
        ///     Is request and result valid and successful?
        /// </summary>
        [DataMember]
        public bool OK { get; set; }

        /// <summary>
        ///     Status code of result
        /// </summary>
        [DataMember]
        public int StatusCode { get; set; }

        /// <summary>
        ///     List of errors
        /// </summary>
        [DataMember]
        public List<string> Errors { get; set; }

        /// <summary>
        ///     Details related to result
        /// </summary>
        [DataMember]
        public object Details { get; set; }

        public ResponseResult() { }

        public ResponseResult(Exception e)
        {
            Errors = new List<string> {e.Message};
            var innerE = e.InnerException;
            while (innerE != null)
            {
                Errors.Add(innerE.Message);
                innerE = innerE.InnerException;
            }
        }
    }
}