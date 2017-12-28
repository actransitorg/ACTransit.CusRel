using System;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using ACTransit.CusRel.Repositories;
using ACTransit.CusRel.Security;
using ACTransit.Framework.Web.Infrastructure;

namespace ACTransit.CusRel
{
    public class MvcApplication : HttpApplication
    {
        private log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            PassiveSessionConfiguration.ConfigureSessionCache(new EFTokenCacheRepository());
            CacheWarmup.Startup();

            //ValueProviderFactories.Factories.Remove(ValueProviderFactories.Factories.OfType<JsonValueProviderFactory>().FirstOrDefault());
            //ValueProviderFactories.Factories.Add(new JsonDotNetValueProviderFactory()); 
        }

        public override void Init()
        {
            PassiveSessionConfiguration.EnableSlidingSessionExpirations();
            base.Init();
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var exception = Server.GetLastError();
            if (exception == null) return;
            //_logger.Fatal(exception.Message, exception);

            //Send an email if the error is anything other than Not Found.
            if (exception is HttpException && ((HttpException)exception).GetHttpCode() == (int)HttpStatusCode.NotFound)
                return;
            log.Error(exception.ToString());
            //WebMailer.EmailError(exception);
        }

    }
}
