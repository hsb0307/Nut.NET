using Nut.Services.Helpers;
using Nut.Services.Localization;
using Nut.Services.Security;
using Nut.Services.Users;
using Nut.Admin.Models.Users;
using Nut.Web.Framework.Kendoui;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Nut.Admin.Controllers {
    public class OnlineUserController : BaseAdminController {
        #region Fields

        private readonly IUserService _userService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;

        #endregion

        #region Constructors

        public OnlineUserController(IUserService userService,
             IDateTimeHelper dateTimeHelper,
            IPermissionService permissionService, ILocalizationService localizationService) {
            this._userService = userService;
            this._dateTimeHelper = dateTimeHelper;
            this._permissionService = permissionService;
            this._localizationService = localizationService;
        }

        #endregion

        #region Methods

        public ActionResult List() {
            //if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
            //    return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public ActionResult List(DataSourceRequest command) {
            //if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
            //    return AccessDeniedView();

            var customers = _userService.GetOnlineUsers(DateTime.UtcNow.AddMinutes(-30),
                null, command.Page - 1, command.PageSize);
            var gridModel = new DataSourceResult {
                Data = customers.Select(x => new OnlineUserModel {
                    Id = x.Id,
                    UserInfo = x.Email,
                    LastIpAddress = x.LastIpAddress,
                    LastActivityDate = _dateTimeHelper.ConvertToUserTime(x.LastActivityDateUtc, DateTimeKind.Utc)
                }),
                Total = customers.TotalCount
            };

            return Json(gridModel);
        }

        #endregion
    }
}