using System.Web.Optimization;

namespace MyNetSensors.WebController
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //jquery
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            //modernizr
            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            //bootstrap
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            //site styles
            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            //signalr
            bundles.Add(new ScriptBundle("~/bundles/signalr").Include(
                      "~/Scripts/jquery.signalR-*"));

            //moment
            bundles.Add(new ScriptBundle("~/bundles/moment").Include(
                      "~/Scripts/moment.js"));

            //jqueryui
            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
              "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new StyleBundle("~/Content/jqueryui")
              .Include("~/Content/themes/base/all.css"));


            //bootstrap-switch
            bundles.Add(new ScriptBundle("~/bundles/switch").Include(
                "~/Scripts/bootstrap-switch.js"));

            bundles.Add(new StyleBundle("~/Content/switch")
              .Include("~/Content/bootstrap-switch.css"));


            //scrips for pages
            bundles.Add(new ScriptBundle("~/bundles/mysensors").Include(
                "~/Scripts/mysensors.js"));

            bundles.Add(new ScriptBundle("~/bundles/mynetsensors-view-page").Include(
                "~/Scripts/mynetsensors-view-page.js"));

            bundles.Add(new ScriptBundle("~/bundles/mynetsensors-control-page").Include(
                "~/Scripts/mynetsensors-control-page.js"));

            bundles.Add(new ScriptBundle("~/bundles/mynetsensors-settings-page").Include(
                 "~/Scripts/mynetsensors-settings-page.js"));

            bundles.Add(new ScriptBundle("~/bundles/mynetsensors-history-chart-page").Include(
                "~/Scripts/mynetsensors-history-chart-page.js"));

            bundles.Add(new ScriptBundle("~/bundles/mynetsensors-history-chart-live-page").Include(
                "~/Scripts/mynetsensors-history-chart-live-page.js"));


            //noty
            bundles.Add(new ScriptBundle("~/bundles/noty").Include(
                "~/Scripts/noty/packaged/jquery.noty.packaged.min.js",
                "~/Scripts/noty/themes/relax.js"));

            bundles.Add(new StyleBundle("~/Content/noty")
                .Include("~/Content/animate.min.css"));

            //font-awesome for WebGrid pagenation
            bundles.Add(new StyleBundle("~/Content/font-awesome")
                .Include("~/Content/font-awesome.css"));


            //vis js charts
            bundles.Add(new ScriptBundle("~/bundles/visjs").Include(
                "~/Scripts/vis.min.js"));

            bundles.Add(new StyleBundle("~/Content/visjs")
                .Include("~/Content/vis.min.css"));
        }
    }
}
