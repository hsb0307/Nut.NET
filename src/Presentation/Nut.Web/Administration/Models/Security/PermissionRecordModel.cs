using Nut.Web.Framework.Mvc;

namespace Nut.Admin.Models.Security
{
    public partial class PermissionRecordModel : BaseNutModel
    {
        public string Name { get; set; }
        public string SystemName { get; set; }
    }
}