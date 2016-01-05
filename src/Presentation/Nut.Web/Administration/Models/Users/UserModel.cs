using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nut.Core.Domain.Users;
using Nut.Web.Framework;
using Nut.Web.Framework.Mvc;


namespace Nut.Admin.Models.Users {
    public class UserModel :BaseNutEntityModel{

        public UserModel() {
            AvailableDepartments = new List<SelectListItem>();
        }

        [NutResourceDisplayName("Admin.Users.Fields.UserGuid")]
        //[AllowHtml]
        public System.Guid UserGuid { get; set; }
        [NutResourceDisplayName("Admin.Users.Fields.Username")]
        //[AllowHtml]
        public System.String Username { get; set; }
        [NutResourceDisplayName("Admin.Users.Fields.Email")]
        //[AllowHtml]
        public System.String Email { get; set; }
        [NutResourceDisplayName("Admin.Users.Fields.Password")]
        //[AllowHtml]
        public System.String Password { get; set; }
        [NutResourceDisplayName("Admin.Users.Fields.PasswordFormatId")]
        //[AllowHtml]
        public System.Int32 PasswordFormatId { get; set; }
        [NutResourceDisplayName("Admin.Users.Fields.PasswordSalt")]
        //[AllowHtml]
        public System.String PasswordSalt { get; set; }
        [NutResourceDisplayName("Admin.Users.Fields.AdminComment")]
        //[AllowHtml]
        public System.String AdminComment { get; set; }
        [NutResourceDisplayName("Admin.Users.Fields.Active")]
        //[AllowHtml]
        public System.Boolean Active { get; set; }
        [NutResourceDisplayName("Admin.Users.Fields.Deleted")]
        //[AllowHtml]
        public System.Boolean Deleted { get; set; }
        [NutResourceDisplayName("Admin.Users.Fields.IsSystemAccount")]
        //[AllowHtml]
        public System.Boolean IsSystemAccount { get; set; }
        [NutResourceDisplayName("Admin.Users.Fields.LastIpAddress")]
        //[AllowHtml]
        public System.String LastIpAddress { get; set; }
        [NutResourceDisplayName("Admin.Users.Fields.CreatedOnUtc")]
        //[AllowHtml]
        public System.DateTime CreatedOnUtc { get; set; }
        [NutResourceDisplayName("Admin.Users.Fields.LastLoginDateUtc")]
        //[AllowHtml]
        public System.DateTime LastLoginDateUtc { get; set; }
        [NutResourceDisplayName("Admin.Users.Fields.LastActivityDateUtc")]
        //[AllowHtml]
        public System.DateTime LastActivityDateUtc { get; set; }
        [NutResourceDisplayName("Admin.Users.Fields.DepartmentId")]
        //[AllowHtml]
        public System.Int32 DepartmentId { get; set; }

        public DepartmentModel DepartmentModel { get; set; }

        [NutResourceDisplayName("Admin.Users.Users.List.Departments")]
        [AllowHtml]
        public List<SelectListItem> AvailableDepartments { get; set; }

        //customer roles
        [NutResourceDisplayName("Admin.Users.Users.List.UserRoles")]
        public string UserRoleNames { get; set; }
        public List<UserRoleModel> AvailableUserRoles { get; set; }
        public int[] SelectedUserRoleIds { get; set; }
    }
}