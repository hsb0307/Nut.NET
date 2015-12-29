using Autofac;
using Autofac.Core;
using Nut.Core.Caching;
using Nut.Core.Infrastructure;
using Nut.Core.Infrastructure.DependencyManagement;
using Nut.Web.Controllers;
using Nut.Web.Infrastructure.Installation;

namespace Nut.Web.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            //we cache presentation models between requests            
            //installation localization service
            builder.RegisterType<InstallationLocalizationService>().As<IInstallationLocalizationService>().InstancePerLifetimeScope();
        }

        public int Order
        {
            get { return 2; }
        }
    }
}
