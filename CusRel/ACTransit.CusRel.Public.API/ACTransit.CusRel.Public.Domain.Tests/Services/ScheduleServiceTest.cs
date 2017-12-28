using System.Linq;
using ACTransit.Contracts.Data.Common.PublicSite;
using ACTransit.CusRel.Public.Domain.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ACTransit.CusRel.Public.Domain.Tests.Services
{
    [TestClass]
    public class ScheduleServiceTest
    {
        private readonly RequestState requestState;

        [TestMethod]
        public void TestStop511()
        {
            var result = new ScheduleService(requestState).Stop511(55121);
            Assert.IsNotNull(result.Result);
            Assert.IsTrue(result.Result.StopId.Length > 0);
        }

        [TestMethod]
        public void NearestStops()
        {
            var result = new ScheduleService(requestState).NearestStops(new decimal(37.87515475), new decimal(-122.2938555));
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Any());
        }
    }
}
