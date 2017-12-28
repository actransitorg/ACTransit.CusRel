using System;
using ACTransit.CusRel.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace ACTransit.CusRel.Tests.Services
{
    [TestClass]
    public class MapsSchedulesServiceTests
    {
        private readonly ServicesProxy servicesProxy = new ServicesProxy();
        private readonly ACTransit.Contracts.Data.Common.PublicSite.RequestState MapsSchedulesRequestState = new ACTransit.Contracts.Data.Common.PublicSite.RequestState();

        public MapsSchedulesServiceTests()
        {
            servicesProxy.MapsScheduleService = new MapsScheduleService(servicesProxy, MapsSchedulesRequestState);
        }

        [TestInitialize]
        public void TestInitialize()
        {
            var path = AppDomain.CurrentDomain.BaseDirectory;
            path = path.Substring(0, path.IndexOf("ACTransit.CusRel", StringComparison.Ordinal)) + @"ACTransit.CusRel\ACTransit.CusRel\App_Data";
            AppDomain.CurrentDomain.SetData("DataDirectory", path);
        }

        [TestMethod]
        public void GetLineSchedule()
        {
            var result = servicesProxy.MapsScheduleService.GetRouteInfo(null, DateTime.MinValue);
            var json = JsonConvert.SerializeObject(result);
            Assert.IsNotNull(result);
        }

    }
}
