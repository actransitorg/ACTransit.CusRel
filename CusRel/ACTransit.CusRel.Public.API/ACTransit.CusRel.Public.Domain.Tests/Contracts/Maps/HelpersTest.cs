using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ACTransit.Contracts.Data.Schedules.PublicSite;

namespace ACTransit.CusRel.Public.Domain.Tests.Contracts.Maps
{
    [TestClass]
    public class HelpersTest
    {
        [TestMethod]
        public void StopTimeParseTest()
        {
            Assert.AreEqual(null, "   ".ToDateTime());
            Assert.AreEqual(null, "-".ToDateTime());
            Assert.AreEqual(DateTime.Parse("12:44 pm"), "1244p".ToDateTime());
            Assert.AreEqual(DateTime.Parse("3:40 am"), "340a".ToDateTime());
            Assert.AreEqual(DateTime.Parse("1:10 pm").ToString(), "110p".ToDateTime().ToString());
        }
    }
}
