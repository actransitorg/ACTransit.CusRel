using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace ACTransit.Contracts.Data.CusRel.TicketContract.Result
{
    [DataContract] //, JsonObject(MemberSerialization.OptIn)
    public class TicketsResult : Common.Result
    {
        [DataMember]
        public List<Ticket> Tickets { get; set; }
    }
}