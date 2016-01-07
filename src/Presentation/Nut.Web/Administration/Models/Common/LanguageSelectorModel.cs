using System.Collections.Generic;
using Nut.Admin.Models.Localization;
using Nut.Web.Framework.Mvc;

namespace Nut.Admin.Models.Common
{
    public partial class LanguageSelectorModel : BaseNutModel {
        public LanguageSelectorModel()
        {
            AvailableLanguages = new List<LanguageModel>();
        }

        public IList<LanguageModel> AvailableLanguages { get; set; }

        public LanguageModel CurrentLanguage { get; set; }
    }
}