using System.Runtime.Serialization;

namespace ACTransit.Contracts.Data.Customer.Feedback
{
    /// <summary>
    ///     Make a Suggestion form to be submitted to API
    /// </summary>
    [DataContract]
    public class SuggestionForm : Form
    {
        protected const string AdditionalFormatString = "&feedback_action={0}";

        public SuggestionForm()
        {
            ActionEnum = ActionEnum.suggestion;
        }

        public override string Serialize()
        {
            return base.Serialize() + string.Format(AdditionalFormatString, ActionEnum);
        }
    }
}