using System;
using System.Web.Mvc;
using Nut.Core;
using Nut.Core.Infrastructure;
using Nut.Services.Localization;
using Nut.Core.Logging;
using Nut.Core.Domain.Users;
using Nut.Services.Security;
using Nut.Services.Configuration;
using Nut.Core.Domain.Common;
using Nut.Services.Users;

namespace Nut.Web.Framework.Controllers {
    public abstract partial class BaseApiController : Controller {
        /// <summary>
        /// On exception
        /// </summary>
        /// <param name="filterContext">Filter context</param>
        protected override void OnException(ExceptionContext filterContext) {
            if (filterContext.Exception != null)
                LogException(filterContext.Exception.Message);
            base.OnException(filterContext);
        }

        /// <summary>
        /// Log exception
        /// </summary>
        /// <param name="exc">Exception</param>
        protected void LogException(string exc) {
            var workContext = EngineContext.Current.Resolve<IWorkContext>();
            var logger = EngineContext.Current.Resolve<ILogger>();

            var user = workContext.CurrentUser;
            logger.Error(exc, exc, user);
        }

        /// <summary>
        /// Access denied view
        /// </summary>
        /// <returns>Access denied view</returns>
        protected ActionResult AccessDeniedView() {
            var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
            return ErrorNotification(localizationService.GetResource("Admin.AccessDenied.Description"));
        }

        /// <summary>
        /// APIs the user access.
        /// </summary>
        /// <param name="userTicket">The user ticket.</param>
        /// <returns></returns>
        public virtual User ApiUserAccess(string userTicket) {

            if (string.IsNullOrEmpty(userTicket))
                return null;

            //userTicket = Server.UrlDecode(userTicket);

            var encryptionService = EngineContext.Current.Resolve<IEncryptionService>();
            var settingService = EngineContext.Current.Resolve<ISettingService>();
            var userService = EngineContext.Current.Resolve<IUserService>();

            var webAPISettings = settingService.LoadSetting<WebAPISettings>();

            var userGuid = encryptionService.DecryptText(userTicket, webAPISettings.UserEncryptionKey);

            if (string.IsNullOrEmpty(userGuid))
                return null;
            var user = userService.GetUserByGuid(Guid.Parse(userGuid));
            return user;
        }


        /// <summary>
        /// Successes the notification.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        protected ActionResult SuccessNotification(string message, object data) {

            return Json(new ApiNotification {
                Success = true,
                Message = message,
                Data = data
            },
                "text/plain", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Errors the notification.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="logException">if set to <c>true</c> [log exception].</param>
        /// <returns></returns>
        protected ActionResult ErrorNotification(string message, bool logException = true) {
            return ErrorNotification(message, null, logException);
        }
        /// <summary>
        /// Errors the notification.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="data">The data.</param>
        /// <param name="logException">if set to <c>true</c> [log exception].</param>
        /// <returns></returns>
        protected ActionResult ErrorNotification(string message, object data, bool logException = true) {
            if (logException)
                LogException(message);

            return Json(new ApiNotification {
                Success = false,
                Message = message,
                Data = data
            },
                "text/plain", JsonRequestBehavior.AllowGet);
        }

        public class ApiNotification {
            /// <summary>
            /// Gets or sets a value indicating whether this <see cref="ApiNotification"/> is success.
            /// </summary>
            public bool Success { get; set; }

            /// <summary>
            /// Gets or sets the message.
            /// </summary>
            public string Message { get; set; }

            /// <summary>
            /// Gets or sets the data.
            /// </summary>
            public Object Data { get; set; }
        }
    }
}
