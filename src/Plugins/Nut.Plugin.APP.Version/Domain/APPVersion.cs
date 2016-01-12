﻿using Nut.Core;
using System;

namespace Nut.Plugin.APP.Version.Domain {
    /// <summary>
    /// APP Version
    /// </summary>
    public class APPVersion : BaseEntity {
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
        /// Gets or sets the download URL.
        /// </summary>
        public string DownloadURL { get; set; }

        /// <summary>
        /// Gets or sets the create on.
        /// </summary>
        public DateTime CreateON { get; set; }
    }
}
