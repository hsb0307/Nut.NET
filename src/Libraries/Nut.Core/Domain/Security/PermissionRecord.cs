using Nut.Core.Domain.Users;
using System.Collections.Generic;

namespace Nut.Core.Domain.Security {
    public class PermissionRecord : BaseEntity {

        private ICollection<UserRole> _userRoles;

        /// <summary>
        /// Gets or sets the permission name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the permission system name
        /// </summary>
        public string SystemName { get; set; }

        /// <summary>
        /// Gets or sets the permission category
        /// </summary>
        public string Category { get; set; }


        /// <summary>
        /// 显示顺序
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets User Role
        /// </summary>
        public virtual ICollection<UserRole> UserRoles {
            get { return _userRoles ?? (_userRoles = new List<UserRole>()); }
            protected set { _userRoles = value; }
        }
    }
}
