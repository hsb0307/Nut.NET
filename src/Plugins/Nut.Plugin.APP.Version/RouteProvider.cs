using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nut.Web.Framework.Mvc.Routes;
using System.Web.Routing;
using System.Web.Mvc;

namespace Nut.Plugin.APP.Version {
    public class RouteProvider : IRouteProvider {
        public void RegisterRoutes(RouteCollection routes) {
            routes.MapRoute("Plugin.APP.AppVersion.List",
                 "Plugins/AppVersion/List",
                 new { controller = "AppVersion", action = "List" },
                 new[] { "Nut.Plugin.APP.Version.Controllers" }
            );
        }
        public int Priority {
            get {
                return 0;
            }
        }
    }
}
