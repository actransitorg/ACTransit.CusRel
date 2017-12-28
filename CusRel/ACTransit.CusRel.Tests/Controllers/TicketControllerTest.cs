using Microsoft.VisualStudio.TestTools.UnitTesting;
using ACTransit.CusRel.Controllers;
using Newtonsoft.Json;

namespace ACTransit.CusRel.Tests.Controllers
{
    [TestClass]
    public class TicketControllerTest
    {
        [TestMethod]
        public void GetLast()
        {
            // Arrange
            var controller = new TicketController();

            // Act
            var result = controller.GetLast();

            var json = JsonConvert.SerializeObject(result);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Search()
        {
            // Arrange
            var controller = new TicketController();

            // Act
            var result = controller.Search();

            var json = JsonConvert.SerializeObject(result);

            // Assert
            Assert.IsNotNull(result);
        }    
    }
}
