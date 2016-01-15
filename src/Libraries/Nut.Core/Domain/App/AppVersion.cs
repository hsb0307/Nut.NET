using System;

namespace Nut.Core.Domain.App {
    /// <summary>
    /// App Version 
    /// </summary>
    public class AppVersion : BaseEntity {
        /// <summary>
        /// Gets or sets the version number.
        /// </summary>
        public int VersionNum { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the name of the application.
        /// </summary>
        public string APPName { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the download identifier.
        /// </summary>
        public int DownloadId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="AppVersion"/> is delete.
        /// </summary>
        public bool Deleted { get; set; }

        /// <summary>
        /// Gets or sets the create on.
        /// </summary>
        public DateTime CreateON { get; set; }
    }
}
