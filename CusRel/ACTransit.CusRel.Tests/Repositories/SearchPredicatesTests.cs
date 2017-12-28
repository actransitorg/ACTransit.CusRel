using System;
using System.Collections.Generic;
using System.Linq;
using ACTransit.CusRel.Repositories.DAL;
using ACTransit.CusRel.Repositories.Mapping;
using ACTransit.CusRel.Repositories.Search;
using LinqKit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ACTransit.Entities.CustomerRelations;

namespace ACTransit.CusRel.Tests.Repositories
{
    public static class TestExtension
    {
    }
    
    [TestClass]
    public class SearchPredicatesTests
    {
        //[TestMethod]
        //public void SplitPascalCase()
        //{
        //    var test = "ThisIsATest";
        //    //var result = test.SplitPascalCaseDescending();
        //}


        //[TestMethod]
        //public void SimpleIdTest()
        //{
        //    var searchCriteria = new SearchTicketParams
        //    {
        //        Id = 362526
        //    };
        //    var searchEntity = searchCriteria.ToEntities<SearchContacts>().Contact;

        //    using (var context = new CusRelDbContext())
        //    {
        //        var results = searchEntity.BuildQuery(context).ToList();
        //        Assert.IsNotNull(results);
        //        Assert.AreSame(results.Count, 1);
        //    }
        //}

        //[TestMethod]
        //public void SimpleSearchPredicatesTest()
        //{
        //    var searchCriteria = new SearchTicketParams
        //    {
        //        Id = 362526
        //    };
        //    var searchEntity = searchCriteria.ToEntities<SearchContacts>().Contact;

        //    using (var context = new CusRelDbContext())
        //    {
        //        var predicate = SearchPredicates.BuildPredicate<tblContacts, SearchContacts>(searchEntity);
        //        var query = context.tblContacts.AsExpandable().Where(predicate) as IOrderedQueryable<tblContacts>;
        //        var result = query.ToList();
        //    }
        //}
    }
}
