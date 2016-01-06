using Nut.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Optimization;

namespace Nut.Web.Framework.Mvc.Bundles {
    /// <summary>
    /// Bundle Publisher
    /// </summary>
    public class BundlePublisher : IBundlePublisher {
        protected readonly ITypeFinder typeFinder;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="typeFinder"></param>
        public BundlePublisher(ITypeFinder typeFinder) {
            this.typeFinder = typeFinder;
        }
        /// <summary>
        /// Registers the bundles.
        /// </summary>
        /// <param name="bundles">The bundles.</param>
        public virtual void RegisterBundles(BundleCollection bundles) {
            var bundleProviderTypes = typeFinder.FindClassesOfType<IBundleProvider>();
            var bundleProviders = new List<IBundleProvider>();
            foreach (var providerType in bundleProviderTypes) {
                //Ignore not installed plugins
                //var plugin = FindPlugin(providerType);
                //if (plugin != null && !plugin.Installed)
                //    continue;

                var provider = Activator.CreateInstance(providerType) as IBundleProvider;
                bundleProviders.Add(provider);
            }
            bundleProviders = bundleProviders.OrderByDescending(rp => rp.Priority).ToList();

            bundleProviders.ForEach(rp => rp.RegisterBundles(bundles));
        }
    }
}
