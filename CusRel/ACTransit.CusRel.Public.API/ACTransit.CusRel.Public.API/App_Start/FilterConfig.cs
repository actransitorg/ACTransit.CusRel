using System;
using System.Diagnostics.Contracts;
using System.Web.Mvc;

namespace ACTransit.CusRel.Public.API
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            if (filters == null)
                throw new ArgumentNullException();
            filters.Add(new HandleErrorAttribute());
        }
    }
}
