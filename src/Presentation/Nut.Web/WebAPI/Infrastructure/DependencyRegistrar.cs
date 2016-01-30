using Autofac;
using Nut.Core.Infrastructure;
using Nut.Core.Infrastructure.DependencyManagement;

using Nut.Services.App;


namespace Nut.WebAPI.Infrastructure {
    public class DependencyRegistrar : IDependencyRegistrar {
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder) {

            builder.RegisterType<JPushUserService>().As<IJPushUserService>().InstancePerLifetimeScope();
        }

        public int Order {
            get { return 4; }
        }
    }
}