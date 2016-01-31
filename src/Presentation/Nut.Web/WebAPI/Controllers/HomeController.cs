using Nut.Web.Framework.Controllers;
using System.Collections.Generic;
using System.Web.Mvc;
using Nut.Core.Message;

namespace Nut.WebAPI.Controllers {
    public class HomeController : BaseApiController {

        private readonly IMessageService _messageService;

        public HomeController(
           IMessageService messageService
           ) {
            _messageService = messageService;
        }

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

        [HttpGet]
        // GET: Home
        public ActionResult JPush() {

            var parameters = new Dictionary<string, object> {
                {"Subject", "Subject"},
                {"Body", "Body"},
                {"Recipients", "Recipients"}
            };

            //_messageService.Send("JPush", parameters);

            return SuccessNotification("获取成功", null);
        }
    }
}