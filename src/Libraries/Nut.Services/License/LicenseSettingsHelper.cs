using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nut.Core.License;
using Nut.Core.Infrastructure;

namespace Nut.Services.License {
    public class LicenseSettingsHelper {
        private static bool? _LicenseIsInstalled;
        public static bool LicenseIsInstalled() {
            if (!_LicenseIsInstalled.HasValue) {
                var manager = new LicenseSettingsManager();
                var settings = manager.LoadSettings();
                var licenseService = EngineContext.Current.Resolve<ILicenseService>();

                _LicenseIsInstalled = settings != null && (!String.IsNullOrEmpty(settings) && licenseService.CheckAccess());
            }
            return _LicenseIsInstalled.Value;
        }

        public static void ResetCache() {
            _LicenseIsInstalled = null;
        }
    }
}
