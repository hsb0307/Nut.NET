using System;
using Nut.Web.Framework;
using Nut.Web.Framework.Mvc;

namespace Nut.Admin.Models.Logging
{
    public partial class ActivityLogModel : BaseNutEntityModel
    {
        [NutResourceDisplayName("Admin.Configuration.ActivityLog.ActivityLog.Fields.ActivityLogType")]
        public string ActivityLogTypeName { get; set; }
        [NutResourceDisplayName("Admin.Configuration.ActivityLog.ActivityLog.Fields.User")]
        public int UserId { get; set; }
        [NutResourceDisplayName("Admin.Configuration.ActivityLog.ActivityLog.Fields.User")]
        public string UserName { get; set; }
        [NutResourceDisplayName("Admin.Configuration.ActivityLog.ActivityLog.Fields.Comment")]
        public string Comment { get; set; }
        [NutResourceDisplayName("Admin.Configuration.ActivityLog.ActivityLog.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }
    }
}
