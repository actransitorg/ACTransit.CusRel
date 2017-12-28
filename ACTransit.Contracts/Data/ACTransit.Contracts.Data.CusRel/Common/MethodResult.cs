using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace ACTransit.Contracts.Data.CusRel.Common
{
    [DataContract] //, JsonObject(MemberSerialization.OptIn)
    public class MethodResult
    {
        public MethodResult()
        {
            OK = true;
        }

        [DataMember]
        public bool OK { get; set; }

        [DataMember]
        public Int64 ID { get; set; }

        [DataMember]
        public Exception Exception { get; set; }

        [DataMember]
        public string Message { get; set; }

        public void SetFail(Exception e, string message = null)
        {
            OK = false;
            Exception = e;
            Message = message;
        }
    }
}