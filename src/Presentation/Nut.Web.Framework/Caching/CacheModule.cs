using System;
using System.Linq;
using Autofac;

using Nut.Core.Caching;

namespace Nut.Web.Framework.Caching {
    public class CacheModule : Module {
        protected override void Load(ContainerBuilder builder) {

            builder.RegisterType<Signals>()
               .As<ISignals>().As<IVolatileProvider>().InstancePerLifetimeScope();

            builder.RegisterType<Clock>()
               .As<IClock>().As<IVolatileProvider>().InstancePerLifetimeScope();

            builder.RegisterType<DefaultCacheHolder>()
               .As<ICacheHolder>().InstancePerLifetimeScope();

            builder.RegisterType<DefaultCacheContextAccessor>().As<ICacheContextAccessor>()
                .InstancePerLifetimeScope();

            builder.RegisterType<DefaultCacheManager>()
                .As<ICacheManager>()
                .InstancePerLifetimeScope();
        }

        protected override void AttachToComponentRegistration(Autofac.Core.IComponentRegistry componentRegistry, Autofac.Core.IComponentRegistration registration) {
            var needsCacheManager = registration.Activator.LimitType
                .GetConstructors()
                .Any(x => x.GetParameters()
                    .Any(xx => xx.ParameterType == typeof(ICacheManager)));

            if (needsCacheManager) {
                registration.Preparing += (sender, e) => {
                    var parameter = new TypedParameter(
                        typeof(ICacheManager),
                        e.Context.Resolve<ICacheManager>(new TypedParameter(typeof(Type), registration.Activator.LimitType)));
                    e.Parameters = e.Parameters.Concat(new[] { parameter });
                };
            }
        }
    }
}
