using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nut.Services.Security;
using Nut.Services.Users;
using Nut.Core.Domain.Users;
using Nut.Admin.Models.Users;
using Nut.Admin.Extensions;
using Nut.Web.Framework.Kendoui;
using Nut.Web.Framework.Mvc;
using Nut.Services.Helpers;
using Nut.Services.Localization;
using System.Text;
using Nut.Web.Framework.Controllers;
using Nut.Core;

namespace Nut.Admin.Controllers {
    public class UserController : BaseAdminController {

        #region Fields
        private readonly IPermissionService _permissionService;
        private readonly IUserService _userService;
        private readonly IDepartmentService _departmentService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly IUserRegistrationService _userRegistrationService;
        #endregion

        #region Ctor
        public UserController(IPermissionService permissionService,
            IUserService userService,
            IDepartmentService departmentService,
            IDateTimeHelper dateTimeHelper,
            ILocalizationService localizationService,
            IWorkContext workContext,
            IUserRegistrationService userRegistrationService) {
            this._permissionService = permissionService;
            this._userService = userService;
            this._departmentService = departmentService;
            this._dateTimeHelper = dateTimeHelper;
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._userRegistrationService = userRegistrationService;
        }
        #endregion

        [NonAction]
        protected virtual string GetUserRolesNames(IList<UserRole> userRoles, string separator = ",") {
            var sb = new StringBuilder();
            for (int i = 0; i < userRoles.Count; i++) {
                sb.Append(userRoles[i].Name);
                if (i != userRoles.Count - 1) {
                    sb.Append(separator);
                    sb.Append(" ");
                }
            }
            return sb.ToString();
        }

        [NonAction]
        protected virtual UserModel PrepareUserModelForList(User user) {
            return new UserModel {
                Id = user.Id,
                UserGuid = user.UserGuid,
                Email = user.Email,
                Username = user.Username,
                UserRoleNames = GetUserRolesNames(user.UserRoles.ToList()),
                Active = user.Active,
                DepartmentModel = user.Department.ToModel(),
                CreatedOnUtc = _dateTimeHelper.ConvertToUserTime(user.CreatedOnUtc),
                LastActivityDateUtc = _dateTimeHelper.ConvertToUserTime(user.LastActivityDateUtc),
            };
        }

        [NonAction]
        protected virtual void PrepareUserModel(UserModel model, User user, bool excludeProperties) {
            if (user != null) {
                model.Id = user.Id;
                if (!excludeProperties) {
                    model.Email = user.Email;
                    model.Username = user.Username;
                    model.AdminComment = user.AdminComment;
                    model.Active = user.Active;

                    model.CreatedOnUtc = _dateTimeHelper.ConvertToUserTime(user.CreatedOnUtc, DateTimeKind.Utc);
                    model.LastActivityDateUtc = _dateTimeHelper.ConvertToUserTime(user.LastActivityDateUtc, DateTimeKind.Utc);
                    model.LastIpAddress = user.LastIpAddress;

                    model.SelectedUserRoleIds = user.UserRoles.Select(cr => cr.Id).ToArray();
                }
            }

            //user roles
            model.AvailableUserRoles = _userService
                .GetAllUserRoles(true)
                .Select(cr => cr.ToModel())
                .ToList();
            //department
            foreach (var at in _departmentService.GetAll()) {
                model.AvailableDepartments.Add(new SelectListItem {
                    Value = at.Id.ToString(),
                    Text = at.GetFormattedBreadCrumb(_departmentService),
                    Selected = (user != null && user.DepartmentId == at.Id)
                });
            }
        }

        [NonAction]
        protected virtual string ValidateUserRoles(IList<UserRole> userRoles) {
            if (userRoles == null)
                throw new ArgumentNullException("customerRoles");

            //ensure a customer is not added to both 'Guests' and 'Registered' customer roles
            //ensure that a customer is in at least one required role ('Guests' and 'Registered')
            bool isInGuestsRole = userRoles.FirstOrDefault(cr => cr.SystemName == SystemUserRoleNames.Guests) != null;
            bool isInRegisteredRole = userRoles.FirstOrDefault(cr => cr.SystemName == SystemUserRoleNames.Registered) != null;
            if (isInGuestsRole && isInRegisteredRole)
                return "The customer cannot be in both 'Guests' and 'Registered' customer roles";
            if (!isInGuestsRole && !isInRegisteredRole)
                return "Add the customer to 'Guests' or 'Registered' customer role";

            //no errors
            return "";
        }

        #region Methods
        // GET: User
        public ActionResult Index() {
            return View();
        }

        public ActionResult List() {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            //load registered users by default
            var defaultRoleIds = new[] { _userService.GetUserRoleBySystemName(SystemUserRoleNames.Registered).Id };
            var model = new UserListModel {
                AvailableUserRoles = _userService.GetAllUserRoles(true).Select(cr => cr.ToModel()).ToList(),
                SearchUserRoleIds = defaultRoleIds
            };

            foreach (var at in _departmentService.GetAll()) {
                model.AvailableDepartments.Add(new SelectListItem {
                    Value = at.Id.ToString(),
                    Text = at.GetFormattedBreadCrumb(_departmentService)
                });
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult UserList(DataSourceRequest command, UserListModel model,
            [ModelBinder(typeof(CommaSeparatedModelBinder))] int[] searchUserRoleIds) {
            //we use own own binder for searchCustomerRoleIds property 
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var customers = _userService.GetAllUsers(
                UserRoleIds: searchUserRoleIds,
                departmentId: model.SearchDepartmentId,
                email: model.SearchEmail,
                username: model.SearchUsername,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize);
            var gridModel = new DataSourceResult {
                Data = customers.Select(PrepareUserModelForList),
                Total = customers.TotalCount
            };

            return Json(gridModel);
        }


        public ActionResult Create() {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var model = new UserModel();
            PrepareUserModel(model, null, false);
            //default value
            model.Active = true;
            model.Deleted = false;
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        [ValidateInput(false)]
        public ActionResult Create(UserModel model, bool continueEditing, FormCollection form) {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            if (!String.IsNullOrWhiteSpace(model.Username)) {
                var cust2 = _userService.GetUserByUsername(model.Username);
                if (cust2 != null)
                    ModelState.AddModelError("", "Username is already registered");
            }

            //validate customer roles
            var allUserRoles = _userService.GetAllUserRoles(true);
            var newUserRoles = new List<UserRole>();
            foreach (var customerRole in allUserRoles)
                if (model.SelectedUserRoleIds != null && model.SelectedUserRoleIds.Contains(customerRole.Id))
                    newUserRoles.Add(customerRole);
            var userRolesError = ValidateUserRoles(newUserRoles);
            if (!String.IsNullOrEmpty(userRolesError)) {
                ModelState.AddModelError("", userRolesError);
                ErrorNotification(userRolesError, false);
            }

            if (ModelState.IsValid) {
                var user = new User {
                    UserGuid = Guid.NewGuid(),
                    Email = model.Email,
                    Username = model.Username,
                    AdminComment = model.AdminComment,
                    Active = model.Active,
                    Deleted = model.Deleted,
                    DepartmentId = model.DepartmentId,
                    CreatedOnUtc = DateTime.UtcNow,
                    LastActivityDateUtc = DateTime.UtcNow,
                };
                _userService.InsertUser(user);


                //password
                if (!String.IsNullOrWhiteSpace(model.Password)) {
                    var changePassRequest = new ChangePasswordRequest(model.Username, false, PasswordFormat.Hashed, model.Password);
                    var changePassResult = _userRegistrationService.ChangePassword(changePassRequest);
                    if (!changePassResult.Success) {
                        foreach (var changePassError in changePassResult.Errors)
                            ErrorNotification(changePassError);
                    }
                }

                //customer roles
                foreach (var userRole in newUserRoles) {
                    //ensure that the current customer cannot add to "Administrators" system role if he's not an admin himself
                    if (userRole.SystemName == SystemUserRoleNames.Administrators &&
                        !_workContext.CurrentUser.IsAdmin())
                        continue;

                    user.UserRoles.Add(userRole);
                }
                _userService.UpdateUser(user);

                //activity log
                // _customerActivityService.InsertActivity("AddNewCustomer", _localizationService.GetResource("ActivityLog.AddNewCustomer"), user.Id);

                SuccessNotification(_localizationService.GetResource("Admin.Customers.Customers.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = user.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            PrepareUserModel(model, null, true);
            return View(model);

        }

        public ActionResult Edit(int id) {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var user = _userService.GetUserById(id);
            if (user == null || user.Deleted)
                //No customer found with the specified id
                return RedirectToAction("List");

            var model = new UserModel();
            PrepareUserModel(model, user, false);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        [ValidateInput(false)]
        public ActionResult Edit(UserModel model, bool continueEditing, FormCollection form) {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var user = _userService.GetUserById(model.Id);
            if (user == null || user.Deleted)
                //No customer found with the specified id
                return RedirectToAction("List");

            //validate customer roles
            var allUserRoles = _userService.GetAllUserRoles(true);
            var newUserRoles = new List<UserRole>();
            foreach (var userRole in allUserRoles)
                if (model.SelectedUserRoleIds != null && model.SelectedUserRoleIds.Contains(userRole.Id))
                    newUserRoles.Add(userRole);
            var userRolesError = ValidateUserRoles(newUserRoles);
            if (!String.IsNullOrEmpty(userRolesError)) {
                ModelState.AddModelError("", userRolesError);
                ErrorNotification(userRolesError, false);
            }

            if (ModelState.IsValid) {
                try {
                    user.AdminComment = model.AdminComment;
                    user.Active = model.Active;
                    //email
                    if (!String.IsNullOrWhiteSpace(model.Email)) {
                        _userRegistrationService.SetEmail(user, model.Email);
                    } else {
                        user.Email = model.Email;
                    }

                    //username
                    if (!String.IsNullOrWhiteSpace(model.Username)) {
                        _userRegistrationService.SetUsername(user, model.Username);
                    } else {
                        user.Username = model.Username;
                    }

                  

                    //customer roles
                    foreach (var userRole in allUserRoles) {
                        //ensure that the current customer cannot add/remove to/from "Administrators" system role
                        //if he's not an admin himself
                        if (userRole.SystemName == SystemUserRoleNames.Administrators &&
                            !_workContext.CurrentUser.IsAdmin())
                            continue;

                        if (model.SelectedUserRoleIds != null &&
                            model.SelectedUserRoleIds.Contains(userRole.Id)) {
                            //new role
                            if (user.UserRoles.Count(cr => cr.Id == userRole.Id) == 0)
                                user.UserRoles.Add(userRole);
                        } else {
                            //remove role
                            if (user.UserRoles.Count(cr => cr.Id == userRole.Id) > 0)
                                user.UserRoles.Remove(userRole);
                        }
                    }
                    _userService.UpdateUser(user);

                    //password
                    if (!String.IsNullOrWhiteSpace(model.Password)) {
                        var changePassRequest = new ChangePasswordRequest(model.Username, false, PasswordFormat.Hashed, model.Password);
                        var changePassResult = _userRegistrationService.ChangePassword(changePassRequest);
                        if (!changePassResult.Success) {
                            foreach (var changePassError in changePassResult.Errors)
                                ErrorNotification(changePassError);
                        }
                    }

                    //activity log
                    //_customerActivityService.InsertActivity("EditCustomer", _localizationService.GetResource("ActivityLog.EditCustomer"), user.Id);

                    SuccessNotification(_localizationService.GetResource("Admin.Customers.Customers.Updated"));
                    if (continueEditing) {
                        return RedirectToAction("Edit", new { id = user.Id });
                    }
                    return RedirectToAction("List");
                } catch (Exception exc) {
                    ErrorNotification(exc.Message, false);
                }
            }


            //If we got this far, something failed, redisplay form
            PrepareUserModel(model, user, true);
            return View(model);
        }



        [HttpPost]
        public ActionResult Delete(int id) {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var user = _userService.GetUserById(id);
            if (user == null)
                //No customer found with the specified id
                return RedirectToAction("List");

            try {
                _userService.DeleteUser(user);

                //activity log
                // _customerActivityService.InsertActivity("DeleteCustomer", _localizationService.GetResource("ActivityLog.DeleteCustomer"), user.Id);

                return Json(new { success = true, message = _localizationService.GetResource("Admin.Users.Deleted") });
            } catch (Exception exc) {
                ErrorNotification(exc.Message);
                return RedirectToAction("Edit", new { id = user.Id });
            }
        }
        #endregion
    }
}