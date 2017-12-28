using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using ACTransit.Contracts.Data.CusRel.UserContract;
using Newtonsoft.Json;

namespace ACTransit.Contracts.Data.CusRel.TicketContract
{
    [DataContract] //, JsonObject(MemberSerialization.OptIn)
    public class ChangeHistory
    {
        [Key, DataMember]
        public int Id { get; set; }

        [DataMember]
        public DateTime? ChangeAt { get; set; }

        [DataMember]
        public User ChangeBy { get; set; }

        [DataMember]
        public string Action { get; set; }

        [DataMember]
        public string TableName { get; set; }

        [DataMember]
        public string ColumnName { get; set; }

        [DataMember]
        public string OldValue { get; set; }

        [DataMember]
        public string NewValue { get; set; }
    }
}