using Nut.Web.Framework;
using Nut.Web.Framework.Mvc;
using System.Web.Mvc;


namespace Nut.Admin.Models.Users {
    public partial class DepartmentListModel : BaseNutModel
    {
        [NutResourceDisplayName("Admin.Department.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

    }
}