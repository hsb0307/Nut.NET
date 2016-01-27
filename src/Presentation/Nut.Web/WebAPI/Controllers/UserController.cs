using System;
using System.Web.Mvc;
using Nut.Web.Framework.Controllers;
using Nut.Services.Users;
using Nut.Services.Logging;
using Nut.Services.Authentication;
using Nut.Services.Localization;
using Nut.Core.Domain.Users;
using System.Web.Security;
using Nut.Services.Security;
using Nut.Core.Domain.Common;

namespace Nut.WebAPI.Controllers {
    public class UserController : BaseApiController {

        private readonly IUserRegistrationService _userRegistrationService;
        private readonly IActivityLogService _activityLogService;
        private readonly IUserService _userService;
        private readonly ILocalizationService _localizationService;
        private readonly IEncryptionService _encryptionService;
        private readonly WebAPISettings _webAPISettings;

        public UserController(IUserRegistrationService userRegistrationService,
            IActivityLogService activityLogService,
            IUserService userService,
            IAuthenticationService authenticationService,
            ILocalizationService localizationService,
            IEncryptionService encryptionService,
            WebAPISettings webAPISettings) {
            this._userRegistrationService = userRegistrationService;
            this._activityLogService = activityLogService;
            this._userService = userService;
            this._localizationService = localizationService;
            this._encryptionService = encryptionService;
            this._webAPISettings = webAPISettings;
        }

        // GET: User
        public ActionResult Login(string username, string password) {

            if (string.IsNullOrEmpty(username))
                return ErrorNotification(_localizationService.GetResource("Account.Login.Fields.Username.Required"));

            if (string.IsNullOrEmpty(password))
                return ErrorNotification(_localizationService.GetResource("Account.Login.Fields.Password.Required"));

            username = username.Trim();

            var loginResult = _userRegistrationService.ValidateUser(username, password);
            var message = "";
            switch (loginResult) {
                case UserLoginResults.Successful:
                    {
                        var user = _userService.GetUserByUsername(username);
                        //activity log
                        _activityLogService.InsertActivity("PublicStore.Login", _localizationService.GetResource("ActivityLog.PublicStore.Login"), user);
                        //user ticket
                        var UserTicket = _encryptionService.CreatePasswordHash(user.UserGuid.ToString(), _webAPISettings.UserEncryptionKey);

                        return SuccessNotification(_localizationService.GetResource("Account.Login.Success"), UserTicket);
                    }
                case UserLoginResults.CustomerNotExist:
                    message = _localizationService.GetResource("Account.Login.WrongCredentials.CustomerNotExist");
                    break;
                case UserLoginResults.Deleted:
                    message = _localizationService.GetResource("Account.Login.WrongCredentials.Deleted");
                    break;
                case UserLoginResults.NotActive:
                    message = _localizationService.GetResource("Account.Login.WrongCredentials.NotActive");
                    break;
                case UserLoginResults.NotRegistered:
                    message = _localizationService.GetResource("Account.Login.WrongCredentials.NotRegistered");
                    break;
                case UserLoginResults.WrongPassword:
                default:
                    message = _localizationService.GetResource("Account.Login.WrongCredentials");
                    break;
            }
            return ErrorNotification(message);
        }
    }

}