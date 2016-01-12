using Nut.Data.Mapping;
using Nut.Plugin.APP.Version.Domain;

namespace Nut.Plugin.APP.Version.Data {

    public class APPVersionMap : NutEntityTypeConfiguration<APPVersion> {
        public APPVersionMap() {
            this.ToTable("AppVersion");
            this.HasKey(x => x.Id);

            this.Property(x => x.APPName).HasMaxLength(200);
            this.Property(x => x.Description).HasMaxLength(500);
            this.Property(x => x.DownloadURL).HasMaxLength(200);
            this.Property(x => x.Version).HasMaxLength(20);
        }
    }
}
