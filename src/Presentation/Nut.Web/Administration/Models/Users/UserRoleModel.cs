using Nut.Web.Framework;
using Nut.Web.Framework.Mvc;
using FluentValidation.Attributes;
using Nut.Admin.Validators.Users;

namespace Nut.Admin.Models.Users {

    [Validator(typeof(UserRoleValidator))]
    public class UserRoleModel : BaseNutEntityModel {

        [NutResourceDisplayName("Admin.Users.UserRoles.Fields.Name")]
        public string Name { get; set; }

        [NutResourceDisplayName("Admin.Users.UserRoles.Fields.Active")]
        public bool Active { get; set; }

        [NutResourceDisplayName("Admin.Users.UserRoles.Fields.IsSystemRole")]
        public bool IsSystemRole { get; set; }

        [NutResourceDisplayName("Admin.Users.UserRoles.Fields.SystemName")]
        public string SystemName { get; set; }

        [NutResourceDisplayName("Admin.Users.UserRoles.Fields.IsSystemRole")]
        public string IsSystemRoleShow { get; set; }
    }
}