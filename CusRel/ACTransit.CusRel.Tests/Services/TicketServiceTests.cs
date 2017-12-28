using System;
using System.Linq;
using ACTransit.Contracts.Data.CusRel.TicketContract;
using ACTransit.Contracts.Data.CusRel.TicketContract.Params;
using ACTransit.CusRel.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace ACTransit.CusRel.Tests.Services
{
    [TestClass]
    public class TicketServiceTests
    {
        private readonly ServicesProxy servicesProxy = new ServicesProxy();

        public TicketServiceTests()
        {
            servicesProxy.TicketService = new TicketService(servicesProxy);
        }

        [TestInitialize]
        public void TestInitialize()
        {
            var path = AppDomain.CurrentDomain.BaseDirectory;
            path = path.Substring(0, path.IndexOf("ACTransit.CusRel", StringComparison.Ordinal)) + @"ACTransit.CusRel\ACTransit.CusRel\App_Data";
            AppDomain.CurrentDomain.SetData("DataDirectory", path);
        }

        [TestMethod]
        public void GetEmptyTicketTest()
        {
            var result = servicesProxy.TicketService.GetEmptyTicket();
            var json = JsonConvert.SerializeObject(result);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.OK);

            result = servicesProxy.TicketService.GetEmptyTicket(Ticket.EmptyTicketEnum.NewTicket);
            json = JsonConvert.SerializeObject(result);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.OK);        
        }

        [TestMethod]
        public void EmptySearchTicketsTest()
        {
            var result = servicesProxy.TicketService.Search(new TicketSearchParams());
            Assert.IsNotNull(result);
            Assert.IsTrue(result.OK);
            Assert.IsNotNull(result.Tickets);
            Assert.AreNotEqual(result.Tickets.Count, 0);
        }

        
        [TestMethod]
        public void DateSearchTicketsTest()
        {
            var result = servicesProxy.TicketService.Search(new TicketSearchParams
            {
                ReceivedAtFrom = DateTime.Parse("2014-06-10"),
            });
            Assert.IsNotNull(result);
            Assert.IsTrue(result.OK);
            Assert.IsNotNull(result.Tickets);
            Assert.AreNotEqual(result.Tickets.Count, 0);
        }

        [TestMethod]
        public void ExplicitEmptySearchTicketsTest()
        {
            var Criteria = new TicketSearchParams
            {
                Id = 0,
                IncidentAtTo = null,
                IncidentAtFrom = null,
                ReceivedAtFrom = null,
                ReceivedAtTo = null
            };
            var result = servicesProxy.TicketService.Search(Criteria);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.OK);
            Assert.IsNotNull(result.Tickets);
            Assert.AreNotEqual(result.Tickets.Count, 0);
        }

        [TestMethod]
        public void GetTicketSearchFieldsTest()
        {
            var result = servicesProxy.TicketService.GetTicketSearchFields();
            var json = JsonConvert.SerializeObject(result);
            var a = json;
            Assert.IsNotNull(result);
            Assert.IsTrue(result.OK);
        }

        [TestMethod]
        public void AddLinkedTicketTest()
        {
            servicesProxy.TicketService.DetachGet = true;
            var ticketResult = servicesProxy.TicketService.GetLast();
            Assert.IsNotNull(ticketResult);
            Assert.IsTrue(ticketResult.OK);
            Assert.IsNotNull(ticketResult.Ticket);
            var ticket = ticketResult.Ticket;

            servicesProxy.TicketService.DetachGet = true;
            var linkedTicketResult = servicesProxy.TicketService.GetPrevious(ticket.Id);
            Assert.IsNotNull(linkedTicketResult);
            Assert.IsTrue(linkedTicketResult.OK);
            Assert.IsNotNull(linkedTicketResult.Ticket);
            var linkedTicket = linkedTicketResult.Ticket;

            servicesProxy.TicketService.DetachGet = false;
            ticket.LinkedTickets.Add(linkedTicket);
            var changeResult = servicesProxy.TicketService.AddOrUpdateLinkedTicket(ticket);
            Assert.AreEqual(changeResult.OK, true);
        }

        [TestMethod]
        public void DeleteLinkedTicketTest()
        {
            servicesProxy.TicketService.DetachGet = true;
            var ticketResult = servicesProxy.TicketService.GetLast();
            Assert.IsNotNull(ticketResult);
            Assert.IsTrue(ticketResult.OK);
            Assert.IsNotNull(ticketResult.Ticket);
            var ticket = ticketResult.Ticket;

            servicesProxy.TicketService.DetachGet = true;
            var linkedTicketResult = servicesProxy.TicketService.GetPrevious(ticket.Id);
            Assert.IsNotNull(linkedTicketResult);
            Assert.IsTrue(linkedTicketResult.OK);
            Assert.IsNotNull(linkedTicketResult.Ticket);
            var linkedTicket = linkedTicketResult.Ticket;

            servicesProxy.TicketService.DetachGet = false;
            var changeResult = servicesProxy.TicketService.DeleteLinkedTicket(ticket, linkedTicket);
            Assert.AreEqual(changeResult.OK, true);
        }

    }
}
