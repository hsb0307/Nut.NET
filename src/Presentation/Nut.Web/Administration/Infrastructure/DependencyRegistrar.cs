using Autofac;
using Autofac.Core;
using Nut.Core.Infrastructure;
using Nut.Core.Infrastructure.DependencyManagement;

namespace Nut.Admin.Infrastructure {
    public class DependencyRegistrar : IDependencyRegistrar {
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder) {


        }

        public int Order {
            get { return 2; }
        }
    }
}