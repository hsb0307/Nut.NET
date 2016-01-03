using Nut.Core.Domain.Users;

namespace Nut.Data.Mapping.Customers
{
    public partial class UserMap : NutEntityTypeConfiguration<User>
    {
        public UserMap()
        {
            this.ToTable("User");
            this.HasKey(c => c.Id);
            this.Property(u => u.Username).HasMaxLength(1000);
            this.Property(u => u.Email).HasMaxLength(1000);

            this.Ignore(u => u.PasswordFormat);

            this.HasMany(c => c.UserRoles)
                .WithMany()
                .Map(m => m.ToTable("User_UserRole_Mapping"));
        }
    }
}