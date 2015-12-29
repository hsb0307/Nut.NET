using Nut.Admin.Models.Users;
using Nut.Services.Localization;
using Nut.Web.Framework.Validators;
using FluentValidation;

namespace Nut.Admin.Validators.Users {
    public class UserRoleValidator : BaseNutValidator<UserRoleModel> {
        public UserRoleValidator(ILocalizationService localizationService) {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Users.UserRoles.Fields.Name.Required"));
        }
    }
}