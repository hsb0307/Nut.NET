using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nut.Core.Infrastructure;
using Nut.Core.Infrastructure.DependencyManagement;
using Nut.Web.Framework.Mvc;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;
using Nut.Plugin.APP.Version.Data;
using Nut.Plugin.APP.Version.Domain;
using Nut.Plugin.APP.Version.Services;
using Nut.Data;
using Nut.Core.Data;


namespace Nut.Plugin.APP.Version {
    public class DependencyRegistrar : IDependencyRegistrar {
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder) {

            builder.RegisterType<AppVersionService>().As<IAppVersionService>().InstancePerLifetimeScope();

            this.RegisterPluginDataContext<APPVersionObjectContext>(builder, "nut_object_context_app_version");

            builder.RegisterType<EfRepository<AppVersion>>().As<IRepository<AppVersion>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nut_object_context_app_version"))
                .InstancePerLifetimeScope();
        }

        public int Order {
            get { return 1; }
        }
    }

}
