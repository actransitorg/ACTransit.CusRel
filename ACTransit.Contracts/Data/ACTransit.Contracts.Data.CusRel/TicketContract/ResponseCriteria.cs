using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace ACTransit.Contracts.Data.CusRel.TicketContract
{
    [DataContract] //, JsonObject(MemberSerialization.OptIn)
    public class ResponseCriteria
    {
        [DataMember]
        public bool? HasRequestedResponse { get; set; }

        [DataMember]
        public ResponseCriteriaVia Via { get; set; }
    }

    [DataContract]
    public enum ResponseCriteriaVia
    {
        None,
        Letter,
        Phone,
        Email
    }
}