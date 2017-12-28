using System;
using System.Text;
using System.Collections.Generic;
using ACTransit.Contracts.Data.Common.PublicSite;
using ACTransit.Entities.CustomerRelations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ACTransit.CusRel.Tests.ServiceHost
{
    /// <summary>
    /// Summary description for MainStartTest
    /// </summary>
    [TestClass]
    public class RuntimeTests
    {
        public RuntimeTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void MainStartTest()
        {
            CusRel.ServiceHost.Program.Main(new []{"--start"});
            Assert.IsTrue(CusRel.ServiceHost.Program.IsRunning);
        }

        internal class VirusScanRequestChild : CusRel.ServiceHost.VirusScan.Request.VirusScanRequest
        {
            public VirusScanRequestChild(): base(new CusRel.ServiceHost.Request.RequestState { })
            {               
            }

            public tblAttachmentsTemp Get()
            {
                return GetAttachmentTemp(new tblAttachmentsTemp {Id = 1});
            }

            public bool Save(tblAttachmentsTemp item)
            {
                return UpdateAttachmentTemp(item);
            }
        }


        [TestMethod]
        public void VirusScanRequestTest()
        {
            var req = new VirusScanRequestChild();
            var attach1 = req.Get();
            Assert.IsTrue(!string.IsNullOrEmpty(attach1.FileName));
            Assert.IsTrue(attach1.ScanId != "123");
            attach1.ScanId = "123";
            var result2 = req.Save(attach1);
            Assert.IsTrue(result2);
            var attach2 = req.Get();
            Assert.IsTrue(!string.IsNullOrEmpty(attach2.FileName));
            Assert.IsTrue(attach2.ScanId == "123");
            attach2.ScanId = null;
            var result3 = req.Save(attach2);
            Assert.IsTrue(result3);
        }
    }
}
