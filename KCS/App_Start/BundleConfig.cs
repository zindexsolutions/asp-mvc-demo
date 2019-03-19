using System.Web;
using System.Web.Optimization;

namespace KCS
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery.blockUI.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*",
                        "~/Scripts/custom/jquery.validate.date.js",
                        "~/Scripts/moment.js"));


            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                    "~/Content/bootstrap.css",
                    "~/Content/custom.css",
                    "~/Content/jquery.alerts.css"));

            bundles.Add(new StyleBundle("~/Content/adminlte").Include(
                    "~/Content/AdminLTE/AdminLTE.min.css",
                    "~/Content/AdminLTE/skins/_all-skins.min.css",
                    "~/Scripts/AdminLTE/plugins/daterangepicker/daterangepicker-bs3.css",
                    "~/Scripts/AdminLTE/plugins/datepicker/datepicker3.css",
                    "~/Scripts/AdminLTE/plugins/datatables/dataTables.bootstrap.css",
                    "~/Scripts/AdminLTE/plugins/fullcalendar/fullcalendar.min.css",
                    "~/Scripts/AdminLTE/plugins/fullcalendar/fullcalendar.print.min.css",
                     "~/Scripts/AdminLTE/plugins/select2/select2.min.css"));

            bundles.Add(new ScriptBundle("~/bundles/adminltejs").Include(
                        "~/Scripts/AdminLTE/app.js",
                        "~/Scripts/AdminLTE/plugins/daterangepicker/daterangepicker.js",
                        "~/Scripts/AdminLTE/plugins/datepicker/bootstrap-datepicker.js",
                        "~/Scripts/AdminLTE/plugins/slimScroll/jquery.slimscroll.min.js",
                        "~/Scripts/AdminLTE/plugins/chartjs/Chart.min.js",
                        "~/Scripts/AdminLTE/plugins/datatables/jquery.dataTables.js",
                        "~/Scripts/AdminLTE/plugins/datatables/dataTables.bootstrap.js",
                        "~/Scripts/AdminLTE/plugins/jQueryUI/jquery-ui-1.10.3.min.js",
                        "~/Scripts/AdminLTE/plugins/slimScroll/jquery.slimscroll.min.js",
                        "~/Scripts/AdminLTE/plugins/fullcalendar/fullcalendar.min.js",
                        "~/Scripts/AdminLTE/plugins/select2/select2.full.min.js",
                        "~/Scripts/AdminLTE/plugins/select2/select2.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/customjs").Include(
                        "~/Scripts/custom/custom.js",
                        "~/Scripts/jquery.alerts.js"));
        }
    }
}
