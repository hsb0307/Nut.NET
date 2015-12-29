using System.Web.Mvc;
using Nut.Core.License;
using Nut.Services.License;
using Nut.Web.Models.License;

namespace Nut.Web.Controllers {
    public class LicenseController : BasePublicController {

        #region Fields

        private readonly IMachineCodeProvider _machineCodeProvider;
        private readonly ILicenseService _licenseService;

        #endregion

        #region Ctor

        public LicenseController(IMachineCodeProvider machineCodeProvider, ILicenseService licenseService) {
            this._machineCodeProvider = machineCodeProvider;
            this._licenseService = licenseService;
        }

        #endregion

        #region Methods
        public ActionResult Index() {
            if (LicenseSettingsHelper.LicenseIsInstalled())
                return RedirectToRoute("HomePage");
            var model = new LicenseModel() {
                MachineCode = _machineCodeProvider.GetMachineCode(),
                LicenseCode = ""
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(LicenseModel model) {
            if (LicenseSettingsHelper.LicenseIsInstalled())
                return RedirectToRoute("HomePage");

            if (ModelState.IsValid) {
                if (_licenseService.TryCheckAccess(model.LicenseCode)) {
                    _licenseService.Install(model.LicenseCode);
                    LicenseSettingsHelper.ResetCache();
                    return RedirectToRoute("HomePage");
                }

                ModelState.AddModelError("", "序列号不正确，请重新填写！");
            }
            return View(model);
        }
        #endregion

    }
}