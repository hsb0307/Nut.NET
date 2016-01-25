using System.Web.Optimization;
using Nut.Web.Framework.Mvc.Bundles;

namespace Nut.Web.Infrastructure {
    public class BundleConfig : IBundleProvider {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public void RegisterBundles(BundleCollection bundles) {
            // App Styles
            bundles.Add(new StyleBundle("~/Content/appCss").Include(
                "~/Content/app/css/app.css"
            ));

            // Bootstrap Styles
            bundles.Add(new StyleBundle("~/Content/bootstrapCss").Include(
                "~/Content/app/css/bootstrap.css", new CssRewriteUrlTransform()
            ));


            bundles.Add(new ScriptBundle("~/bundles/Angle").Include(
                // App init
                "~/Scripts/app/app.init.js",
                // Modules
                "~/Scripts/app/modules/bootstrap-start.js",
                "~/Scripts/app/modules/clear-storage.js",
                "~/Scripts/app/modules/constants.js",
               "~/Scripts/app/modules/fullscreen.js",
               "~/Scripts/app/modules/load-css.js",
                "~/Scripts/app/modules/localize.js",
                "~/Scripts/app/modules/navbar-search.js",
                "~/Scripts/app/modules/sidebar.js",
                "~/Scripts/app/modules/toggle-state.js",
                "~/Scripts/app/modules/utils.js",
                "~/Scripts/admin.common.js"
            ));

            // Main Vendor

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            // Vendor Plugins

            bundles.Add(new StyleBundle("~/bundles/simpleLineIcons").Include(
              "~/Vendor/simple-line-icons/css/simple-line-icons.css", new CssRewriteUrlTransform()
            ));


            bundles.Add(new ScriptBundle("~/bundles/storage").Include(
              "~/Vendor/jQuery-Storage-API/jquery.storageapi.js"
            ));

            bundles.Add(new StyleBundle("~/bundles/whirl").Include(
              "~/Vendor/whirl/dist/whirl.css"
            ));

            bundles.Add(new StyleBundle("~/bundles/fontawesome").Include(
              "~/Vendor/fontawesome/css/font-awesome.min.css", new CssRewriteUrlTransform()
            ));

            bundles.Add(new ScriptBundle("~/bundles/screenfull").Include(
             "~/Vendor/screenfull/dist/screenfull.js"
           ));

            bundles.Add(new StyleBundle("~/bundles/animatecss").Include(
              "~/Vendor/animate.css/animate.min.css"
            ));

            bundles.Add(new ScriptBundle("~/bundles/localize").Include(
              "~/Vendor/jquery-localize-i18n/dist/jquery.localize.js"
            ));

            bundles.Add(new StyleBundle("~/bundles/kendocss").Include(
              "~/Vendor/kendo/2014.1.318/kendo.default.min.css",
              "~/Vendor/kendo/2014.1.318/kendo.bootstrap.min.css",
              "~/Vendor/kendo/2014.1.318/kendo.common.min.css",
              "~/Vendor/jquery-ui/themes/smoothness/jquery-ui.css"
            ));

            bundles.Add(new ScriptBundle("~/bundles/kendo").Include(
              //"~/Scripts/kendo/2014.1.318/cultures/kendo.culture.zh-Hans.min.js",
              "~/Scripts/kendo/2014.1.318/kendo.web.min.js"
           ));

            bundles.Add(new StyleBundle("~/bundles/SweetAlertCss").Include(
                "~/Vendor/sweetalert/dist/sweetalert.css"
            ));
            bundles.Add(new ScriptBundle("~/bundles/SweetAlert").Include(
                "~/Vendor/sweetalert/dist/sweetalert.min.js"
            ));

            bundles.Add(new StyleBundle("~/bundles/JqueryGritterCss").Include(
                "~/Vendor/gritter/jquery.gritter.css"
            ));

            bundles.Add(new ScriptBundle("~/bundles/JqueryGritter").Include(
                "~/Vendor/gritter/jquery.gritter.min.js"
            ));

            
        }

        public int Priority {
            get {
                return 1;
            }
        }
    }
}