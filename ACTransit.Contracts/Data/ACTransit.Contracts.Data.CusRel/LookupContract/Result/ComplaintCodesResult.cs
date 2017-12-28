using System.Collections.Generic;
using System.Runtime.Serialization;
using ACTransit.Contracts.Data.CusRel.TicketContract;
using Newtonsoft.Json;

namespace ACTransit.Contracts.Data.CusRel.LookupContract.Result
{
    [DataContract] //, JsonObject(MemberSerialization.OptIn)
    public class ComplaintCodesResult : Common.Result
    {
        [DataMember]
        public List<ComplaintCode> ComplaintCodes { get; set; }
    }
}