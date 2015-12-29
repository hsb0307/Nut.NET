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

namespace Nut.Admin.Controllers
{
    public class UserRoleController : BaseAdminController
    {
        #region Fields

        private readonly IUserService _userService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Constructors

        public UserRoleController(IUserService userService,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            IStoreService storeService,
            IWorkContext workContext) {
            this._userService = userService;
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
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public ActionResult List(DataSourceRequest command) {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            var customerRoles = _userService.GetAllUserRoles(true);

            var gridModel = new DataSourceResult {
                Data = customerRoles.Select(PrepareUserRoleModel),
                Total = customerRoles.Count
            };

            return Json(gridModel);
        }

        public ActionResult Create() {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            var model = new UserRoleModel();
            //default values
            model.Active = true;
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Create(UserRoleModel model, bool continueEditing) {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            if (ModelState.IsValid) {
                var userRole = model.ToEntity();
                _userService.InsertUserRole(userRole);

                //activity log
                //_customerActivityService.InsertActivity("AddNewCustomerRole", _localizationService.GetResource("ActivityLog.AddNewCustomerRole"), customerRole.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Users.UserRoles.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = userRole.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult Edit(int id) {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
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
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            var userRole = _userService.GetUserRoleById(model.Id);
            if (userRole == null)
                //No customer role found with the specified id
                return RedirectToAction("List");

            try {
                if (ModelState.IsValid) {
                    if (userRole.IsSystemRole && !model.Active)
                        throw new NutException(_localizationService.GetResource("Admin.Customers.CustomerRoles.Fields.Active.CantEditSystem"));

                    if (userRole.IsSystemRole && !userRole.SystemName.Equals(model.SystemName, StringComparison.InvariantCultureIgnoreCase))
                        throw new NutException(_localizationService.GetResource("Admin.Customers.CustomerRoles.Fields.SystemName.CantEditSystem"));

                    userRole = model.ToEntity(userRole);
                    _userService.UpdateUserRole(userRole);

                    //activity log
                    //_customerActivityService.InsertActivity("EditCustomerRole", _localizationService.GetResource("ActivityLog.EditCustomerRole"), userRole.Name);

                    SuccessNotification(_localizationService.GetResource("Admin.Customers.CustomerRoles.Updated"));
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
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            var customerRole = _userService.GetUserRoleById(id);
            if (customerRole == null)
                //No customer role found with the specified id
                return RedirectToAction("List");

            try {
                _userService.DeleteUserRole(customerRole);

                //activity log
                //_customerActivityService.InsertActivity("DeleteCustomerRole", _localizationService.GetResource("ActivityLog.DeleteCustomerRole"), customerRole.Name);

                //SuccessNotification(_localizationService.GetResource("Admin.Customers.CustomerRoles.Deleted"));

                return Json(new { success = true, message = "删除用户成功", });
            } catch (Exception exc) {
                ErrorNotification(exc.Message);
                return Json(new { success = false, message = "删除用户失败", });
            }

        }

        #endregion
    }
}