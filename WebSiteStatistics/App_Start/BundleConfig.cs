using System.Web;
using System.Web.Optimization;

namespace WebSiteStatistics
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-1.10.2.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/SignalR").Include(
                        "~/Scripts/jquery.signalR-2.2.1.min.js"));


            bundles.Add(new ScriptBundle("~/bundles/Canvas").Include(
                       "~/Scripts/canvasjs.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/Chartsjs").Include(
                        "~/Scripts/flot/jquery.flot.min.js",
                        "~/Scripts/flot/jquery.flot.resize.min.js",
                        "~/Scripts/flot/jquery.flot.orderBars.js",
                        "~/Scripts/flot/jquery.flot.stack.min.js",
                        "~/Scripts/flot/jquery.flot.pie.min.js",
                        "~/Scripts/flot-tooltip/jquery.flot.tooltip.min.js",
                        "~/Scripts/raphael/raphael-min.js",
                        "~/Scripts/morris/morris.min.js"
                        ));

            bundles.Add(new StyleBundle("~/Content/Chartscss").Include(
                     "~/Scripts/morris/morris.css"
                    ));
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
"~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/font-awesome.css",
                      "~/Content/site.css"));
        }
    }
}
