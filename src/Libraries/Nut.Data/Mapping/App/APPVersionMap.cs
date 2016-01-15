using Nut.Core.Domain.App;

namespace Nut.Data.Mapping.App {
    public class APPVersionMap : NutEntityTypeConfiguration<AppVersion> {
        public APPVersionMap() {
            this.ToTable("AppVersion");
            this.HasKey(x => x.Id);

            this.Property(x => x.APPName).HasMaxLength(200);
            this.Property(x => x.Description).HasMaxLength(500);
            this.Property(x => x.Version).HasMaxLength(20);
        }
    }
}
