using System.Collections.Generic;
using System.Threading.Tasks;
using ACTransit.Contracts.Data.Common;
using ACTransit.Contracts.Data.Common.PublicSite;
using ACTransit.Contracts.Data.Schedules.Booking;
using ACTransit.Framework.Infrastructure.FileSystem;
using ACTransit.CusRel.Public.API.Controllers.Api;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ACTransit.CusRel.Public.Domain.Tests.Controllers
{
    [TestClass]
    public class ScheduleControllerTest
    {
        private ScheduleController controller;

        private RequestState GetRequestState()
        {
            return new RequestState
            {
            };
        }

        [TestInitialize]
        public void InitializeForTest()
        {
            controller = new ScheduleController();
            controller.Prepare(GetRequestState());
        }


        [TestMethod]
        public void NearestStopsTest()
        {
            // Act
            var call = controller.NearbyStops(new decimal(37.8054133), new decimal(-122.2707171), 2);
            Assert.IsNotNull(call);
        }
    }
}
