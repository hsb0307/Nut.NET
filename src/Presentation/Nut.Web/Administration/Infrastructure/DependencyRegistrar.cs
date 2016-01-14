using Autofac;
using Autofac.Core;
using Nut.Core.Infrastructure;
using Nut.Core.Infrastructure.DependencyManagement;

using Nut.Services.App;

namespace Nut.Admin.Infrastructure {
    public class DependencyRegistrar : IDependencyRegistrar {
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder) {

            builder.RegisterType<AppVersionService>().As<IAppVersionService>().InstancePerLifetimeScope();

        }

        public int Order {
            get { return 2; }
        }
    }
}