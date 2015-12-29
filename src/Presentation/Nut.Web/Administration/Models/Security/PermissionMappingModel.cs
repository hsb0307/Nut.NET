using System.Collections.Generic;
using Nut.Admin.Models.Users;
using Nut.Web.Framework.Mvc;

namespace Nut.Admin.Models.Security
{
    public partial class PermissionMappingModel : BaseNutModel
    {
        public PermissionMappingModel()
        {
            AvailablePermissions = new List<PermissionRecordModel>();
            AvailableCustomerRoles = new List<UserRoleModel>();
            Allowed = new Dictionary<string, IDictionary<int, bool>>();
        }
        public IList<PermissionRecordModel> AvailablePermissions { get; set; }
        public IList<UserRoleModel> AvailableCustomerRoles { get; set; }

        //[permission system name] / [customer role id] / [allowed]
        public IDictionary<string, IDictionary<int, bool>> Allowed { get; set; }
    }
}