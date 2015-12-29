using System.Collections.Generic;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nut.Web.Framework;
using Nut.Web.Framework.Localization;
using Nut.Web.Framework.Mvc;

namespace Nut.Admin.Models.Stores
{
    public partial class StoreModel : BaseNutEntityModel, ILocalizedModel<StoreLocalizedModel>
    {
        public StoreModel()
        {
            Locales = new List<StoreLocalizedModel>();
        }

        [NutResourceDisplayName("Admin.Configuration.Stores.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [NutResourceDisplayName("Admin.Configuration.Stores.Fields.Url")]
        [AllowHtml]
        public string Url { get; set; }

        [NutResourceDisplayName("Admin.Configuration.Stores.Fields.SslEnabled")]
        public virtual bool SslEnabled { get; set; }

        [NutResourceDisplayName("Admin.Configuration.Stores.Fields.SecureUrl")]
        [AllowHtml]
        public virtual string SecureUrl { get; set; }

        [NutResourceDisplayName("Admin.Configuration.Stores.Fields.Hosts")]
        [AllowHtml]
        public string Hosts { get; set; }

        [NutResourceDisplayName("Admin.Configuration.Stores.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }


        public IList<StoreLocalizedModel> Locales { get; set; }
    }

    public partial class StoreLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [NutResourceDisplayName("Admin.Configuration.Stores.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }
    }
}