using System;

namespace Nut.Core.Domain.Media {

    /// <summary>
    /// Pepresents a download
    /// </summary>
    public partial class Download :BaseEntity {
        /// <summary>
        /// Gets or sets the download unique identifier.
        /// </summary>
        public Guid DownloadGuid { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether [use download URL].
        /// </summary>
        public bool UseDownloadUrl { get; set; }

        /// <summary>
        /// Gets or sets the download URL.
        /// </summary>
        public string DownloadUrl { get; set; }

        /// <summary>
        /// Gets or sets the download binary.
        /// </summary>
        public byte[] DownloadBinary { get; set; }

        /// <summary>
        /// Gets or sets the type of the content.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the extension.
        /// </summary>
        public string Extension { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is new.
        /// </summary>
        public bool IsNew { get; set; }
    }
}
