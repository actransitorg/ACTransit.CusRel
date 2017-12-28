using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace ACTransit.Contracts.Data.CusRel.TicketContract.Result
{
    [DataContract] //, JsonObject(MemberSerialization.OptIn)
    public class TicketResult : Common.Result
    {
        public TicketResult()
        {
        }

        public TicketResult(Common.Result Result, Ticket Ticket)
        {
            MergeResults(Result);
            this.Ticket = Ticket;
        }

        [DataMember]
        public Ticket Ticket { get; set; }
    }
}