using Autofac;
using Nut.Core.Infrastructure;
using Nut.Core.Infrastructure.DependencyManagement;

using Nut.Services.App;
using Nut.WebAPI.Extensions;
using Nut.Core.Message;

namespace Nut.WebAPI.Infrastructure {
    public class DependencyRegistrar : IDependencyRegistrar {
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder) {

            builder.RegisterType<JPushUserService>().As<IJPushUserService>().InstancePerLifetimeScope();

            //builder.RegisterType<JPushChannel>().As<IJPushChannel>().InstancePerLifetimeScope();

            //builder.RegisterType<JPushMessageChannelSelector>().As<IMessageChannelSelector>().InstancePerLifetimeScope();
        }

        public int Order {
            get { return 4; }
        }
    }
}