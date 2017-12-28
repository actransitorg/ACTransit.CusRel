using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace ACTransit.Contracts.Data.CusRel.UserContract.Result
{
    [DataContract] //, JsonObject(MemberSerialization.OptIn)
    public class UserResult : Common.Result
    {
        [DataMember]
        public User User { get; set; }

        [DataMember]
        public string[] Roles { get; set; }

        public static UserResult AsFailed()
        {
            var userResult = new UserResult();
            userResult.SetFail("No User");
            return userResult;
        }
    }
}