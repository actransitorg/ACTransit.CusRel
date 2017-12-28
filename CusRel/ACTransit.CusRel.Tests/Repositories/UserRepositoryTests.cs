using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ACTransit.CusRel.Tests.Repositories
{
    [TestClass]
    public class UserRepositoryTests
    {
        private readonly CusRel.Repositories.UserRepository userRepository;

        public UserRepositoryTests()
        {
            userRepository = new CusRel.Repositories.UserRepository();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            var path = AppDomain.CurrentDomain.BaseDirectory;
            path = path.Substring(0, path.IndexOf("ACTransit.CusRel", StringComparison.Ordinal)) + @"ACTransit.CusRel\ACTransit.CusRel\App_Data";
            AppDomain.CurrentDomain.SetData("DataDirectory", path);
        }

        [TestMethod]
        public void GetUserTest()
        {
            var result = userRepository.GetUser("KBAKAR");
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetOperatorTest()
        {
            var result = userRepository.GetOperator("042507");
            Assert.AreEqual(result.OK, true);
            Assert.IsNotNull(result.Operator);
            Assert.AreEqual(result.Operator.Badge, "042507");
        }

        [TestMethod]
        public void GetSpecificOperatorTest()
        {
            var result = userRepository.GetOperator("032094");
            Assert.AreEqual(result.OK, true);
            Assert.IsNotNull(result.Operator);
        }

        [TestMethod]
        public void GetOperator12345Test()
        {
            var result = userRepository.GetOperator("12345");
            Assert.AreEqual(result.OK, true);
            Assert.IsNull(result.Operator);
        }

        [TestMethod]
        public void SaveLastUserTest()
        {
            var getResult = userRepository.GetUser("KBAKAR");
            Assert.IsNotNull(getResult);
            Assert.AreEqual(getResult.OK, true);
            getResult.User.Email += "1";
            var saveResult = userRepository.SaveUser(getResult.User);
            Assert.IsNotNull(saveResult);
            Assert.AreEqual(saveResult.OK, true);
        }    
    }
}
