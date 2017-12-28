using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace ACTransit.Contracts.Data.CusRel.TicketContract
{
    [DataContract] //, JsonObject(MemberSerialization.OptIn)
    public class ComplaintCode
    {
        private static readonly Regex SplitRegex = new Regex(@"(.{1,5})\.\s(.*)", RegexOptions.Compiled);

        public ComplaintCode(string value)
        {
            Fill(value);
        }

        public ComplaintCode()
        {
        }

        [Display(Name = "Code"), DataMember]
        public string Code { get; set; }

        [Display(Name = "Dept"), DataMember]
        public string Group { get; set; }

        [Display(Name = "Category"), DataMember]
        public string Category { get; set; }

        [Display(Name = "Description"), DataMember]
        public string Description { get; set; }

        [Display(Name = "Days Past Due"), DataMember]
        public int? PastDueDays { get; set; }

        [Display(Name = "Order"), DataMember]
        public int? Order { get; set; }

        [Display(Name = "IsVisible"), DataMember]
        public bool? IsVisible { get; set; }

        [Display(Name = "Value"), DataMember]
        public string Value
        {
            set { Fill(value); }
            get { return ToString(); }
        }

        private void Fill(string value)
        {
            if (value == null) return;
            if (!SplitRegex.IsMatch(value)) return;
            MatchCollection matches = SplitRegex.Matches(value);
            Code = matches[0].Groups[1].Value;
            Description = matches[0].Groups[2].Value;
        }

        public override string ToString()
        {
            if (Code == null) return null;
            return Description == null ? Code : string.Format("{0}. {1}", Code, Description);
        }
    }
}