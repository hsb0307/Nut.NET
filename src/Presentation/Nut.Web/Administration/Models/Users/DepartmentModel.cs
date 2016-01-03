using System.Collections.Generic;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nut.Admin.Validators.Users;
using Nut.Web.Framework;
using Nut.Web.Framework.Mvc;

namespace Nut.Admin.Models.Users {
    [Validator(typeof(DepartmentValidator))]
    public partial class DepartmentModel : BaseNutEntityModel //, ILocalizedModel<DepartmentLocalizedModel>
    {
        [NutResourceDisplayName("Admin.Users.Departments.Fields.Name")]
        //[AllowHtml]
        public System.String Name { get; set; }

        public string Breadcrumb { get; set; }

        [NutResourceDisplayName("Admin.Users.Departments.Fields.Code")]
        //[AllowHtml]
        public System.String Code { get; set; }

        [NutResourceDisplayName("Admin.Users.Departments.Fields.ParentId")]
        //[AllowHtml]
        public System.Int32 ParentId { get; set; }

        [NutResourceDisplayName("Admin.Users.Departments.Fields.ParentId")]
        //[AllowHtml]
        public System.String ParentName { get; set; }

        [NutResourceDisplayName("Admin.Users.Departments.Fields.DisplayOrder")]
        //[AllowHtml]
        public System.Int32 DisplayOrder { get; set; }

        [NutResourceDisplayName("Admin.Users.Departments.Fields.StoreId")]
        //[AllowHtml]
        public System.Int32 StoreId { get; set; }

        [NutResourceDisplayName("Admin.Users.Departments.Fields.Deleted")]
        //[AllowHtml]
        public System.Boolean Deleted { get; set; }

        [NutResourceDisplayName("Admin.Users.Departments.Fields.Description")]
        //[AllowHtml]
        public System.String Description { get; set; }

        private IList<SelectListItem> _availableDepartments;
        public IList<SelectListItem> AvailableDepartments {
            get {
                if (_availableDepartments == null)
                    _availableDepartments = new List<SelectListItem>();
                return _availableDepartments;
            }
            set { _availableDepartments = value; }
        }

        private IList<SelectListItem> _availableStores;
        public IList<SelectListItem> AvailableStores {
            get {
                if (_availableStores == null)
                    _availableStores = new List<SelectListItem>();
                return _availableStores;
            }
            set { _availableStores = value; }
        }

    }
}