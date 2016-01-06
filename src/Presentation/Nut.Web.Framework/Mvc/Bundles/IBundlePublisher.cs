using System;
using System.Web.Optimization;

namespace Nut.Web.Framework.Mvc.Bundles {
    public interface IBundlePublisher {
        void RegisterBundles(BundleCollection bundles);
    }
}
