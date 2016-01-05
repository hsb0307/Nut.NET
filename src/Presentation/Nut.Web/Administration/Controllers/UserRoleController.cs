using Nut.Admin.Extensions;
using Nut.Admin.Models.Users;
using Nut.Core;
using Nut.Core.Domain.Users;
using Nut.Services.Localization;
using Nut.Services.Security;
using Nut.Services.Stores;
using Nut.Services.Users;
using Nut.Web.Framework.Controllers;
using Nut.Web.Framework.Kendoui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nut.Services.Logging;

namespace Nut.Admin.Controllers {
    public class UserRoleController : BaseAdminController {
        #region Fields

        private readonly IUserService _userService;
        private readonly IActivityLogService _activityLogService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Constructors

        public UserRoleController(IUserService userService,
            IActivityLogService activityLogService,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            IStoreService storeService,
            IWorkContext workContext) {
            this._userService = userService;
            this._activityLogService = activityLogService;
            this._localizationService = localizationService;
            this._permissionService = permissionService;
            this._storeService = storeService;
            this._workContext = workContext;
        }

        #endregion

        #region Utilities

        [NonAction]
        protected UserRoleModel PrepareUserRoleModel(UserRole userRole) {
            var model = userRole.ToModel();
            return model;
        }

        #endregion

        #region Customer roles

        public ActionResult Index() {
            return RedirectToAction("List");
        }

        public ActionResult List() {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public ActionResult List(DataSourceRequest command) {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var customerRoles = _userService.GetAllUserRoles(true);

            var gridModel = new DataSourceResult {
                Data = customerRoles.Select(PrepareUserRoleModel),
                Total = customerRoles.Count
            };

            return Json(gridModel);
        }

        public ActionResult Create() {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var model = new UserRoleModel();
            //default values
            model.Active = true;
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Create(UserRoleModel model, bool continueEditing) {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            if (ModelState.IsValid) {
                var userRole = model.ToEntity();
                _userService.InsertUserRole(userRole);

                //activity log
                _activityLogService.InsertActivity("AddNewUserRole", _localizationService.GetResource("ActivityLog.AddNewUserRole"), userRole.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Users.UserRoles.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = userRole.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult Edit(int id) {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var userRole = _userService.GetUserRoleById(id);
            if (userRole == null)
                //No customer role found with the specified id
                return RedirectToAction("List");

            var model = PrepareUserRoleModel(userRole);
            if (model.IsSystemRole.Equals(true)) {
                model.IsSystemRoleShow = "是";
            } else {
                model.IsSystemRoleShow = "否";
            }
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Edit(UserRoleModel model, bool continueEditing) {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var userRole = _userService.GetUserRoleById(model.Id);
            if (userRole == null)
                //No customer role found with the specified id
                return RedirectToAction("List");

            try {
                if (ModelState.IsValid) {
                    if (userRole.IsSystemRole && !model.Active)
                        throw new NutException(_localizationService.GetResource("Admin.Users.UserRoles.Fields.Active.CantEditSystem"));

                    if (userRole.IsSystemRole && !userRole.SystemName.Equals(model.SystemName, StringComparison.InvariantCultureIgnoreCase))
                        throw new NutException(_localizationService.GetResource("Admin.Users.UserRoles.Fields.SystemName.CantEditSystem"));

                    userRole = model.ToEntity(userRole);
                    _userService.UpdateUserRole(userRole);

                    //activity log
                    _activityLogService.InsertActivity("EditUserRole", _localizationService.GetResource("ActivityLog.EditUserRole"), userRole.Name);

                    SuccessNotification(_localizationService.GetResource("Admin.Users.UserRoles.Updated"));
                    return continueEditing ? RedirectToAction("Edit", userRole.Id) : RedirectToAction("List");
                }

                //If we got this far, something failed, redisplay form
                return View(model);
            } catch (Exception exc) {
                ErrorNotification(exc);
                return RedirectToAction("Edit", new { id = userRole.Id });
            }
        }

        [HttpPost]
        public ActionResult Delete(int id) {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var customerRole = _userService.GetUserRoleById(id);
            if (customerRole == null)
                //No customer role found with the specified id
                return RedirectToAction("List");

            try {
                _userService.DeleteUserRole(customerRole);

                //activity log
                _activityLogService.InsertActivity("DeleteUserRole", _localizationService.GetResource("ActivityLog.DeleteUserRole"), customerRole.Name);

                return Json(new { success = true, message = _localizationService.GetResource("Admin.Users.UserRoles.Deleted") });
            } catch (Exception exc) {
                ErrorNotification(exc.Message);
                return Json(new { success = false, message = _localizationService.GetResource("Admin.Users.UserRoles.Deleted"), });
            }

        }

        #endregion
    }
}