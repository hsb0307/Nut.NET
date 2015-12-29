using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nut.Web.Framework.Security;
using Nut.Services.Security;

namespace Nut.Web.Controllers {
    public class HomeController : BasePublicController {

        private readonly IPermissionService _permissionService;

        public HomeController(IPermissionService permissionService) {
            this._permissionService = permissionService;
        }


        [NopHttpsRequirement(SslRequirement.No)]
        // GET: Home
        public ActionResult Index() {
            if (_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return RedirectToAction("Index", "Home", new { area = "Admin" });

            return View();
        }
    }
}