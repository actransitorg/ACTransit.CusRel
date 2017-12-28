using System.Collections.Generic;
using System.Threading.Tasks;
using ACTransit.Contracts.Data.Common;
using ACTransit.Contracts.Data.Common.PublicSite;
using ACTransit.Contracts.Data.WordPress;
using ACTransit.Framework.Infrastructure.FileSystem;
using ACTransit.CusRel.Public.API.Controllers.Api;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Http.Results;

namespace ACTransit.CusRel.Public.Domain.Tests.Controllers
{
    [TestClass]
    public class LineDescriptionsControllerTest
    {
        private LineDescriptionController controller;

        private RequestState GetRequestState()
        {
            return new RequestState
            {
                ContentSearchReplaceList = SearchReplaceList.Get(@"C:\Projects\ACTransit.Projects\trunk\ACTransit.Mobile\ACTransit.CusRel.Public.API\Docs\ContentSearchReplaceFile.txt")
            };
        }

        private void initLineDescription()
        {
            controller = new LineDescriptionController();
            controller.Prepare(GetRequestState());
        }

        [TestInitialize]
        public void InitializeForTest()
        {
            initLineDescription();
        }

        [TestMethod]
        public async Task LineDescriptionsInfoTest()
        {
            // Act
            var call = await controller.ListDescriptions() as OkNegotiatedContentResult<Post>;
            Assert.IsNotNull(call);
            var result = call.Content;
            Assert.IsNotNull(result);
        }

    }
}
