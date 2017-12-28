using System;
using ACTransit.Contracts.Data.CusRel.ReportContract.Params;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ACTransit.CusRel.Tests.Repositories
{
    [TestClass]
    public class ReportRepositoryTests
    {
        private readonly CusRel.Repositories.ReportRepository reportRepository;

        public ReportRepositoryTests()
        {
            reportRepository = new CusRel.Repositories.ReportRepository();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            var path = AppDomain.CurrentDomain.BaseDirectory;
            path = path.Substring(0, path.IndexOf("ACTransit.CusRel", StringComparison.Ordinal)) + @"ACTransit.CusRel\ACTransit.CusRel\App_Data";
            AppDomain.CurrentDomain.SetData("DataDirectory", path);
        }

        //[TestMethod]
        //public void AssignedToReportTest()
        //{
        //    var result = reportRepository.AssignedToReport(new AssignedToReportParams {AssignedTo = "AYOUNG"});
        //    Assert.IsNotNull(result);
        //}

        [TestMethod]
        public void GroupContactReportTest()
        {
            var result = reportRepository.ForActionReport(new ForActionReportParams { GroupContact = "K. Hayward Supt Div 6" });
            Assert.IsNotNull(result);
        }
    }
}
