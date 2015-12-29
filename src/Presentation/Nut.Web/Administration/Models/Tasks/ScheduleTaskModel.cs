using System.Web.Mvc;
using Nut.Web.Framework;
using Nut.Web.Framework.Mvc;

namespace Nut.Admin.Models.Tasks
{
    public partial class ScheduleTaskModel : BaseNutEntityModel
    {
        [NutResourceDisplayName("Admin.System.ScheduleTasks.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [NutResourceDisplayName("Admin.System.ScheduleTasks.Seconds")]
        public int Seconds { get; set; }

        [NutResourceDisplayName("Admin.System.ScheduleTasks.Enabled")]
        public bool Enabled { get; set; }

        [NutResourceDisplayName("Admin.System.ScheduleTasks.StopOnError")]
        public bool StopOnError { get; set; }

        [NutResourceDisplayName("Admin.System.ScheduleTasks.LastStart")]
        public string LastStartUtc { get; set; }

        [NutResourceDisplayName("Admin.System.ScheduleTasks.LastEnd")]
        public string LastEndUtc { get; set; }

        [NutResourceDisplayName("Admin.System.ScheduleTasks.LastSuccess")]
        public string LastSuccessUtc { get; set; }
    }
}