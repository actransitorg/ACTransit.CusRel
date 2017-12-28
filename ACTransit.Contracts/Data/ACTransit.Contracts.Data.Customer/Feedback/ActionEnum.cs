using System.ComponentModel;
using System.Runtime.Serialization;

namespace ACTransit.Contracts.Data.Customer.Feedback
{
    /// <summary>
    ///     Type of Feedback
    /// </summary>
    [DataContract]
    public enum ActionEnum
    {
        [Description("Incident Report")] report = 0,

        [Description("Question")] ask = 1,

        [Description("Suggestion")] suggestion = 2,

        [Description("Lost and Found")] lostfound = 3,
    }
}