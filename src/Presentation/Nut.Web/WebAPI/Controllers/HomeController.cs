using Nut.Web.Framework.Controllers;
using System.Web.Mvc;

namespace Nut.WebAPI.Controllers {
    public class HomeController : BaseApiController {

        [HttpGet]
        // GET: Home
        public ActionResult Index() {
            return SuccessNotification("获取成功", new {
                pictureId = 1,
                imageUrl = "url"
            });
        }
    }
}