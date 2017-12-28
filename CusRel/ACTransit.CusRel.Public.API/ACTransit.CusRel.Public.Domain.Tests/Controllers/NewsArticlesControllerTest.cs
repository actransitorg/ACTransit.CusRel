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
    public class NewsArticlesControllerTest
    {
        private NewsController articles;
        private NewsController article;

        private RequestState GetRequestState()
        {
            return new RequestState
            {
                ContentSearchReplaceList = SearchReplaceList.Get(@"C:\Projects\ACTransit.Projects\trunk\ACTransit.Mobile\ACTransit.CusRel.Public.API\Docs\ContentSearchReplaceFile.txt")
            };
        }

        private void initArticles()
        {
            articles = new NewsController();
            articles.Prepare(GetRequestState());
        }

        private void initArticle()
        {
            article = new NewsController();
            article.Prepare(GetRequestState());
        }

        [TestInitialize]
        public void InitializeForTest()
        {
            //initArticles();
            initArticle();
        }

        [TestMethod]
        public async Task ArticlesLastestTest()
        {
            // Act
            var id = "%c2%a1el-15-de-diciembre-llega-un-servicio-mejor-a-la-zona-sur-del-condado-de-alameda";
            var call = await article.Article(id) as OkNegotiatedContentResult<Post>;
            Assert.IsNotNull(call);
            var result = call.Content;
            Assert.IsNotNull(result);
        }
    }
}
