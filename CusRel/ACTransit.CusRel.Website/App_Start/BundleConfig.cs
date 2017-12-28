using System.Web;
using System.Web.Optimization;

namespace ACTransit.CusRel
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            // Styles
            
            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/bootstrap.css",
                "~/Content/site.css",
                "~/Content/magicsuggest-{version}.css"));

            bundles.Add(new StyleBundle("~/Content/TicketDetails").Include(
                "~/Content/CustomTicket.css",
                "~/Content/jquery.datetimepicker-2.4.0.css",
                "~/Content/jquery.fileupload.css",
                "~/Content/jquery.combobox/style.css"));

            bundles.Add(new StyleBundle("~/Content/Tables").Include(
                "~/Content/bootstrap-sortable.css",
                "~/Content/CustomTable.css"));

            // Scripts

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryUI").Include(
                "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryUI").Include(
                "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new StyleBundle("~/content/jqueryUI").Include(
                "~/Content/themes/base/jquery-ui.min.css"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                "~/Scripts/jquery.validate*"));
           
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/Scripts/bootstrap.js",
                "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/datetimepicker").Include(
                "~/Scripts/jquery.datetimepicker-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/modalBox").Include(
                "~/Scripts/modalBox-1.0.0.js"));

            bundles.Add(new ScriptBundle("~/bundles/TicketDetails").Include(
                "~/Scripts/employeeSearch.js",
                "~/Scripts/Fileupload/jquery.ui.widget.js",
                "~/Scripts/Fileupload/jquery.fileupload.js",
                "~/Scripts/magicsuggest-{version}.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/Tables").Include(
                "~/Scripts/moment-{version}.js",
                "~/Scripts/underscore.js",
                "~/Scripts/jquery.floatThead.js",
                "~/Scripts/bootstrap-sortable.js"));

            bundles.Add(new ScriptBundle("~/bundles/TinyMce").Include(
                "~/Scripts/tinymce/tinymce.min.js"));

            
            // Set EnableOptimizations to false for debugging. For more information,
            // visit http://go.microsoft.com/fwlink/?LinkId=301862
            BundleTable.EnableOptimizations = false;
        }
    }
}
