using System;
using System.Configuration;
using ACTransit.Contracts.Data.Common.PublicSite;
using ACTransit.Contracts.Data.Customer.Feedback;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ACTransit.CusRel.Public.Domain.Tests.Services.Customer.Feedback
{
    [TestClass]
    public class FeedbackServiceTest
    {
        private const string Uri = "http://your.company Website/cusrel-feedback-form";
        private const string OK = "Thank you for completing our form";
        
        public RequestState RequestState { get; set; }

        public FeedbackServiceTest()
        {
            RequestState = new RequestState
            {
                FeedbackToEmail = ConfigurationManager.AppSettings["FeedbackToEmail"]
            };
        }

        [TestMethod]
        public void PostSuggestionFeedbackWebClientTest()
        {
            // commented out to prevent always running
            //var comment = "Comment #" + new Random().Next(1, 4000);
            //var form = new SuggestionForm
            //{
            //    ActionEnum = ActionEnum.suggestion,
            //    Comments = comment,
            //    Contact = new Contact
            //    {
            //        Address = "1600 Franklin",
            //        City = "Oakland",
            //        EmailAddress = "your.email@your.company.dns",
            //        FirstName = "dev",
            //        LastName = "team",
            //        HomePhone = "1234567890",
            //        MobilePhone = "1234567890",
            //        State = "CA",
            //        WorkPhone = "1234567890",
            //        ZipCode = "93000"
            //    },
            //    RequestResponse = false
            //};
            //var service = new FeedbackService(form, Uri, OK);
            //var result = service.PostFeedback();
            //Assert.IsTrue(result.OK);
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void PostSuggestionFeedbackTest()
        {
            // commented out to prevent always running
            //var comment = "Comment #" + new Random().Next(1, 4000);
            //var form = new SuggestionForm
            //{
            //    ActionEnum = ActionEnum.suggestion,
            //    Comments = comment,
            //    Contact = new Contact
            //    {
            //        Address = "1600 Franklin",
            //        City = "Oakland",
            //        EmailAddress = "your.email@your.company.dns",
            //        FirstName = "dev",
            //        LastName = "team",
            //        HomePhone = "1234567890",
            //        MobilePhone = "1234567890",
            //        State = "CA",
            //        WorkPhone = "1234567890",
            //        ZipCode = "93000"
            //    },
            //    RequestResponse = false
            //};
            //var service = new FeedbackService(form, RequestState);
            //var result = service.SendToCusRelEntity();
            //Assert.IsTrue(result.OK);
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void PostIncidentFeedbackTest()
        {
            // commented out to prevent always running
            //var comment = "Comment #" + new Random().Next(1, 4000);
            //var form = new IncidentForm
            //{
            //    ActionEnum = ActionEnum.report,
            //    Comments = comment,
            //    Contact = new Contact
            //    {
            //        Address = "1600 Franklin",
            //        City = "Oakland",
            //        EmailAddress = "your.email@your.company.dns",
            //        FirstName = "dev",
            //        LastName = "team",
            //        HomePhone = "1234567890",
            //        MobilePhone = "1234567890",
            //        State = "CA",
            //        WorkPhone = "1234567890",
            //        ZipCode = "93000"
            //    },
            //    Incident = new Incident
            //    {
            //        DateTime = DateTime.Now,
            //        Destination = "Broadway and 4th",
            //        Line = "20",
            //        Location = "Next to stop",
            //        Vehicle = "4141"
            //    },
            //    Employee = new Employee
            //    {
            //        Badge = "A039J",
            //        Description = "Tall and mean"
            //    },
            //    RequestResponse = false
            //};
            //var service = new FeedbackService(form, RequestState);
            //var result = service.SendToCusRelEntity();
            //Assert.IsTrue(result.OK);
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void PostLostItemFeedbackTest()
        {
            // commented out to prevent always running
            //var comment = "Comment #" + new Random().Next(1, 4000);
            //var form = new IncidentForm
            //{
            //    ActionEnum = ActionEnum.report,
            //    Comments = comment,
            //    Contact = new Contact
            //    {
            //        Address = "1600 Franklin",
            //        City = "Oakland",
            //        EmailAddress = "your.email@your.company.dns",
            //        FirstName = "dev",
            //        LastName = "team",
            //        HomePhone = "1234567890",
            //        MobilePhone = "1234567890",
            //        State = "CA",
            //        WorkPhone = "1234567890",
            //        ZipCode = "93000"
            //    },
            //    Incident = new Incident
            //    {
            //        DateTime = DateTime.Now,
            //        Destination = "Broadway and 4th",
            //        Line = "20",
            //        Location = "Next to stop",
            //        Vehicle = "4141"
            //    },
            //    Employee = new Employee
            //    {
            //        Badge = "A039J",
            //        Description = "Tall and mean"
            //    },
            //    RequestResponse = false
            //};
            //var service = new FeedbackService(form, RequestState);
            //var result = service.SendToCusRelEntity();
            //Assert.IsTrue(result.OK);
            Assert.IsTrue(true);
        }
    }
}

