using Nut.Core.Domain.Tasks;

namespace Nut.Data.Mapping.Tasks
{
    public partial class ScheduleTaskMap : NutEntityTypeConfiguration<ScheduleTask>
    {
        public ScheduleTaskMap()
        {
            this.ToTable("ScheduleTask");
            this.HasKey(t => t.Id);
            this.Property(t => t.Name).IsRequired();
            this.Property(t => t.Type).IsRequired();
        }
    }
}