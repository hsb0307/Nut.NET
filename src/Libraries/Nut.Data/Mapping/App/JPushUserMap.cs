using Nut.Core.Domain.App;

namespace Nut.Data.Mapping.App {
    public class JPushUserMap : NutEntityTypeConfiguration<JPushUser> {
        public JPushUserMap() {
            this.ToTable("JPushUser");
            this.HasKey(c => c.Id);

            this.Property(u => u.RegisterId).HasMaxLength(1000);
        }
    }
}
