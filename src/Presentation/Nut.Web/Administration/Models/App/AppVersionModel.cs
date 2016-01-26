using System.Collections.Generic;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nut.Admin.Validators.App;
using Nut.Web.Framework;
using Nut.Web.Framework.Localization;
using Nut.Web.Framework.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Nut.Admin.Models.App {

    [Validator(typeof(AppVersionValidator))]
    public class AppVersionModel :BaseNutEntityModel{

        [NutResourceDisplayName("Admin.AppVersions.Fields.VersionNum")]
        public System.Int32 VersionNum { get; set; }
        [NutResourceDisplayName("Admin.AppVersions.Fields.Version")]
        public System.String Version { get; set; }
        [NutResourceDisplayName("Admin.AppVersions.Fields.APPName")]
        public System.String APPName { get; set; }
        [NutResourceDisplayName("Admin.AppVersions.Fields.Description")]
        public System.String Description { get; set; }
        [NutResourceDisplayName("Admin.AppVersions.Fields.DownloadURL")]
        [UIHint("Pictures")]
        public System.Int32 DownloadId { get; set; }
        [NutResourceDisplayName("Admin.AppVersions.Fields.Deleted")]
        public System.Boolean Deleted { get; set; }
        [NutResourceDisplayName("Admin.AppVersions.Fields.CreateON")]
        [UIHint("Date")]
        public System.DateTime CreateON { get; set; }
    }
}