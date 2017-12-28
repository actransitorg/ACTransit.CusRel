using System.Threading.Tasks;
using ACTransit.CusRel.Services;

namespace ACTransit.CusRel
{
    public class CacheWarmup
    {
        public static async void Startup()
        {
            await Task.Run(() => initialize());
        }

        private static void initialize()
        {
            var servicesProxy = new ServicesProxy
            {
                UserService = new UserService(),
                SettingsService = new SettingsService(),
            };

            // load users
            var usersResult = servicesProxy.UserService.GetUsers();
            //var routeResult = servicesProxy.MapsScheduleService.RouteInfo(null);
        }
    }
}