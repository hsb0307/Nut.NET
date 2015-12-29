using System;
using Autofac;
using Nut.Core.Events;

namespace Nut.Web.Framework.Events {
    public class EventsModule : Module {
        protected override void Load(ContainerBuilder builder) {
            builder.RegisterType<DefaultEventBus>()
               .As<IEventBus>().SingleInstance();

            builder.RegisterSource(new EventsRegistrationSource());
            base.Load(builder);
        }
    }
}
