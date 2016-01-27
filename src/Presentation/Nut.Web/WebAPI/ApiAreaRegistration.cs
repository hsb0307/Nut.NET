using System.Web.Mvc;

namespace Nut.WebAPI {
    public class ApiAreaRegistration : AreaRegistration {
        public override string AreaName {
            get {
                return "Api";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) {
            context.MapRoute(
                "Api_default",
                "Api/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", area = "Api", id = "" },
                new[] { "Nut.WebAPI.Controllers" }
            );
        }
    }
}
