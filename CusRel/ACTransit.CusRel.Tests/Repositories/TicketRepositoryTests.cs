using System;
using System.Collections.Generic;
using System.Linq;
using ACTransit.Contracts.Data.Common.PublicSite;
using ACTransit.Contracts.Data.CusRel.ReportContract.Params;
using ACTransit.Contracts.Data.CusRel.TicketContract;
using ACTransit.Contracts.Data.CusRel.TicketContract.Params;
using ACTransit.Contracts.Data.CusRel.UserContract;
using ACTransit.CusRel.Repositories.DAL;
using ACTransit.CusRel.Repositories.Search;
using ACTransit.Framework.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace ACTransit.CusRel.Tests.Repositories
{
    [TestClass]
    public class TicketRepositoryTests
    {
        private readonly CusRel.Repositories.TicketRepository ticketRepository;
        private readonly ACTransit.Contracts.Data.CusRel.Common.RequestState requestState;

        public TicketRepositoryTests()
        {
            ticketRepository = new CusRel.Repositories.TicketRepository(new CusRelDbContext());
            requestState = new ACTransit.Contracts.Data.CusRel.Common.RequestState();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            var path = AppDomain.CurrentDomain.BaseDirectory;
            path = path.Substring(0, path.IndexOf("ACTransit.CusRel", StringComparison.Ordinal)) + @"ACTransit.CusRel\ACTransit.CusRel\App_Data";
            AppDomain.CurrentDomain.SetData("DataDirectory", path);
        }

        [TestMethod]
        public void GetComplaintCodes()
        {
            var codes = ticketRepository.GetComplaintCodes();
            var json = JsonConvert.SerializeObject(codes);
            Assert.IsNotNull(codes);
        }

        [TestMethod]
        public void GetGroupContacts()
        {
            var groups = ticketRepository.GetGroupContacts();
            var json = JsonConvert.SerializeObject(groups);
            Assert.IsNotNull(groups);
        }

        [TestMethod]
        public void GetAddressStaticListsTest()
        {
            var lists = ticketRepository.GetAddressStaticLists();
            Assert.IsNotNull(lists);
        }

        [TestMethod]
        public void GetTest()
        {
            var ticket = ticketRepository.Get(362493);
            Assert.IsNotNull(ticket);
        }

        [TestMethod]
        public void GetLastTest()
        {
            var ticket = ticketRepository.GetLast();
            Assert.IsNotNull(ticket);
        }

        [TestMethod]
        public void GetPrevious40Test()
        {
            var ticket = ticketRepository.GetLast();
            foreach (var count in Enumerable.Range(0, 40))
            {
                ticket = ticketRepository.GetPrevious(ticket.Ticket.Id);
                Assert.IsNotNull(ticket);                
            }
            Assert.IsNotNull(ticket);
        }

        [TestMethod]
        public void SearchTicketResultTest()
        {
            var searchCriteria = new TicketSearchParams
            {
                Id = 362526,
                Status = TicketStatus.New,
                Priority = TicketPriority.Normal,
                Source = new TicketSource
                {
                    ReceivedAt = DateTime.Parse("2014-06-11 18:46:10.663"),
                    ReceivedBy = new User(Id: "WEB", Username: "accbtona"),
                    Via = TicketSourceVia.WEB,
                    FeedbackId = 26887
                },
                Contact = new Contact
                {
                    Name = new Name
                    {
                        First = "Marlena",
                        Last = "Hanlon"
                    },
                    Address = new Address
                    {
                        Addr1 = null,
                        Addr2 = null,
                        City = "Oakland",
                        State = "CA",
                        ZipCode = null
                    },
                    Email = "m@marlenahanlon.com",
                    Phone = new Phone
                    {
                        Number = null,
                        Kind = PhoneKind.Home
                    },
                    Status = null
                },
                IsAdaComplaint = false,
                IsTitle6 = false,
                Incident = new Incident
                {
                    IncidentAt = DateTime.Parse("2014-06-11 18:15:00.000"),
                    VehicleNumber = "1601",
                    Route = "1",
                    Location = "Telegraph at 34th", // partial text
                    Destination = "none",
                    City = null,
                    Division = "GO"
                },
                Operator = new Operator
                {
                    Badge = null,
                    Description = "Picture available" // partial text
                },
                LostItem = new LostItem
                {
                    Category = "None",
                    Type = "None",
                },
                Reasons = new List<string> { "11. HAZARDOUS OPERATION", "16. CONDUCT/DISCOURTESY" },
                Comments = "since the LAW is that when passing bicyclists",  // partial text
                ResponseCriteria = new ResponseCriteria
                {
                    HasRequestedResponse = true,
                    Via = ResponseCriteriaVia.Email
                },
                Resolution = new Resolution
                {
                    Action = null,
                    Comment = null,
                    ResolvedAt = null
                },
                Assignment = new Assignment
                {
                    GroupContact = new GroupContact("K. Hayward Supt Div 6"),
                    Employee = new User(Id: null),
                },
                UpdatedAt = DateTime.Parse("2014-06-11 19:32:26.000"),
                UpdatedBy = new User(Id: "ACCBTONA"),

                ReceivedAtFrom = DateTime.Parse("2014-06-10"),
                ReceivedAtTo = DateTime.Parse("2014-06-13"),
                IncidentAtFrom = DateTime.Parse("2014-06-11 18:14:00.000"),
                IncidentAtTo = DateTime.Parse("2014-06-11 18:16:00.000")
            };
            var tickets = searchCriteria.Search(requestState);
            Assert.IsNotNull(tickets);
            Assert.AreNotEqual(tickets.Count, 0);
        }

        [TestMethod]
        public void SearchTicketsResultTest()
        {
            var searchCriteria = new TicketSearchParams
            {
                ReceivedAtFrom = DateTime.Parse("2014-06-10"),
            };
            var tickets = searchCriteria.Search(requestState);
            Assert.IsNotNull(tickets);
            Assert.AreNotEqual(tickets.Count, 0);
        }

        [TestMethod]
        public void Title6SearchTicketsTest()
        {
            var maxSearchCount = requestState.MaxSearchCount;
            requestState.MaxSearchCount = 500;
            var searchCriteria = new TicketSearchParams
            {
                SearchAsUnion = true,
                ReasonCode1 = "Title VI",
                IsTitle6 = true,
                ReceivedAtFrom = DateTime.Now.AddMonths(-2),
            };
            var tickets = searchCriteria.Search(requestState);
            Assert.IsNotNull(tickets);
            Assert.AreNotEqual(tickets.Count, 0);
            Assert.AreNotEqual(tickets.Count, 500);
            requestState.MaxSearchCount = maxSearchCount;
        }

        [TestMethod]
        public void CreateTicketTest()
        {
            var ticket = new Ticket
            {
                Status = TicketStatus.New,
                Priority = TicketPriority.Normal,
                Source = new TicketSource
                {
                    ReceivedBy = new User(Id: "WEB", Username: "testuser1"),
                    Via = TicketSourceVia.WEB,
                    FeedbackId = new Random().Next()
                },
                Contact = new Contact
                {
                    Name = new Name
                    {
                        First = "Michael",
                        Last = "Paine"
                    },
                    Address = new Address
                    {
                        Addr1 = null,
                        Addr2 = null,
                        City = "Oakland",
                        State = "CA",
                        ZipCode = null
                    },
                    Email = "your.email@your.company.dns",
                    Phone = new Phone
                    {
                        Number = null,
                        Kind = PhoneKind.Home
                    },
                    Status = null
                },
                Attachments = new List<Attachment>
                {
                    new Attachment
                    {
                        Filename = "1.txt", 
                        Description = "data:text/plain;base64,LmR4LXRoZW1lLWdlbmVyaWMgLm5hdmJhci1sYXlvdXQgLmxheW91dC1oZWFkZXIgI25hdkJhciB7DQogICAgdG9wOiAwOw0KICAgIGhlaWdodDogNDZweDsNCn0NCg0KLmR4LXRoZW1lLWdlbmVyaWMgLm5hdmJhci1sYXlvdXQgLmxheW91dC1jb250ZW50IHsNCiAgICB0b3A6IDA7DQogICAgYm90dG9tOiA3NnB4Ow0KICAgIGJhY2tncm91bmQ6ICNmM2YzZjM7DQp9DQoNCi5keC10aGVtZS1nZW5lcmljIC5uYXZiYXItbGF5b3V0Lmhhcy10b29sYmFyIC5sYXlvdXQtY29udGVudCB7DQogICAgYm90dG9tOiA3NnB4Ow0KfQ0KDQouZHgtdGhlbWUtZ2VuZXJpYyAubmF2YmFyLWxheW91dC5oYXMtbmF2YmFyIC5sYXlvdXQtY29udGVudCB7DQogICAgdG9wOiA0NnB4Ow0KfQ0KDQouZHgtdGhlbWUtZ2VuZXJpYyAubmF2YmFyLWxheW91dCAubGF5b3V0LWZvb3RlciB7DQogICAgYm90dG9tOiAwOw0KICAgIGhlaWdodDogNzZweDsNCn0NCg0KLmR4LXRoZW1lLWdlbmVyaWMgLm5hdmJhci1sYXlvdXQuaGFzLXRvb2xiYXIgLmxheW91dC1mb290ZXIgew0KICAgIGRpc3BsYXk6IGJsb2NrOw0KfQ0KDQouZHgtdGhlbWUtZ2VuZXJpYyAubmF2YmFyLWxheW91dCAuZHgtdGFicy5keC1uYXZiYXIgew0KICAgIHBhZGRpbmc6IDA7DQp9DQoNCi5keC10aGVtZS1nZW5lcmljIC5uYXZiYXItbGF5b3V0IC52aWV3LWZvb3RlciB7DQogICAgcG9zaXRpb246IHN0YXRpYzsNCn0="                    
                    }
                },
                UpdatedBy = new User { Id = "mpaine" }
            };
            var result = ticketRepository.Create(ticket);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.OK, true);
        }

        [TestMethod]
        public void UpdateTicketTest()
        {
            ticketRepository.DetachGet = true;
            var result = ticketRepository.GetLast();
            Assert.AreEqual(result.OK, true);

            ticketRepository.DetachGet = false;
            result.Ticket.UpdatedAt = DateTime.Now;
            var result2 = ticketRepository.Update(result.Ticket);
            Assert.AreEqual(result2.OK, true);
        }

        [TestMethod]
        public void UpdateTicketContactHistoryManualTest()
        {
            ticketRepository.DetachGet = true;
            var result = ticketRepository.GetLast();
            Assert.AreEqual(result.OK, true);

            ticketRepository.DetachGet = false;
            result.Ticket.UpdatedAt = DateTime.Now;
            result.Ticket.UpdatedBy = new User(Id: "testuser");
            var comment = "Comment #" + new Random().Next();
            ResponseHistory rh;
            if (result.Ticket.ResponseHistory != null && result.Ticket.ResponseHistory.Count > 0)
                rh = result.Ticket.ResponseHistory.Last();
            else
            {
                if (result.Ticket.ResponseHistory == null)
                    result.Ticket.ResponseHistory = new List<ResponseHistory>();
                rh = new ResponseHistory();
                result.Ticket.ResponseHistory.Add(rh);
            }
            rh.Comment = comment;
            rh.ResponseBy = result.Ticket.UpdatedBy;
            var result2 = ticketRepository.Update(result.Ticket);
            Assert.AreEqual(result2.OK, true);
        }

        [TestMethod]
        public void UpdateTicketResearchHistoryManualTest()
        {
            ticketRepository.DetachGet = true;
            var result = ticketRepository.GetLast();
            Assert.AreEqual(result.OK, true);

            ticketRepository.DetachGet = false;
            result.Ticket.UpdatedAt = DateTime.Now;
            result.Ticket.UpdatedBy = new User(Id: "testuser");
            var comment = "Comment #" + new Random().Next();
            ResearchHistory rh;
            if (result.Ticket.ResearchHistory != null && result.Ticket.ResearchHistory.Count > 0)
                rh = result.Ticket.ResearchHistory.Last();
            else
            {
                if (result.Ticket.ResearchHistory == null)
                    result.Ticket.ResearchHistory = new List<ResearchHistory>();
                rh = new ResearchHistory();
                result.Ticket.ResearchHistory.Add(rh);
            }
            rh.Comment = comment;
            rh.ResearchedBy = result.Ticket.UpdatedBy;
            var result2 = ticketRepository.Update(result.Ticket);
            Assert.AreEqual(result2.OK, true);
        }


        [TestMethod]
        public void AddTicketAttachmentManualTest()
        {
            ticketRepository.DetachGet = true;
            var result = ticketRepository.GetLast();
            Assert.AreEqual(result.OK, true);

            ticketRepository.DetachGet = false;
            result.Ticket.UpdatedAt = DateTime.Now;
            result.Ticket.Attachments.Add(
                new Attachment
                {
                    Data = new byte[]{0,0,0},
                    Description = "test",
                    Filename = "test.bin",
                    UploadedAt = DateTime.Now
                });
            var result2 = ticketRepository.Update(result.Ticket);
            Assert.AreEqual(result2.OK, true);
        }

        [TestMethod]
        public void UpdateTicketAttachmentManualTest()
        {
            ticketRepository.DetachGet = true;
            var result = ticketRepository.GetLast();
            Assert.AreEqual(result.OK, true);

            ticketRepository.DetachGet = false;
            result.Ticket.UpdatedAt = DateTime.Now;
            Assert.IsTrue(result.Ticket.Attachments[0].Id > 100);
            result.Ticket.Attachments[0].UploadedAt = DateTime.Now;
            var result2 = ticketRepository.Update(result.Ticket);
            Assert.AreEqual(result2.OK, true);
        }

        [TestMethod]
        public void DeleteTicketAttachmenManualtTest()
        {
            ticketRepository.DetachGet = true;
            var result = ticketRepository.GetLast();
            Assert.AreEqual(result.OK, true);

            Assert.IsTrue(result.Ticket.Attachments[0].Id > 100);
            result.Ticket.Attachments[0].ShouldDelete = true;
            var result2 = ticketRepository.Update(result.Ticket);
            Assert.AreEqual(result2.OK, true);
        }

        [TestMethod]
        public void CheckDaysOpenTest()
        {
            ticketRepository.DetachGet = true;
            var result = ticketRepository.Get(362502);
            Assert.AreEqual(result.OK, true);
            Assert.AreEqual(result.Ticket.DaysOpen, 30);
        }

        //[TestMethod]
        //public void RouteInfoTest()
        //{
        //    var result = ticketRepository.RouteInfo("14");
        //    Assert.AreEqual(result.OK, true);
        //    Assert.IsNotNull(result.RouteInfo.Directions);
        //    Assert.IsNotNull(result.RouteInfo.Divisions);
        //}

        //[TestMethod]
        //public void RouteInfoAllTest()
        //{
        //    var result = ticketRepository.RouteInfo(null);
        //    Assert.AreEqual(result.OK, true);
        //    Assert.IsNotNull(result.RouteInfo.Directions);
        //    Assert.IsNotNull(result.RouteInfo.Divisions);
        //}

        [TestMethod]
        public void AddLinkedTicketManualTest()
        {
            ticketRepository.DetachGet = true;
            var result = ticketRepository.GetLast();
            Assert.AreEqual(result.OK, true);

            ticketRepository.DetachGet = false;
            result.Ticket.UpdatedAt = DateTime.Now;
            result.Ticket.LinkedTickets.Add(new Ticket { Id = result.Ticket.Id });
            var result2 = ticketRepository.Update(result.Ticket);
            Assert.AreEqual(result2.OK, true);
        }

        [TestMethod]
        public void CleanTicketTest()
        {
            var testString = "number is <span>510-307-1720<a target=\"_blank\" rel=\"nofollow\"><img alt=\"\"></a></span>.";
            ticketRepository.DetachGet = true;
            var result = ticketRepository.GetLast();
            Assert.AreEqual(result.OK, true);
            result.Ticket.Comments = testString;
            ticketRepository.PrepareTicket(result.Ticket);
            Assert.AreNotEqual(result.Ticket.Comments, testString);
        }

        [TestMethod]
        public void AddAttachmentTest()
        {
            ticketRepository.DetachGet = true;
            var result = ticketRepository.GetLast();
            Assert.AreEqual(result.OK, true);

            ticketRepository.DetachGet = false;
            var items = result.Ticket.Attachments;
            items.Add(new Attachment
            {
                Data = new byte[]{0,1,2,3,4,5,6,7,8,9}, 
                Id = 0, 
                Description = "Test", 
                Filename = "test.bin", 
                ShouldDelete = false,
                UploadedBy = new User(Id: "testuser")
            });
            var changeResult = ticketRepository.AddOrUpdateAttachment(result.Ticket);
            Assert.AreEqual(changeResult.OK, true);
        }

        [TestMethod]
        public void UpdateAttachmentTest()
        {
            ticketRepository.DetachGet = true;
            var result = ticketRepository.GetLast();
            Assert.AreEqual(result.OK, true);

            ticketRepository.DetachGet = false;
            var item = result.Ticket.Attachments.LastOrDefault();
            Assert.AreNotEqual(item, null);
            item.Filename = "test" + new Random().Next() + ".bin";
            var changeResult = ticketRepository.AddOrUpdateAttachment(result.Ticket);
            Assert.AreEqual(changeResult.OK, true);
        }

        [TestMethod]
        public void DeleteAttachmentTest()
        {
            ticketRepository.DetachGet = true;
            var result = ticketRepository.GetLast();
            Assert.AreEqual(result.OK, true);

            ticketRepository.DetachGet = false;
            var item = result.Ticket.Attachments.LastOrDefault();
            Assert.AreNotEqual(item, null);
            item.ShouldDelete = true;
            var changeResult = ticketRepository.AddOrUpdateAttachment(result.Ticket);
            Assert.AreEqual(changeResult.OK, true);
        }

        [TestMethod]
        public void AddLinkedTicketTest()
        {
            ticketRepository.DetachGet = true;
            var lastTicket = ticketRepository.GetLast();
            Assert.AreEqual(lastTicket.OK, true);
            ticketRepository.DetachGet = true;
            var prevTicket = ticketRepository.GetPrevious(lastTicket.Ticket.Id);
            Assert.AreEqual(prevTicket.OK, true);
            Assert.AreNotEqual(lastTicket.Ticket.Id, prevTicket.Ticket.Id);

            ticketRepository.DetachGet = false;
            lastTicket.Ticket.LinkedTickets.Add(prevTicket.Ticket);
            var changeResult = ticketRepository.AddOrUpdateLinkedTicket(lastTicket.Ticket);
            Assert.AreEqual(changeResult.OK, true);
        }

        [TestMethod]
        public void UpdateLinkedTicketTest()
        {
            ticketRepository.DetachGet = true;
            var lastTicket = ticketRepository.GetLast();
            Assert.AreEqual(lastTicket.OK, true);
            ticketRepository.DetachGet = true;
            var childTicket = ticketRepository.GetRandom();
            Assert.AreEqual(childTicket.OK, true);
            Assert.AreNotEqual(lastTicket.Ticket.Id, childTicket.Ticket.Id);

            ticketRepository.DetachGet = false;
            lastTicket.Ticket.LinkedTickets.Add(childTicket.Ticket);
            var changeResult = ticketRepository.AddOrUpdateLinkedTicket(lastTicket.Ticket);
            Assert.AreEqual(changeResult.OK, true);
        }

        [TestMethod]
        public void DeleteLinkedTicketTest()
        {
            ticketRepository.DetachGet = true;
            var lastTicket = ticketRepository.GetLast();
            Assert.AreEqual(lastTicket.OK, true);

            ticketRepository.DetachGet = false;
            var item = lastTicket.Ticket.LinkedTickets.LastOrDefault();
            Assert.AreNotEqual(item, null);
            var changeResult = ticketRepository.DeleteLinkedTicket(lastTicket.Ticket, item);
            Assert.AreEqual(changeResult.OK, true);
        }

        [TestMethod]
        public void AddResponseHistoryTest()
        {
            ticketRepository.DetachGet = true;
            var result = ticketRepository.GetLast();
            Assert.AreEqual(result.OK, true);

            ticketRepository.DetachGet = false;
            var items = result.Ticket.ResponseHistory;
            items.Add(new ResponseHistory
            {
                Via = ResponseHistoryVia.CalledLeftMessage,
                Comment = "Comment " + new Random().Next(),
                ResponseBy = new User(Id: "testuser")
            });
            var changeResult = ticketRepository.AddOrUpdateResponseHistory(result.Ticket);
            Assert.AreEqual(changeResult.OK, true);
        }

        [TestMethod]
        public void UpdateResponseHistoryTest()
        {
            ticketRepository.DetachGet = true;
            var result = ticketRepository.GetLast();
            Assert.AreEqual(result.OK, true);

            ticketRepository.DetachGet = false;
            var item = result.Ticket.ResponseHistory.LastOrDefault();
            Assert.AreNotEqual(item, null);
            item.Comment = "Comment " + new Random().Next();
            var changeResult = ticketRepository.AddOrUpdateResponseHistory(result.Ticket);
            Assert.AreEqual(changeResult.OK, true);
        }

        [TestMethod]
        public void AddResearchHistoryTest()
        {
            ticketRepository.DetachGet = true;
            var result = ticketRepository.GetLast();
            Assert.AreEqual(result.OK, true);

            ticketRepository.DetachGet = false;
            var items = result.Ticket.ResearchHistory;
            items.Add(new ResearchHistory
            {
                Comment = "Comment " + new Random().Next(),
                ResearchedBy = new User(Id: "testuser")
            });
            var changeResult = ticketRepository.AddOrUpdateResearchHistory(result.Ticket);
            Assert.AreEqual(changeResult.OK, true);
        }

        [TestMethod]
        public void UpdateResearchHistoryTest()
        {
            ticketRepository.DetachGet = true;
            var result = ticketRepository.GetLast();
            Assert.AreEqual(result.OK, true);

            ticketRepository.DetachGet = false;
            var item = result.Ticket.ResearchHistory.LastOrDefault();
            Assert.AreNotEqual(item, null);
            item.Comment = "Comment " + new Random().Next();
            var changeResult = ticketRepository.AddOrUpdateResearchHistory(result.Ticket);
            Assert.AreEqual(changeResult.OK, true);
        }

        [TestMethod]
        public void UpdateTicketStatusTest()
        {
            ticketRepository.DetachGet = true;
            var result = ticketRepository.GetLast();
            Assert.AreEqual(result.OK, true);
            var originalStatus = result.Ticket.Status.GetValueOrDefault();
            
            ticketRepository.DetachGet = false;
            var update1 = ticketRepository.UpdateTicketStatus(
                new ReadyToCloseReportParams
                {
                    Items = new List<ReadyToCloseReportParamItem>
                    {
                        new ReadyToCloseReportParamItem
                        {
                            Id = result.Ticket.Id,
                            CurrentStatus = TicketStatus.ReadyToClose.ToString().PascalCaseToDescription()
                        }
                    }
                });
            Assert.AreEqual(update1.OK, true);
            Assert.AreEqual(update1.Id, result.Ticket.Id.ToString());

            ticketRepository.DetachGet = true;
            result = ticketRepository.GetLast();
            Assert.AreEqual(result.OK, true);
            Assert.AreEqual(result.Ticket.Status, TicketStatus.ReadyToClose);
            
            ticketRepository.DetachGet = false;
            var update2 = ticketRepository.UpdateTicketStatus(
                new ReadyToCloseReportParams
                {
                    Items = new List<ReadyToCloseReportParamItem>
                    {
                        new ReadyToCloseReportParamItem
                        {
                            Id = result.Ticket.Id,
                            CurrentStatus = originalStatus.ToString().PascalCaseToDescription()
                        }
                    }
                });
            Assert.AreEqual(update2.OK, true);
            Assert.AreEqual(update2.Id, result.Ticket.Id.ToString());

            ticketRepository.DetachGet = true;
            result = ticketRepository.GetLast();
            Assert.AreEqual(result.OK, true);
            Assert.AreEqual(result.Ticket.Status, originalStatus);
        }

    
    }
}

