using Nut.Web.Framework.Mvc.Bundles;
using System.Web.Optimization;

namespace Nut.Web.Infrastructure {
    public class BundleConfig : IBundleProvider {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public void RegisterBundles(BundleCollection bundles) {

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-2.1.3.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));
        }

        public int Priority {
            get {
                return 99;
            }
        }
    }
}