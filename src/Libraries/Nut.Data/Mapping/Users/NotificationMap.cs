using Nut.Core.Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nut.Data.Mapping.Users {
    public class NotificationMap : NutEntityTypeConfiguration<Notification> {

        public NotificationMap() {
            this.ToTable("Notification");
            this.HasKey(c => c.Id);

            this.Property(u => u.Comment).HasMaxLength(1000);
            this.Property(u => u.Url).HasMaxLength(200);
        }
    }
}
