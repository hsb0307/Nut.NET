using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nut.Core.Plugins;
using Nut.Plugin.APP.Version.Data;
using Nut.Services.Localization;

namespace Nut.Plugin.APP.Version {
    public class APPVersionPlugin : BasePlugin, IPlugin {
        private readonly APPVersionObjectContext _objectContext;

        public APPVersionPlugin(APPVersionObjectContext objectContext) {
            this._objectContext = objectContext;
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
