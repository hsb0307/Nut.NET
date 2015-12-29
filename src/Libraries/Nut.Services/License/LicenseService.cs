using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Nut.Core.License;

namespace Nut.Services.License {
    public class LicenseService : ILicenseService {
        private readonly IMachineCodeProvider _machineCodeProvider;
        private readonly ICryptoProvider _cryptoProvider;

        public LicenseService(IMachineCodeProvider machineCodeProvider, ICryptoProvider cryptoProvider) {
            this._machineCodeProvider = machineCodeProvider;
            this._cryptoProvider = cryptoProvider;
        }
        public void Install(string licenseCode) {
            var manager = new LicenseSettingsManager();
            manager.SaveSettings(licenseCode);
        }

        public bool TryCheckAccess(string LicenseCode) {

            var machineCode = _machineCodeProvider.GetMachineCode();
            var cryproText = _cryptoProvider.Encrypt(machineCode);

            return cryproText.ToLower().Equals(LicenseCode.ToLower());
        }

        public bool CheckAccess() {

            var manager = new LicenseSettingsManager();
            var codeResult = manager.LoadSettings();
            if (string.IsNullOrEmpty(codeResult))
                return false;
            return TryCheckAccess(codeResult);
        }
    }
}
