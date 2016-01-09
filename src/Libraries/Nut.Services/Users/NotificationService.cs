using Nut.Core;
using Nut.Core.Caching;
using Nut.Core.Data;
using Nut.Core.Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nut.Services.Users {
    public class NotificationService : INotificationService {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>
        private const string NOTIFICATION_ALL_KEY = "Hgs.notification.all-{0}";

        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string NOTIFICATION_PATTERN_KEY = "Hgs.notification.";

        #endregion

        #region Fields

        private readonly IRepository<Notification> _notificationRepository;
        private readonly ICacheManager _cacheManager;
        private readonly ISignals _signals;
        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="notificationRepository">Notification repository</param>
        /// <param name="eventPublisher">Event published</param>
        public NotificationService(ICacheManager cacheManager,
            IRepository<Notification> notificationRepository,
            ISignals signals) {
            _cacheManager = cacheManager;
            _notificationRepository = notificationRepository;
            _signals = signals;
        }

        #endregion

        #region Methods
        /// <summary>
        /// Gets all countries
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Notification collection</returns>
        public virtual IList<Notification> GetAll(bool showHidden = false) {
            string key = string.Format(NOTIFICATION_ALL_KEY, showHidden);
            return _cacheManager.Get(key, ctx => {
                ctx.Monitor(_signals.When(NOTIFICATION_PATTERN_KEY));
                var query = from c in _notificationRepository.Table
                                //orderby c.DisplayOrder, c.Name
                                //where showHidden || c.Published
                            select c;
                if (!showHidden)
                    query = query.Where(c => !c.IsNotice).OrderBy(c => c.CreatedOn);
                var notifications = query.ToList();
                return notifications;
            });
        }


        public virtual IPagedList<Notification> GetPaged(int userId = 0, bool? isNotice = null, int pageIndex = 0, int pageSize = int.MaxValue) {
            var query = _notificationRepository.Table;
            if (userId > 0)
                query = query.Where(v => v.UserId == userId);
            if (isNotice.HasValue)
                query = query.Where(v => v.IsNotice == isNotice.Value);
            query = query.OrderByDescending(v => v.CreatedOn);

            var notifications = new PagedList<Notification>(query, pageIndex, pageSize);
            return notifications;
        }

        /// <summary>
        /// Gets a notification 
        /// </summary>
        /// <param name="notificationId">Notification identifier</param>
        /// <returns>Notification</returns>
        public virtual Notification GetById(int notificationId) {
            if (notificationId == 0)
                return null;

            return _notificationRepository.GetById(notificationId);
        }

        /// <summary>
        /// Inserts a notification
        /// </summary>
        /// <param name="notification">Notification</param>
        public virtual void Insert(Notification notification) {
            if (notification == null)
                throw new ArgumentNullException("notification");

            _notificationRepository.Insert(notification);

            _signals.Trigger(NOTIFICATION_PATTERN_KEY);

        }

        /// <summary>
        /// Updates the notification
        /// </summary>
        /// <param name="notification">Notification</param>
        public virtual void Update(Notification notification) {
            if (notification == null)
                throw new ArgumentNullException("notification");

            _notificationRepository.Update(notification);

            _signals.Trigger(NOTIFICATION_PATTERN_KEY);

        }

        /// <summary>
        /// Deletes a notification
        /// </summary>
        /// <param name="notification">Notification</param>
        public virtual void Delete(Notification notification) {
            if (notification == null)
                throw new ArgumentNullException("notification");

            _notificationRepository.Delete(notification);

            _signals.Trigger(NOTIFICATION_PATTERN_KEY);

        }

        /// <summary>
        /// IsNotice a notification
        /// </summary>
        /// <param name="notification">Notification</param>
        public void Notice(Notification notification) {
            if (notification == null)
                throw new ArgumentNullException("notification");

            notification.IsNotice = true;
            Update(notification);
        }

        /// <summary>
        /// Deletes a notification
        /// </summary>
        /// <param name="olderThanUtc">Older than date and time</param>
        /// <returns>Number of deleted items</returns>
        public int Delete(DateTime olderThanUtc) {
            var query = from sci in _notificationRepository.Table
                        where sci.CreatedOn < olderThanUtc
                        select sci;

            var notifications = query.ToList();
            foreach (var notification in notifications)
                _notificationRepository.Delete(notification);
            return notifications.Count;
        }

        #endregion
    }
}
