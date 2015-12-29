using System;
using System.Web;
using System.Web.Mvc;
using Nut.Core;
using Nut.Core.Data;
using Nut.Core.Domain.Localization;
using Nut.Core.Infrastructure;
using Nut.Web.Framework.Localization;

namespace Nut.Web.Framework
{
    /// <summary>
    /// Attribute which ensures that store URL contains a language SEO code if "SEO friendly URLs with multiple languages" setting is enabled
    /// </summary>
    public class LanguageSeoCodeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext == null || filterContext.HttpContext == null)
                return;

            HttpRequestBase request = filterContext.HttpContext.Request;
            if (request == null)
                return;

            //don't apply filter to child methods
            if (filterContext.IsChildAction)
                return;

            //only GET requests
            if (!String.Equals(filterContext.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
                return;

            if (!DataSettingsHelper.DatabaseIsInstalled())
                return;

            var localizationSettings = EngineContext.Current.Resolve<LocalizationSettings>();
            if (!localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
                return;
            
            //ensure that this route is registered and localizable (LocalizedRoute in RouteProvider.cs)
            if (filterContext.RouteData == null || filterContext.RouteData.Route == null || !(filterContext.RouteData.Route is LocalizedRoute))
                return;


            //process current URL
            var pageUrl = filterContext.HttpContext.Request.RawUrl;
            string applicationPath = filterContext.HttpContext.Request.ApplicationPath;
            if (pageUrl.IsLocalizedUrl(applicationPath, true))
                //already localized URL
                return;
            //add language code to URL
            var workContext = EngineContext.Current.Resolve<IWorkContext>();
            pageUrl = pageUrl.AddLanguageSeoCodeToRawUrl(applicationPath, workContext.WorkingLanguage);
            //301 (permanent) redirection
            filterContext.Result = new RedirectResult(pageUrl, true);
        }
    }
}
