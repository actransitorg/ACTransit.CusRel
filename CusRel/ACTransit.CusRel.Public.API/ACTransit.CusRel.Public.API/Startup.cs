using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(ACTransit.CusRel.Public.API.Startup))]

namespace ACTransit.CusRel.Public.API
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
