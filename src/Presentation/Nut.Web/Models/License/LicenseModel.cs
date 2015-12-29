using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nut.Web.Framework.Mvc;
using Nut.Web.Validators.License;

namespace Nut.Web.Models.License {
    [Validator(typeof(LicenseValidator))]
    public class LicenseModel : BaseNutModel {

        public string MachineCode { get; set; }
        public string LicenseCode { get; set; }

    }
}