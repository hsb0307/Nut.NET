using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nut.Web.Framework;
using Nut.Web.Framework.Mvc;
using Nut.Web.Validators.User;

namespace Nut.Web.Models.User
{
    [Validator(typeof(LoginValidator))]
    public partial class LoginModel : BaseNutModel
    {
        public bool CheckoutAsGuest { get; set; }

        [NutResourceDisplayName("Account.Login.Fields.UserName")]
        [AllowHtml]
        public string Username { get; set; }

        [DataType(DataType.Password)]
        [NutResourceDisplayName("Account.Login.Fields.Password")]
        [AllowHtml]
        public string Password { get; set; }

        [NutResourceDisplayName("Account.Login.Fields.RememberMe")]
        public bool RememberMe { get; set; }

    }
}