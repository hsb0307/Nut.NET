using System.Web.Mvc;
using System.Web.Routing;
using Nut.Core.Infrastructure;
using Nut.Web.Framework;
using Nut.Web.Framework.Controllers;
using Nut.Web.Framework.Security;
using Nut.Web.Framework.Seo;

namespace Nut.Web.Controllers
{
    [LanguageSeoCode]
    [NopHttpsRequirement(SslRequirement.NoMatter)]
    [WwwRequirement]
    public abstract partial class BasePublicController : BaseController
    {
        protected virtual ActionResult InvokeHttp404()
        {
            // Call target Controller and pass the routeData.
            IController errorController = EngineContext.Current.Resolve<CommonController>();

            var routeData = new RouteData();
            routeData.Values.Add("controller", "Common");
            routeData.Values.Add("action", "PageNotFound");

            errorController.Execute(new RequestContext(this.HttpContext, routeData));

            return new EmptyResult();
        }

    }
}
