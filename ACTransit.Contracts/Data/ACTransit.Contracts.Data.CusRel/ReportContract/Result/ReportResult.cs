using System.Collections.Generic;
using System.Runtime.Serialization;
using ACTransit.Contracts.Data.CusRel.TicketContract;
using Newtonsoft.Json;

namespace ACTransit.Contracts.Data.CusRel.ReportContract.Result
{
    [DataContract] //, JsonObject(MemberSerialization.OptIn)
    public class ReportResult : Common.Result
    {
        [DataMember]
        public List<Ticket> Tickets { get; set; }
    }
}