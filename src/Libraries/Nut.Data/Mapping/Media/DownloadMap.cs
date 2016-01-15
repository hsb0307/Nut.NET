using Nut.Core.Domain.Media;

namespace Nut.Data.Mapping.Media {
    public class DownloadMap : NutEntityTypeConfiguration<Download> {

        public DownloadMap() {
            this.ToTable("Download");
            this.HasKey(p => p.Id);

            this.Property(p => p.DownloadBinary).IsMaxLength();
            this.Property(p => p.DownloadUrl).HasMaxLength(500);
            this.Property(p => p.Extension).HasMaxLength(100);
            this.Property(p => p.FileName).HasMaxLength(200);
            
        }
    }
}
