using System.Runtime.Serialization;

namespace ACTransit.Contracts.Data.Customer.Feedback
{
    /// <summary>
    ///     Ask a Question form to be submitted to API
    /// </summary>
    [DataContract]
    public class AskForm : Form
    {
        protected const string AdditionalFormatString = "&feedback_action={0}";

        public AskForm()
        {
            ActionEnum = ActionEnum.ask;
        }

        public override string Serialize()
        {
            return base.Serialize() + string.Format(AdditionalFormatString, ActionEnum);
        }
    }
}