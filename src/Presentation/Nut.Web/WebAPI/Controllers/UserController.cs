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
using Nut.Services.App;

namespace Nut.WebAPI.Controllers {
    public class UserController : BaseApiController {

        private readonly IUserRegistrationService _userRegistrationService;
        private readonly IActivityLogService _activityLogService;
        private readonly IUserService _userService;
        private readonly IJPushUserService _jPushUserService;
        private readonly ILocalizationService _localizationService;
        private readonly IEncryptionService _encryptionService;
        private readonly WebAPISettings _webAPISettings;

        public UserController(IUserRegistrationService userRegistrationService,
            IActivityLogService activityLogService,
            IUserService userService,
            IJPushUserService jPushUserService,
            IAuthenticationService authenticationService,
            ILocalizationService localizationService,
            IEncryptionService encryptionService,
            WebAPISettings webAPISettings) {
            this._userRegistrationService = userRegistrationService;
            this._activityLogService = activityLogService;
            this._userService = userService;
            this._jPushUserService = jPushUserService;
            this._localizationService = localizationService;
            this._encryptionService = encryptionService;
            this._webAPISettings = webAPISettings;
        }

        // GET: User
        public ActionResult Login(string username, string password, string registerId) {

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
                        //JPush register ID
                        if (!string.IsNullOrEmpty(registerId)) {
                            _jPushUserService.Delete(registerId);

                            _jPushUserService.Insert(new Core.Domain.App.JPushUser() {
                                CreatedOn = DateTime.Now,
                                RegisterId = registerId,
                                User = user,
                                UserId = user.Id
                            });
                        }
                        //user ticket
                        var userTicket = _encryptionService.EncryptText(user.UserGuid.ToString(), _webAPISettings.UserEncryptionKey);

                        return SuccessNotification(_localizationService.GetResource("Account.Login.Success"), Server.UrlEncode(userTicket));
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