using System.Collections.Generic;
using System.Web.Mvc;
using Nut.Web.Framework;
using Nut.Web.Framework.Mvc;

namespace Nut.Admin.Models.Users {
    public class UserListModel : BaseNutModel {

        public UserListModel() {
            AvailableDepartments = new List<SelectListItem>();
        }

        [NutResourceDisplayName("Admin.Users.Users.List.UserRoles")]
        [AllowHtml]
        public List<UserRoleModel> AvailableUserRoles { get; set; }

        [NutResourceDisplayName("Admin.Users.Users.List.UserRoles")]
        public int[] SearchUserRoleIds { get; set; }

        [NutResourceDisplayName("Admin.Users.Users.List.SearchUsername")]
        [AllowHtml]
        public string SearchUsername { get; set; }

        [NutResourceDisplayName("Admin.Users.Users.List.SearchEmail")]
        [AllowHtml]
        public string SearchEmail { get; set; }

        [NutResourceDisplayName("Admin.Users.Users.List.Departments")]
        [AllowHtml]
        public List<SelectListItem> AvailableDepartments { get; set; }

        [NutResourceDisplayName("Admin.Users.Users.List.Departments")]
        public int SearchDepartmentId { get; set; }


    }
}