using FluentValidation;
using Nut.Core.Domain.Users;
using Nut.Services.Localization;
using Nut.Web.Framework.Validators;
using Nut.Web.Models.User;

namespace Nut.Web.Validators.User
{
    public class LoginValidator : BaseNutValidator<LoginModel>
    {
        public LoginValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Username).NotEmpty().WithMessage(localizationService.GetResource("Account.Login.Fields.Username.Required"));
        }
    }
}