using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace ACTransit.Contracts.Data.CusRel.TicketContract.Result
{
    [DataContract] //, JsonObject(MemberSerialization.OptIn)
    public class AttachmentResult : Common.Result
    {
        public AttachmentResult()
        {
        }

        public AttachmentResult(Common.Result Result, Attachment Attachment, Ticket Ticket)
        {
            MergeResults(Result);
            this.Attachment = Attachment;
            this.Ticket = Ticket;
        }

        [DataMember]
        public Attachment Attachment { get; set; }

        [DataMember]
        public Ticket Ticket { get; set; }
    }
}