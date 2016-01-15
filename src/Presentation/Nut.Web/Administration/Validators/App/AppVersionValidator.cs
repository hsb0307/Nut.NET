using FluentValidation;
using Nut.Admin.Models.App;
using Nut.Services.Localization;

namespace Nut.Admin.Validators.App {
    public class AppVersionValidator : AbstractValidator<AppVersionModel> {
        public AppVersionValidator(ILocalizationService localizationService) {
            RuleFor(x => x.VersionNum).NotEmpty().WithMessage(localizationService.GetResource("Admin.AppVersions.Fields.VersionNum.Required"));
            RuleFor(x => x.Version).NotEmpty().WithMessage(localizationService.GetResource("Admin.AppVersions.Fields.Version.Required"));
            RuleFor(x => x.DownloadId).NotEmpty().WithMessage(localizationService.GetResource("Admin.AppVersions.Fields.DownloadURL.Required"));
        }
    }
}