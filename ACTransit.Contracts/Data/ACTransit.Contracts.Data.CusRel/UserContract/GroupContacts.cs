using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace ACTransit.Contracts.Data.CusRel.UserContract
{
    [DataContract] //, JsonObject(MemberSerialization.OptIn)
    public class GroupContact
    {
        public static readonly string UnassignedValue = "5. Unassigned";
        private static readonly Regex SplitRegex = new Regex(@"(.{1,5})\.\s(.*)", RegexOptions.Compiled);

        public GroupContact(string value)
        {
            Fill(value);
        }

        public GroupContact()
        {
        }

        [Display(Name = "Code"), DataMember]
        public string Code { get; set; }

        [Display(Name = "Description"), DataMember]
        public string Description { get; set; }

        [Display(Name = "Email"), DataMember]
        public string Email { get; set; }

        [Display(Name = "Value"), DataMember]
        public string Value
        {
            set { Fill(value); }
            get { return ToString(); }

        }

        [Display(Name = "Order"), DataMember]
        public int? Order { get; set; }

        [Display(Name = "IsVisible"), DataMember]
        public bool? IsVisible { get; set; }

        private void Fill(string value)
        {
            if (value == null) return;
            if (!SplitRegex.IsMatch(value)) return;
            MatchCollection matches = SplitRegex.Matches(value);
            Code = matches[0].Groups[1].Value;
            Description = matches[0].Groups[2].Value;
        }

        public bool IsUnassigned => Value == UnassignedValue;

        public override string ToString()
        {
            return Code == null 
                ? ""
                : (Description == null 
                    ? Code 
                    : string.Format("{0}. {1}", Code, Description));
        }
    }
}