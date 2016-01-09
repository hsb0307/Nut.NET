using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nut.Core.Domain.Users {
    public class Notification : BaseEntity {

        /// <summary>
        /// Gets or sets the Comment
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is notice.
        public bool IsNotice { get; set; }

        /// <summary>
        /// Gets or sets the create time.
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        public virtual User User { get; set; }
    }
}
