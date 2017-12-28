using System.Web.Mvc;
using ACTransit.CusRel.Models.Filters;

namespace ACTransit.CusRel
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new ExceptionFilter());
            filters.Add(new HandleErrorAttribute());
        }
    }
}
