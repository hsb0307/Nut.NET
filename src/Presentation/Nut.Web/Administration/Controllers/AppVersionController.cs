using System;
using System.Linq;
using System.Web.Mvc;
using Nut.Core;
using Nut.Admin.Models.App;
using Nut.Core.Domain.App;
using Nut.Services.App;
using Nut.Services.Localization;
using Nut.Services.Logging;
using Nut.Services.Security;
using Nut.Core.Exceptions;
using Nut.Web.Framework.Controllers;
using Nut.Web.Framework.Kendoui;
using Nut.Admin.Extensions;
using Nut.Services.Helpers;

namespace Nut.Admin.Controllers {

    public class AppVersionController : BaseAdminController {
        #region Fields

        private readonly IAppVersionService _appVersionService;
        private readonly ILocalizationService _localizationService;
        private readonly IActivityLogService _activityLogService;
        private readonly IPermissionService _permissionService;
        private readonly IExceptionPolicy _exceptionPolicy;
        private readonly IDateTimeHelper _dateTimeHelper;

        #endregion Fields

        #region Constructors

        public AppVersionController(IAppVersionService appVersionService,
            ILocalizationService localizationService, IActivityLogService activityLogService,
            IPermissionService permissionService, IExceptionPolicy exceptionPolicy,
            IDateTimeHelper dateTimeHelper) {
            this._appVersionService = appVersionService;

            this._localizationService = localizationService;
            this._activityLogService = activityLogService;
            this._permissionService = permissionService;
            this._exceptionPolicy = exceptionPolicy;
            this._dateTimeHelper = dateTimeHelper;
        }

        #endregion


        #region Methods

        //list
        public ActionResult Index() {
            return RedirectToAction("List");
        }

        public ActionResult List() {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAppVersions))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public ActionResult List(DataSourceRequest command) {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAppVersions))
                return AccessDeniedView();

            var appVersions = _appVersionService
                .GetPaged(command.Page - 1, command.PageSize);
            var gridModel = new DataSourceResult {
                Data = appVersions.Select(x => {
                    var model = x.ToModel();
                    model.CreateON = _dateTimeHelper.ConvertToUserTime(model.CreateON);
                    return model;
                    }),
                Total = appVersions.TotalCount
            };

            return Json(gridModel);
        }

        //create
        public ActionResult Create() {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAppVersions))
                return AccessDeniedView();

            var model = new AppVersionModel();

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Create(AppVersionModel model, bool continueEditing) {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAppVersions))
                return AccessDeniedView();

            if (ModelState.IsValid) {
                var appVersion = model.ToEntity();
                appVersion.CreateON = DateTime.Now;

                _appVersionService.Insert(appVersion);

                //activity log
                _activityLogService.InsertActivity("AddNewAppVersion", _localizationService.GetResource("ActivityLog.AddNewAppVersion"), appVersion.VersionNum);

                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Attributes.AppVersions.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = appVersion.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        //edit
        public ActionResult Edit(int id) {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAppVersions))
                return AccessDeniedView();

            var appVersion = _appVersionService.GetById(id);
            if (appVersion == null)
                //No product attribute found with the specified id
                return RedirectToAction("List");

            var model = appVersion.ToModel();

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Edit(AppVersionModel model, bool continueEditing) {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAppVersions))
                return AccessDeniedView();

            var appVersion = _appVersionService.GetById(model.Id);
            if (appVersion == null)
                //No product attribute found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid) {
                appVersion = model.ToEntity(appVersion);

                _appVersionService.Update(appVersion);


                //activity log
                _activityLogService.InsertActivity("EditAppVersion", _localizationService.GetResource("ActivityLog.EditAppVersion"), appVersion.VersionNum);

                SuccessNotification(_localizationService.GetResource("Admin.AppVersions.Updated"));
                return continueEditing ? RedirectToAction("Edit", appVersion.Id) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        //delete
        [HttpPost]
        public ActionResult Delete(int id) {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAppVersions))
                return AccessDeniedView();

            var appVersion = _appVersionService.GetById(id);
            if (appVersion == null)
                //No product attribute found with the specified id
                return RedirectToAction("List");

            try {
                _appVersionService.Delete(appVersion);

                //activity log
                _activityLogService.InsertActivity("DeleteAppVersion", _localizationService.GetResource("ActivityLog.DeleteAppVersion"), appVersion.VersionNum);

                return Json(new { success = true, message = _localizationService.GetResource("Admin.Users.UserRoles.Deleted") });
            } catch (Exception exc) {
                if (!_exceptionPolicy.HandleException(this, exc))
                    throw;
                return Json(new { success = false, message = _localizationService.GetResource("Admin.AppVersions.UnDeleted"), });
            }
        }

        #endregion
    }
}