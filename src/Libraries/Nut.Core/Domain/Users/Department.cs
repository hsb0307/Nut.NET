using System.Collections.Generic;
using Nut.Core.Domain.Stores;

namespace Nut.Core.Domain.Users {
    public class Department : BaseEntity {
        private ICollection<User> _users;

        #region Properties

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { set; get; }

        /// <summary>
        /// Gets or sets the code.
        /// </summary>
       public string Code { set; get; }

        /// <summary>
        /// Gets or sets the parent identifier.
        /// </summary>
        public int ParentId { set; get; }

        /// <summary>
        /// Gets or sets the display order.
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the store identifier.
        /// </summary>
        public int StoreId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Department"/> is deleted.
        /// </summary>
        public bool Deleted { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }
        #endregion

        #region Navigation properties

        /// <summary>
        /// Gets or sets the users.
        /// </summary>
        public virtual ICollection<User> Users {
            get { return _users ?? (_users = new List<User>()); }
            protected set { _users = value; }
        }

        public virtual Store Store { get; set; }
        #endregion


    }
}
