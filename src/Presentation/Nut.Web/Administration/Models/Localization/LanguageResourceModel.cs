using System.Web.Mvc;
using FluentValidation.Attributes;
using Nut.Admin.Validators.Localization;
using Nut.Web.Framework;
using Nut.Web.Framework.Mvc;

namespace Nut.Admin.Models.Localization
{
    [Validator(typeof(LanguageResourceValidator))]
    public partial class LanguageResourceModel : BaseNutEntityModel
    {
        [NutResourceDisplayName("Admin.Configuration.Languages.Resources.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [NutResourceDisplayName("Admin.Configuration.Languages.Resources.Fields.Value")]
        [AllowHtml]
        public string Value { get; set; }

        [NutResourceDisplayName("Admin.Configuration.Languages.Resources.Fields.LanguageName")]
        [AllowHtml]
        public string LanguageName { get; set; }

        public int LanguageId { get; set; }
    }
}