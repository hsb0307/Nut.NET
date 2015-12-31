using Nut.Core.Domain.Logging;

namespace Nut.Data.Mapping.Logging {
    public class ActivityLogMap : NutEntityTypeConfiguration<ActivityLog> {
        public ActivityLogMap() {
            this.ToTable("ActivityLog");
            this.HasKey(al => al.Id);
            this.Property(al => al.Comment).IsRequired();

            this.HasRequired(al => al.ActivityLogType)
                .WithMany()
                .HasForeignKey(al => al.ActivityLogTypeId);

            this.HasRequired(al => al.User)
                .WithMany()
                .HasForeignKey(al => al.UserId);
        }
    }
}
