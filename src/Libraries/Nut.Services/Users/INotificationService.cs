using Nut.Core;
using Nut.Core.Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nut.Services.Users {
    /// <summary>
    /// Notification service interface
    /// </summary>
    public interface INotificationService {
        /// <summary>
        /// Gets all Notifications
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Notification collection</returns>
        IList<Notification> GetAll(bool showHidden = false);
        /// <summary>
        /// Gets the paged.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="isNotice">The is notice.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns></returns>
        IPagedList<Notification> GetPaged(int userId = 0, bool? isNotice = null, int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Gets a notification 
        /// </summary>
        /// <param name="countryId">Notification identifier</param>
        /// <returns>Notification</returns>
        Notification GetById(int id);

        /// <summary>
        /// Inserts a notification
        /// </summary>
        /// <param name="notification">Notification</param>
        void Insert(Notification notification);

        /// <summary>
        /// Updates the notification
        /// </summary>
        /// <param name="notification">Notification</param>
        void Update(Notification notification);
        /// <summary>
        /// Deletes a notification
        /// </summary>
        /// <param name="notification">Notification</param>
        void Delete(Notification notification);

        /// <summary>
        /// IsNotice a notification
        /// </summary>
        /// <param name="notification">Notification</param>
        void Notice(Notification notification);

        /// <summary>
        /// Deletes a notification
        /// </summary>
        /// <param name="olderThanUtc">Older than date and time</param>
        /// <returns>Number of deleted items</returns>
        int Delete(DateTime olderThanUtc);
    }
}
