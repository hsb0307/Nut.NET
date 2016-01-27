using System.Web.Mvc;

namespace Nut.WebAPI.Controllers {
    public class HomeController : Controller {

        [HttpGet]
        // GET: Home
        public ActionResult Index() {
            return Json(new {
                success = true, pictureId = 1,
                imageUrl = "url"
            },
                "text/plain", JsonRequestBehavior.AllowGet);
        }
    }
}