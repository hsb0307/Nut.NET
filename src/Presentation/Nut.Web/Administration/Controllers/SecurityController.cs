using Nut.Admin.Models.Security;
using Nut.Admin.Models.Users;
using Nut.Core;
using Nut.Core.Logging;
using Nut.Services.Localization;
using Nut.Services.Security;
using Nut.Services.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Nut.Admin.Controllers {
    public class SecurityController : BaseAdminController {
        #region Fields
        private readonly IWorkContext _workContext;
        private readonly IPermissionService _permissionService;
        private readonly IUserService _userService;
        private readonly ILocalizationService _localizationService;

        #endregion

        #region Constructors

        public SecurityController(IWorkContext workContext,
            IPermissionService permissionService,
            IUserService userService, ILocalizationService localizationService) {
            this._workContext = workContext;
            this._permissionService = permissionService;
            this._userService = userService;
            this._localizationService = localizationService;

            this.Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        #endregion

        #region Methods

        public ActionResult AccessDenied(string pageUrl) {
            var currentCustomer = _workContext.CurrentUser;
            if (currentCustomer == null) {
                Logger.Information(string.Format("Access denied to anonymous request on {0}", pageUrl));
                return View();
            }

            Logger.Information(string.Format("Access denied to user #{0} '{1}' on {2}", currentCustomer.Email, currentCustomer.Email, pageUrl));


            return View();
        }

        public ActionResult Permissions() {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAcl))
                return AccessDeniedView();

            var model = new PermissionMappingModel();

            var permissionRecords = _permissionService.GetAllPermissionRecords();
            var userRoles = _userService.GetAllUserRoles(true);
            foreach (var pr in permissionRecords) {
                model.AvailablePermissions.Add(new PermissionRecordModel {
                    //Name = pr.Name,
                    Name = pr.GetLocalizedPermissionName(_localizationService, _workContext),
                    SystemName = pr.SystemName
                });
            }
            foreach (var cr in userRoles) {
                model.AvailableCustomerRoles.Add(new UserRoleModel {
                    Id = cr.Id,
                    Name = cr.Name
                });
            }
            foreach (var pr in permissionRecords)
                foreach (var cr in userRoles) {
                    bool allowed = pr.UserRoles.Count(x => x.Id == cr.Id) > 0;
                    if (!model.Allowed.ContainsKey(pr.SystemName))
                        model.Allowed[pr.SystemName] = new Dictionary<int, bool>();
                    model.Allowed[pr.SystemName][cr.Id] = allowed;
                }

            return View(model);
        }

        [HttpPost, ActionName("Permissions")]
        public ActionResult PermissionsSave(FormCollection form) {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAcl))
                return AccessDeniedView();

            var permissionRecords = _permissionService.GetAllPermissionRecords();
            var userRoles = _userService.GetAllUserRoles(true);


            foreach (var cr in userRoles) {
                string formKey = "allow_" + cr.Id;
                var permissionRecordSystemNamesToRestrict = form[formKey] != null ? form[formKey].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList() : new List<string>();

                foreach (var pr in permissionRecords) {

                    bool allow = permissionRecordSystemNamesToRestrict.Contains(pr.SystemName);
                    if (allow) {
                        if (pr.UserRoles.FirstOrDefault(x => x.Id == cr.Id) == null) {
                            pr.UserRoles.Add(cr);
                            _permissionService.UpdatePermissionRecord(pr);
                        }
                    } else {
                        if (pr.UserRoles.FirstOrDefault(x => x.Id == cr.Id) != null) {
                            pr.UserRoles.Remove(cr);
                            _permissionService.UpdatePermissionRecord(pr);
                        }
                    }
                }
            }

            SuccessNotification(_localizationService.GetResource("Admin.Configuration.ACL.Updated"));
            return RedirectToAction("Permissions");
        }

        #endregion
    }
}