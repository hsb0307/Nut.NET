using Nut.Admin.Extensions;
using Nut.Admin.Models.Logging;
using Nut.Services.Helpers;
using Nut.Services.Localization;
using Nut.Services.Logging;
using Nut.Services.Security;
using Nut.Web.Framework.Kendoui;
using Nut.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nut.Admin.Controllers
{
    public class ActivityLogController : BaseAdminController
    {
        #region Fields

        private readonly IActivityLogService _activityLogService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;

        #endregion Fields

        #region Constructors

        public ActivityLogController(IActivityLogService activityLogService,
            IDateTimeHelper dateTimeHelper, ILocalizationService localizationService,
            IPermissionService permissionService) {
            this._activityLogService = activityLogService;
            this._dateTimeHelper = dateTimeHelper;
            this._localizationService = localizationService;
            this._permissionService = permissionService;
        }

        #endregion

        #region Activity log types

        public ActionResult ListTypes() {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageActivityLog))
                return AccessDeniedView();

            var model = _activityLogService
                .GetAllActivityTypes()
                .Select(x => x.ToModel())
                .ToList();
            return View(model);
        }

        [HttpPost]
        public ActionResult SaveTypes(FormCollection form) {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageActivityLog))
                return AccessDeniedView();

            string formKey = "checkbox_activity_types";
            var checkedActivityTypes = form[formKey] != null ? form[formKey].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => Convert.ToInt32(x)).ToList() : new List<int>();

            var activityTypes = _activityLogService.GetAllActivityTypes();
            foreach (var activityType in activityTypes) {
                activityType.Enabled = checkedActivityTypes.Contains(activityType.Id);
                _activityLogService.UpdateActivityType(activityType);
            }
            SuccessNotification(_localizationService.GetResource("Admin.Configuration.ActivityLog.ActivityLogType.Updated"));
            return RedirectToAction("ListTypes");
        }

        #endregion

        #region Activity log

        public ActionResult ListLogs() {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageActivityLog))
                return AccessDeniedView();

            var activityLogSearchModel = new ActivityLogSearchModel();
            activityLogSearchModel.ActivityLogType.Add(new SelectListItem {
                Value = "0",
                Text = "All"
            });


            foreach (var at in _activityLogService.GetAllActivityTypes()) {
                activityLogSearchModel.ActivityLogType.Add(new SelectListItem {
                    Value = at.Id.ToString(),
                    Text = at.Name
                });
            }
            return View(activityLogSearchModel);
        }

        [HttpPost]
        public ActionResult ListLogs(DataSourceRequest command, ActivityLogSearchModel model) {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageActivityLog))
                return AccessDeniedView();

            DateTime? startDateValue = (model.CreatedOnFrom == null) ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.CreatedOnFrom.Value, _dateTimeHelper.DefaultStoreTimeZone);

            DateTime? endDateValue = (model.CreatedOnTo == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.CreatedOnTo.Value, _dateTimeHelper.DefaultStoreTimeZone).AddDays(1);

            var activityLog = _activityLogService.GetAllActivities(startDateValue, endDateValue, null, model.ActivityLogTypeId, command.Page - 1, command.PageSize);
            var gridModel = new DataSourceResult {
                Data = activityLog.Select(x => {
                    var m = x.ToModel();
                    m.CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc);
                    return m;

                }),
                Total = activityLog.TotalCount
            };
            return Json(gridModel);
        }

        public ActionResult AcivityLogDelete(int id) {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageActivityLog))
                return AccessDeniedView();

            var activityLog = _activityLogService.GetActivityById(id);
            if (activityLog == null) {
                throw new ArgumentException("No activity log found with the specified id");
            }
            _activityLogService.DeleteActivity(activityLog);

            return new NullJsonResult();
        }

        public ActionResult ClearAll() {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageActivityLog))
                return AccessDeniedView();

            _activityLogService.ClearAllActivities();
            return RedirectToAction("ListLogs");
        }

        #endregion
    }
}