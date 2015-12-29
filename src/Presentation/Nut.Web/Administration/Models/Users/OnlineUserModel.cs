using Nut.Web.Framework;
using Nut.Web.Framework.Mvc;
using System;

namespace Nut.Admin.Models.Users {
    public class OnlineUserModel : BaseNutEntityModel {

        [NutResourceDisplayName("Admin.Users.OnlineUsers.Fields.UserInfo")]
        public string UserInfo { get; set; }

        [NutResourceDisplayName("Admin.Users.OnlineUsers.Fields.IPAddress")]
        public string LastIpAddress { get; set; }

        [NutResourceDisplayName("Admin.Users.OnlineUsers.Fields.LastActivityDate")]
        public DateTime LastActivityDate { get; set; }

    }
}