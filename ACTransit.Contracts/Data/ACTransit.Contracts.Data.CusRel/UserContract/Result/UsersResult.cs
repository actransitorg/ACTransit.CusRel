using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace ACTransit.Contracts.Data.CusRel.UserContract.Result
{
    [DataContract] //, JsonObject(MemberSerialization.OptIn)
    public class UsersResult : Common.Result
    {
        [DataMember]
        public List<User> Users { get; set; }

        public User GetUser(string Id)
        {
            return Users.FirstOrDefault(gc => string.Equals(gc.Id, Id, StringComparison.OrdinalIgnoreCase)) ??
                   Users.FirstOrDefault(gc => string.Equals(gc.Username, Id, StringComparison.OrdinalIgnoreCase));
        }
    }
}