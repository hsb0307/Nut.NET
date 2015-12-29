using System;
using System.Web.Mvc;
using Nut.Core;
using Nut.Core.Data;
using Nut.Core.Infrastructure;
using Nut.Services.Users;

namespace Nut.Web.Framework
{
    public class StoreIpAddressAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!DataSettingsHelper.DatabaseIsInstalled())
                return;

            if (filterContext == null || filterContext.HttpContext == null || filterContext.HttpContext.Request == null)
                return;

            //don't apply filter to child methods
            if (filterContext.IsChildAction)
                return;

            //only GET requests
            if (!String.Equals(filterContext.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
                return;

            var webHelper = EngineContext.Current.Resolve<IWebHelper>();

            //update IP address
            string currentIpAddress = webHelper.GetCurrentIpAddress();
            if (!String.IsNullOrEmpty(currentIpAddress))
            {
                var workContext = EngineContext.Current.Resolve<IWorkContext>();
                var user = workContext.CurrentUser;
                if (!currentIpAddress.Equals(user.LastIpAddress, StringComparison.InvariantCultureIgnoreCase))
                {
                    var userService = EngineContext.Current.Resolve<IUserService>();
                    user.LastIpAddress = currentIpAddress;
                    userService.UpdateUser(user);
                }
            }
        }
    }
}
