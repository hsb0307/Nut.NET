using System;
using FluentValidation;
using Nut.Web.Infrastructure.Installation;
using Nut.Web.Models.License;
using Nut.Services.Localization;

namespace Nut.Web.Validators.License {
    public class LicenseValidator: AbstractValidator<LicenseModel>
    {
        public LicenseValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.LicenseCode).NotEmpty().WithMessage(localizationService.GetResource("LicenseCodeRequired"));
        }
    }
}