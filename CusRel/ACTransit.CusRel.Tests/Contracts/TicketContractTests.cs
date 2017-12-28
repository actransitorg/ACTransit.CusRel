using ACTransit.Contracts.Data.CusRel.TicketContract;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ACTransit.CusRel.Tests.Contracts
{
    [TestClass]
    public class TicketContractTests
    {
        [TestMethod]
        public void ModifyingReasonsTest()
        {
            var ticket = new Ticket();
            ticket.ReasonCode1 = "1";
            Assert.AreEqual(ticket.ReasonCode1, "1");
            Assert.AreEqual(ticket.ReasonCode2, null);
            Assert.AreEqual(ticket.Reasons.Count, 1);
            ticket.ReasonCode2 = "2";
            Assert.AreEqual(ticket.ReasonCode1, "1");
            Assert.AreEqual(ticket.ReasonCode2, "2");
            Assert.AreEqual(ticket.Reasons.Count, 2);
            ticket.ReasonCode1 = null;
            Assert.AreEqual(ticket.ReasonCode1, null);
            Assert.AreEqual(ticket.ReasonCode2, "2");
            Assert.AreEqual(ticket.Reasons.Count, 2);
            ticket.ReasonCode2 = null;
            Assert.AreEqual(ticket.ReasonCode1, null);
            Assert.AreEqual(ticket.ReasonCode2, null);
            Assert.AreEqual(ticket.Reasons.Count, 2);
            ticket.ReasonCode2 = "2";
            Assert.AreEqual(ticket.ReasonCode1, null);
            Assert.AreEqual(ticket.ReasonCode2, "2");
            Assert.AreEqual(ticket.Reasons.Count, 2);
            ticket.ReasonCode2 = null;
            Assert.AreEqual(ticket.ReasonCode1, null);
            Assert.AreEqual(ticket.ReasonCode2, null);
            Assert.AreEqual(ticket.Reasons.Count, 2);
        }
    }
}
