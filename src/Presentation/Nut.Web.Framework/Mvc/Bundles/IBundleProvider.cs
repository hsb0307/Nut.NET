using System.Web.Optimization;

namespace Nut.Web.Framework.Mvc.Bundles {
    public interface IBundleProvider {
        void RegisterBundles(BundleCollection bundles);

        int Priority { get; }
    }
}
