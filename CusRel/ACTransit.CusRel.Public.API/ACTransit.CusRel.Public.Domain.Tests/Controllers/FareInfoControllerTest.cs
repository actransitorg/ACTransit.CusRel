using System.Threading.Tasks;
using ACTransit.Contracts.Data.Common.PublicSite;
using ACTransit.Contracts.Data.WordPress;
using ACTransit.Framework.Infrastructure.FileSystem;
using ACTransit.CusRel.Public.API.Controllers.Api;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Http.Results;

namespace ACTransit.CusRel.Public.Domain.Tests.Controllers
{
    [TestClass]
    public class FareInfoControllerTest
    {
        private FareInfoController fare;

        private RequestState GetRequestState()
        {
            return new RequestState
            {
                ContentSearchReplaceList = SearchReplaceList.Get(@"P:\ACTransit.Projects\trunk\ACTransit.Mobile\ACTransit.CusRel.Public.API\Docs\ContentSearchReplaceFile.txt")
            };
        }

        private void init()
        {
            fare = new FareInfoController();
            fare.Prepare(GetRequestState());
        }

        [TestInitialize]
        public void InitializeForTest()
        {
            init();
        }

        [TestMethod]
        public async Task FareInfoTest()
        {
            // Act
            var call = await fare.List() as OkNegotiatedContentResult<FaresPost>;
            Assert.IsNotNull(call);
            var result = call.Content;
            Assert.IsNotNull(result);
        }
    }
}
