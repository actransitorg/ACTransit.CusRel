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
    public class ArticlesControllerTest
    {
        private ArticlesController articles;
        private ArticleController article;

        private RequestState GetRequestState()
        {
            return new RequestState
            {
                ContentSearchReplaceList = SearchReplaceList.Get(@"C:\Projects\ACTransit.Projects\trunk\ACTransit.Mobile\ACTransit.CusRel.Public.API\Docs\ContentSearchReplaceFile.txt")
            };
        }

        private void initArticles()
        {
            articles = new ArticlesController();
            articles.Prepare(GetRequestState());
        }

        private void initArticle()
        {
            article = new ArticleController();
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
            var call = await articles.Latest() as OkNegotiatedContentResult<List<Post>>;
            Assert.IsNotNull(call);
            var result = call.Content;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task ContactUsArticleTest()
        {
            // Act
            var call = await article.Index("contact-us") as OkNegotiatedContentResult<List<Post>>;
            Assert.IsNotNull(call);
            var result = call.Content;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task UnicodeArticleTest()
        {
            // Act
            var call = await article.Index(@"%e5%b7%b4%e5%a3%ab%e7%a5%a8%e5%83%b9%e8%88%87%e4%bd%bf%e7%94%a8%e8%b3%87%e6%a0%bc-%e6%96%bc2014%e5%b9%b47%e6%9c%881%e6%97%a5%e8%b5%b7") as OkNegotiatedContentResult<Post>;
            Assert.IsNotNull(call);
            var result = call.Content;
            Assert.IsNotNull(result);
        }
    }
}
