using System.Web.Optimization;

namespace MyNetSensors.WebController
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

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
                      "~/Content/site.css"));

            bundles.Add(new ScriptBundle("~/bundles/signalr").Include(
                      "~/Scripts/jquery.signalR-*"));

            bundles.Add(new ScriptBundle("~/bundles/moment").Include(
                      "~/Scripts/moment.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
              "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/switch").Include(
                "~/Scripts/bootstrap-switch.js"));

            bundles.Add(new ScriptBundle("~/bundles/mysensors").Include(
                "~/Scripts/mysensors.js"));

            bundles.Add(new ScriptBundle("~/bundles/mynsensors").Include(
                "~/Scripts/mysensors.js"));

            bundles.Add(new ScriptBundle("~/bundles/mynetsensors-view-page").Include(
                "~/Scripts/mynetsensors-view-page.js"));

            bundles.Add(new ScriptBundle("~/bundles/mynetsensors-control-page").Include(
                "~/Scripts/mynetsensors-control-page.js"));

            bundles.Add(new ScriptBundle("~/bundles/mynetsensors-settings-page").Include(
                 "~/Scripts/mynetsensors-settings-page.js"));

            bundles.Add(new ScriptBundle("~/bundles/noty").Include(
                "~/Scripts/noty/packaged/jquery.noty.packaged.min.js",
                "~/Scripts/noty/themes/relax.js"));

            bundles.Add(new StyleBundle("~/Content/font-awesome")
                .Include("~/Content/font-awesome.css"));

        }
    }
}
