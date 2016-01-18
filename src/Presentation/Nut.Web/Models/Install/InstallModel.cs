using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nut.Web.Framework.Mvc;
using Nut.Web.Validators.Install;

namespace Nut.Web.Models.Install
{
    [Validator(typeof(InstallValidator))]
    public partial class InstallModel : BaseNutModel
    {
        public InstallModel()
        {
            this.AvailableLanguages = new List<SelectListItem>();
        }
        [AllowHtml]
        public string AdminUsername { get; set; }
        [AllowHtml]
        [DataType(DataType.Password)]
        public string AdminPassword { get; set; }
        [AllowHtml]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }


        
        public string DataProvider { get; set; }
        public bool DisableSqlCompact { get; set; }
        
        //sql server
        public bool SqlServerCreateDatabase { get; set; }

        [AllowHtml]
        public string SqlServerConnectionString { get; set; }

        //my sql
        public bool MySqlCreateDatabase { get; set; }

        [AllowHtml]
        public string MySqlConnectionString { get; set; }


        public bool UseCustomCollation { get; set; }
        [AllowHtml]
        public string Collation { get; set; }


        public bool DisableSampleDataOption { get; set; }
        public bool InstallSampleData { get; set; }

        public List<SelectListItem> AvailableLanguages { get; set; }
    }
}