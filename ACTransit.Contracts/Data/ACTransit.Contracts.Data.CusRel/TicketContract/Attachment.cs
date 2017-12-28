using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using ACTransit.Contracts.Data.CusRel.UserContract;
using Newtonsoft.Json;

namespace ACTransit.Contracts.Data.CusRel.TicketContract
{
    [DataContract] //, JsonObject(MemberSerialization.OptIn)
    public class Attachment
    {
        [Key, DataMember]
        public int Id { get; set; }

        [Required, DataMember]
        public string Filename { get; set; }

        [DataMember]
        public byte[] Data { get; set; }

        [DataMember]
        public string Base64Data { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public DateTime? UploadedAt { get; set; }

        [DataMember]
        public User UploadedBy { get; set; }

        [DataMember]
        public bool ShouldDelete { get; set; }

        [DataMember]
        public bool IsNew { get; set; }
    }
}