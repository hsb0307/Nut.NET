using Nut.Core.Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nut.Data.Mapping.Users {
    public class DepartmentMap : NutEntityTypeConfiguration<Department> {
        public DepartmentMap() {
            this.ToTable("Department");
            this.HasKey(d => d.Id);


            this.Property(u => u.Code).IsRequired().HasMaxLength(200);
            this.Property(u => u.Name).IsRequired().HasMaxLength(200);

            this.HasMany(d => d.Users)
                .WithRequired(c => c.Department)
                .HasForeignKey(c => c.DepartmentId).WillCascadeOnDelete(false);
        }
    }
}
