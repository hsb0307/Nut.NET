﻿using FluentValidation;
using Nut.Web.Framework.Validators;
using Nut.Web.Infrastructure.Installation;
using Nut.Web.Models.Install;

namespace Nut.Web.Validators.Install
{
    public class InstallValidator : BaseNutValidator<InstallModel>
    {
        public InstallValidator(IInstallationLocalizationService locService)
        {
            RuleFor(x => x.AdminUsername).NotEmpty().WithMessage(locService.GetResource("AdminEmailRequired"));
            RuleFor(x => x.AdminPassword).NotEmpty().WithMessage(locService.GetResource("AdminPasswordRequired"));
            RuleFor(x => x.ConfirmPassword).NotEmpty().WithMessage(locService.GetResource("ConfirmPasswordRequired"));
            RuleFor(x => x.AdminPassword).Equal(x => x.ConfirmPassword).WithMessage(locService.GetResource("PasswordsDoNotMatch"));
            RuleFor(x => x.DataProvider).NotEmpty().WithMessage(locService.GetResource("DataProviderRequired"));
        }
    }
}