using System;
using Nut.Core.Domain.Users;

namespace Nut.Core.Domain.App {

    public class JPushUser : BaseEntity {

        /// <summary>
        /// Gets or sets the register identifier.
        /// </summary>
        public string RegisterId { get; set; }

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the create time.
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        public virtual User User { get; set; }
    }
}
