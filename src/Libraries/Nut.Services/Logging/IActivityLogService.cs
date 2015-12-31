using Nut.Core;
using Nut.Core.Domain.Logging;
using Nut.Core.Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nut.Services.Logging {
    /// <summary>
    /// activity log service interface
    /// </summary>
    public partial interface IActivityLogService {
        /// <summary>
        /// Inserts an activity log type item
        /// </summary>
        /// <param name="activityLogType">Activity log type item</param>
        void InsertActivityType(ActivityLogType activityLogType);

        /// <summary>
        /// Updates an activity log type item
        /// </summary>
        /// <param name="activityLogType">Activity log type item</param>
        void UpdateActivityType(ActivityLogType activityLogType);

        /// <summary>
        /// Deletes an activity log type item
        /// </summary>
        /// <param name="activityLogType">Activity log type</param>
        void DeleteActivityType(ActivityLogType activityLogType);

        /// <summary>
        /// Gets all activity log type items
        /// </summary>
        /// <returns>Activity log type items</returns>
        IList<ActivityLogType> GetAllActivityTypes();

        /// <summary>
        /// Gets an activity log type item
        /// </summary>
        /// <param name="activityLogTypeId">Activity log type identifier</param>
        /// <returns>Activity log type item</returns>
        ActivityLogType GetActivityTypeById(int activityLogTypeId);

        /// <summary>
        /// Inserts an activity log item
        /// </summary>
        /// <param name="systemKeyword">The system keyword</param>
        /// <param name="comment">The activity comment</param>
        /// <param name="commentParams">The activity comment parameters for string.Format() function.</param>
        /// <returns>Activity log item</returns>
        ActivityLog InsertActivity(string systemKeyword, string comment, params object[] commentParams);

        /// <summary>
        /// Inserts the activity.
        /// </summary>
        /// <param name="systemKeyword">The system keyword.</param>
        /// <param name="comment">The comment.</param>
        /// <param name="user">The user.</param>
        /// <param name="commentParams">The comment parameters.</param>
        /// <returns></returns>
        ActivityLog InsertActivity(string systemKeyword,
            string comment, User user, params object[] commentParams);

        /// <summary>
        /// Deletes an activity log item
        /// </summary>
        /// <param name="activityLog">Activity log</param>
        void DeleteActivity(ActivityLog activityLog);

        /// <summary>
        /// Gets all activities.
        /// </summary>
        /// <param name="createdOnFrom">The created on from.</param>
        /// <param name="createdOnTo">The created on to.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="activityLogTypeId">The activity log type identifier.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns></returns>
        IPagedList<ActivityLog> GetAllActivities(DateTime? createdOnFrom = null,
            DateTime? createdOnTo = null, int? userId = null, int activityLogTypeId = 0,
            int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Gets an activity log item
        /// </summary>
        /// <param name="activityLogId">Activity log identifier</param>
        /// <returns>Activity log item</returns>
        ActivityLog GetActivityById(int activityLogId);

        /// <summary>
        /// Clears activity log
        /// </summary>
        void ClearAllActivities();
    }
}
