using System.Threading.Tasks;
using ACTransit.Contracts.Data.Common;
using ACTransit.Contracts.Data.Common.PublicSite;
using ACTransit.Contracts.Data.Customer.Feedback;
using ACTransit.Framework.Infrastructure.FileSystem;
using ACTransit.CusRel.Public.API.Controllers.Api;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ACTransit.CusRel.Public.Domain.Tests.Controllers
{
    [TestClass]
    public class FeedbackControllerTest
    {
        private FeedbackController feedback;

        private RequestState GetRequestState()
        {
            return new RequestState
            {
                ContentSearchReplaceList = SearchReplaceList.Get(@"C:\Projects\ACTransit.Projects\trunk\ACTransit.Mobile\ACTransit.CusRel.Public.API\Docs\ContentSearchReplaceFile.txt")
            };
        }

        private void initFeedback()
        {
            feedback = new FeedbackController();
            feedback.Prepare(GetRequestState());
        }


        [TestInitialize]
        public void InitializeForTest()
        {
            initFeedback();
        }

        [TestMethod]
        public void FeedbackLastestTest()
        {
            // Act
            var call = feedback.Ask(
                new AskForm
                {
                    Contact = new Contact
                    {
                        FirstName = "Bob", 
                        LastName = "Smith", 
                        EmailAddress = "bob@smith.com"
                    },
                    Comments = "This is a test.  Please ignore."
                });
            Assert.IsNotNull(call);
        }

        [TestMethod]
        public void FeedbackLostFoundTest()
        {
            var call = feedback.LostFound(
                new LostFoundForm
                {
                    Contact = new Contact
                    {
                        FirstName = "Bob",
                        LastName = "Smith",
                        EmailAddress = "bob@smith.com"
                    },
                    Comments = "This is a test.  Please ignore.",
                    LostItem = new LostItem
                    {
                        Category = "Bags and Luggage",
                        Type = "Lunchbox"

                    }
                });
            Assert.IsNotNull(call);
        }

    }
}
