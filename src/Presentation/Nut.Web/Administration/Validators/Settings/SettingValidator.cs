using FluentValidation;
using Nut.Admin.Models.Settings;
using Nut.Services.Localization;
using Nut.Web.Framework.Validators;

namespace Nut.Admin.Validators.Settings
{
    public class SettingValidator : BaseNutValidator<SettingModel>
    {
        public SettingValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Configuration.Settings.AllSettings.Fields.Name.Required"));
        }
    }
}