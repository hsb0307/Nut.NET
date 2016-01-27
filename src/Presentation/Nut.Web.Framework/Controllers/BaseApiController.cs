using System;
using System.Web.Mvc;
using Nut.Core;
using Nut.Core.Infrastructure;
using Nut.Core.Logging;

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
            return ErrorNotification("AccessDenied");
        }

        /// <summary>
        /// Successes the notification.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        protected ActionResult SuccessNotification(string message, object data) {

            return Json(new Notification {
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

            return Json(new Notification {
                Success = true,
                Message = message,
                Data = data
            },
                "text/plain", JsonRequestBehavior.AllowGet);
        }

        public class Notification {
            /// <summary>
            /// Gets or sets a value indicating whether this <see cref="Notification"/> is success.
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
