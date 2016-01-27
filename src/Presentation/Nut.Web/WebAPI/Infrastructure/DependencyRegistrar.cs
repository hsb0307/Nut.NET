using Autofac;
using Nut.Core.Infrastructure;
using Nut.Core.Infrastructure.DependencyManagement;


namespace Nut.WebAPI.Infrastructure {
    public class DependencyRegistrar : IDependencyRegistrar {
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder) {

            
        }

        public int Order {
            get { return 3; }
        }
    }
}