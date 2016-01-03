using FluentValidation;
using Nut.Admin.Models.Users;
using Nut.Services.Localization;
using Nut.Web.Framework.Validators;

namespace Nut.Admin.Validators.Users {
    public class DepartmentValidator : BaseNutValidator<DepartmentModel>{
        public DepartmentValidator(ILocalizationService localizationService) {
           
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Departments.Fields.Name.Required"));
            RuleFor(x => x.Name).Length(0, 50).WithMessage(localizationService.GetResource("Admin.Common.LengthValidation"), 0, 50);
            RuleFor(x => x.Code).NotEmpty().WithMessage(localizationService.GetResource("Admin.Departments.Fields.Code.Required"));
            RuleFor(x => x.Code).Length(0, 50).WithMessage(localizationService.GetResource("Admin.Common.LengthValidation"), 0, 50);

            RuleFor(x => x.StoreId).GreaterThan(0).WithMessage(localizationService.GetResource("Admin.Departments.Fields.StoreId.Required"), 0, 50);
        }
    }
}