using Nut.Core.Domain.Localization;
using Nut.Core.Domain.Users;

namespace Nut.Core {
    /// <summary>
    /// Work context
    /// </summary>
    public interface IWorkContext {
        /// <summary>
        /// Gets or sets the current User
        /// </summary>
        User CurrentUser { get; set; }

        /// <summary>
        /// Get or set current user working language
        /// </summary>
        Language WorkingLanguage { get; set; }

        /// <summary>
        /// Get or set value indicating whether we're in admin area
        /// </summary>
        bool IsAdmin { get; set; }
    }
}
