using System;
using System.Linq;
using System.Web.Mvc;
using Nut.Core;
using Nut.Plugin.APP.Version.Domain;
using Nut.Plugin.APP.Version.Services;
using Nut.Services.Localization;
using Nut.Services.Logging;
using Nut.Services.Security;
using Nut.Core.Exceptions;
using Nut.Web.Framework.Controllers;
using Nut.Web.Framework.Kendoui;


namespace Nut.Plugin.APP.Version.Controllers {
    public class AppVersionController : BasePluginController {
        #region Fields

        private readonly IAppVersionService _appVersionService;
        private readonly ILocalizationService _localizationService;
        private readonly IActivityLogService _activityLogService;
        private readonly IPermissionService _permissionService;
        private readonly IExceptionPolicy _exceptionPolicy;

        #endregion Fields

        #region Constructors

        public AppVersionController(IAppVersionService appVersionService,
            ILocalizationService localizationService, IActivityLogService activityLogService,
            IPermissionService permissionService, IExceptionPolicy exceptionPolicy) {
            this._appVersionService = appVersionService;

            this._localizationService = localizationService;
            this._activityLogService = activityLogService;
            this._permissionService = permissionService;
            this._exceptionPolicy = exceptionPolicy;
        }

        #endregion


        #region Methods

        //list
        public ActionResult Index() {
            return RedirectToAction("List");
        }

        public ActionResult List() {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return Content("Access denied");

            return View("~/Plugins/APP.Version/Views/AppVersion/List.cshtml");
        }

        [HttpPost]
        public ActionResult List(DataSourceRequest command) {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return Content("Access denied");

            var appVersions = _appVersionService
                .GetPaged(command.Page - 1, command.PageSize);
            var gridModel = new DataSourceResult {
                Data = appVersions,
                Total = appVersions.TotalCount
            };

            return Json(gridModel);
        }

        //create
        public ActionResult Create() {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return Content("Access denied");

            var model = new AppVersion();

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Create(AppVersion model, bool continueEditing) {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return Content("Access denied");

            if (ModelState.IsValid) {
                var appVersion = model;
                appVersion.CreateON = DateTime.Now;

                _appVersionService.Insert(appVersion);

                //activity log
                //_activityLogService.InsertActivity("AddNewAppVersion", _localizationService.GetResource("ActivityLog.AddNewAppVersion"), appVersion.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Attributes.AppVersions.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = appVersion.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        //edit
        public ActionResult Edit(int id) {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return Content("Access denied");

            var appVersion = _appVersionService.GetById(id);
            if (appVersion == null)
                //No product attribute found with the specified id
                return RedirectToAction("List");

            var model = appVersion;

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Edit(AppVersion model, bool continueEditing) {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return Content("Access denied");

            var appVersion = _appVersionService.GetById(model.Id);
            if (appVersion == null)
                //No product attribute found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid) {
                appVersion = model;

                _appVersionService.Update(appVersion);


                //activity log
               // _activityLogService.InsertActivity("EditAppVersion", _localizationService.GetResource("ActivityLog.EditAppVersion"), appVersion.Name);

                SuccessNotification(_localizationService.GetResource("Admin.AppVersions.Updated"));
                return continueEditing ? RedirectToAction("Edit", appVersion.Id) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        //delete
        [HttpPost]
        public ActionResult Delete(int id) {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return Content("Access denied");

            var appVersion = _appVersionService.GetById(id);
            if (appVersion == null)
                //No product attribute found with the specified id
                return RedirectToAction("List");

            try {
                _appVersionService.Delete(appVersion);

                //activity log
               // _activityLogService.InsertActivity("DeleteAppVersion", _localizationService.GetResource("ActivityLog.DeleteAppVersion"), appVersion.Name);

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
