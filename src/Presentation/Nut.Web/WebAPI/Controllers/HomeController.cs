using Nut.Web.Framework.Controllers;
using System.Web.Mvc;

namespace Nut.WebAPI.Controllers {
    public class HomeController : BaseApiController {

        [HttpGet]
        // GET: Home
        public ActionResult Index(string userTicket) {
            var user = ApiUserAccess(userTicket);

            if (user == null)
                return ErrorNotification("验证失败");

            return SuccessNotification("获取成功", new {
                pictureId = 1,
                imageUrl = "url"
            });
        }
    }
}