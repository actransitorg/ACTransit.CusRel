using Microsoft.VisualStudio.TestTools.UnitTesting;
using ACTransit.CusRel.Controllers;
using Newtonsoft.Json;

namespace ACTransit.CusRel.Tests.Controllers
{
    [TestClass]
    public class UserControllerTest
    {
        [TestMethod]
        public void GetOperator()
        {
            // Arrange
            var controller = new UserController();

            // Act
            var result = controller.GetOperator("1");

            var json = JsonConvert.SerializeObject(result);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetEmployees()
        {
            // Arrange
            var controller = new UserController();

            // Act
            var result = controller.GetEmployees("43164", null, null);

            var json = JsonConvert.SerializeObject(result);

            // Assert
            Assert.IsNotNull(result);
        }    
    }
}
