using System.Runtime.Serialization;
using ACTransit.Contracts.Data.CusRel.TicketContract;
using Newtonsoft.Json;

namespace ACTransit.Contracts.Data.CusRel.LookupContract.Result
{
    [DataContract] //, JsonObject(MemberSerialization.OptIn)
    public class OperatorResult : Common.Result
    {
        [DataMember]
        public Operator Operator { get; set; }
    }
}