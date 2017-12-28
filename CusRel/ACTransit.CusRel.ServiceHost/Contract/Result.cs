using System;
using System.Runtime.Serialization;

namespace ACTransit.CusRel.ServiceHost.Contract
{
    [DataContract]
    public class Result
    {
        public Result(string Id, string Code, string Message, string Warning)
        {
            this.Id = Id;
            this.Code = Code;
            this.Message = Message;
            this.Warning = Warning;
            CreatedAt = DateTime.Now;
        }

        [DataMember]
        public string Id { get; protected set; }
        [DataMember]
        public string Code { get; protected set; }
        [DataMember]
        public string Message { get; protected set; }
        [DataMember]
        public string Warning { get; protected set; }
        [DataMember]
        public DateTime CreatedAt { get; protected set; }
    }
}
