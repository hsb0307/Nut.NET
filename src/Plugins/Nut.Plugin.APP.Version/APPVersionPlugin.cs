using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nut.Core.Plugins;
using Nut.Plugin.APP.Version.Data;
using Nut.Web.Framework.Menu;
using Nut.Services.Localization;
using System.Web.Routing;

namespace Nut.Plugin.APP.Version {
    public class APPVersionPlugin : BasePlugin, IAdminMenuPlugin, IPlugin {
        private readonly APPVersionObjectContext _objectContext;

        public APPVersionPlugin(APPVersionObjectContext objectContext) {
            this._objectContext = objectContext;
        }

        /// <summary>
        /// Manage sitemap. You can use "SystemName" of menu items to manage existing sitemap or add a new menu item.
        /// </summary>
        /// <param name="rootNode">Root node of the sitemap.</param>
        public void ManageSiteMap(SiteMapNode rootNode) {
            rootNode.ChildNodes.Add(new SiteMapNode() {
                ActionName = "List",
                ControllerName = "AppVersion",
                Icon = "chemistry",
                SystemName = "Admin.AppVersion",
                Title = "AppVersion",
                Visible = true,
                RouteValues = new RouteValueDictionary { { "area", "Admin" } }
            });
        }

        public override void Install() {
            _objectContext.Install();
            //Local

            base.Install();
        }

        public override void Uninstall() {

            _objectContext.Uninstall();
            base.Uninstall();
        }
    }
}
