using Nut.Core;
using Nut.Core.Caching;
using Nut.Core.Data;
using Nut.Core.Domain.Logging;
using Nut.Core.Domain.Users;
using Nut.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nut.Services.Logging {
    public class ActivityLogService : IActivityLogService {

        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        private const string ACTIVITYTYPE_ALL_KEY = "Nop.activitytype.all";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string ACTIVITYTYPE_PATTERN_KEY = "Nop.activitytype.";

        #endregion

        #region Fields

        /// <summary>
        /// Cache manager
        /// </summary>
        private readonly ICacheManager _cacheManager;
        private readonly ISignals _signals;
        private readonly IRepository<ActivityLog> _activityLogRepository;
        private readonly IRepository<ActivityLogType> _activityLogTypeRepository;
        private readonly IWorkContext _workContext;
        private readonly IDbContext _dbContext;
        private readonly IDataProvider _dataProvider;
        #endregion

        #region Ctor
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="activityLogRepository">Activity log repository</param>
        /// <param name="activityLogTypeRepository">Activity log type repository</param>
        /// <param name="workContext">Work context</param>
        /// <param name="dbContext">DB context</param>>
        /// <param name="dataProvider">WeData provider</param>
        public ActivityLogService(ICacheManager cacheManager,
            ISignals signals,
            IRepository<ActivityLog> activityLogRepository,
            IRepository<ActivityLogType> activityLogTypeRepository,
            IWorkContext workContext,
            IDbContext dbContext, IDataProvider dataProvider) {
            this._cacheManager = cacheManager;
            this._signals = signals;
            this._activityLogRepository = activityLogRepository;
            this._activityLogTypeRepository = activityLogTypeRepository;
            this._workContext = workContext;
            this._dbContext = dbContext;
            this._dataProvider = dataProvider;
        }

        #endregion

        #region Nested classes

        [Serializable]
        public class ActivityLogTypeForCaching {
            public int Id { get; set; }
            public string SystemKeyword { get; set; }
            public string Name { get; set; }
            public bool Enabled { get; set; }
        }

        #endregion

        #region Utitlies

        /// <summary>
        /// Gets all activity log types (class for caching)
        /// </summary>
        /// <returns>Activity log types</returns>
        protected virtual IList<ActivityLogTypeForCaching> GetAllActivityTypesCached() {
            //cache
            string key = string.Format(ACTIVITYTYPE_ALL_KEY);
            return _cacheManager.Get(key, ctx => {
                ctx.Monitor(_signals.When(ACTIVITYTYPE_PATTERN_KEY));

                var result = new List<ActivityLogTypeForCaching>();
                var activityLogTypes = GetAllActivityTypes();
                foreach (var alt in activityLogTypes) {
                    var altForCaching = new ActivityLogTypeForCaching {
                        Id = alt.Id,
                        SystemKeyword = alt.SystemKeyword,
                        Name = alt.Name,
                        Enabled = alt.Enabled
                    };
                    result.Add(altForCaching);
                }
                return result;
            });
        }

        #endregion

        #region Methods

        /// <summary>
        /// Inserts an activity log type item
        /// </summary>
        /// <param name="activityLogType">Activity log type item</param>
        public virtual void InsertActivityType(ActivityLogType activityLogType) {
            if (activityLogType == null)
                throw new ArgumentNullException("activityLogType");

            _activityLogTypeRepository.Insert(activityLogType);

            _signals.When(ACTIVITYTYPE_PATTERN_KEY);
        }

        /// <summary>
        /// Updates an activity log type item
        /// </summary>
        /// <param name="activityLogType">Activity log type item</param>
        public virtual void UpdateActivityType(ActivityLogType activityLogType) {
            if (activityLogType == null)
                throw new ArgumentNullException("activityLogType");

            _activityLogTypeRepository.Update(activityLogType);
            _signals.When(ACTIVITYTYPE_PATTERN_KEY);
        }

        /// <summary>
        /// Deletes an activity log type item
        /// </summary>
        /// <param name="activityLogType">Activity log type</param>
        public virtual void DeleteActivityType(ActivityLogType activityLogType) {
            if (activityLogType == null)
                throw new ArgumentNullException("activityLogType");

            _activityLogTypeRepository.Delete(activityLogType);
            _signals.When(ACTIVITYTYPE_PATTERN_KEY);
        }

        /// <summary>
        /// Gets all activity log type items
        /// </summary>
        /// <returns>Activity log type items</returns>
        public virtual IList<ActivityLogType> GetAllActivityTypes() {
            var query = from alt in _activityLogTypeRepository.Table
                        orderby alt.Name
                        select alt;
            var activityLogTypes = query.ToList();
            return activityLogTypes;
        }

        /// <summary>
        /// Gets an activity log type item
        /// </summary>
        /// <param name="activityLogTypeId">Activity log type identifier</param>
        /// <returns>Activity log type item</returns>
        public virtual ActivityLogType GetActivityTypeById(int activityLogTypeId) {
            if (activityLogTypeId == 0)
                return null;

            return _activityLogTypeRepository.GetById(activityLogTypeId);
        }

        /// <summary>
        /// Inserts an activity log item
        /// </summary>
        /// <param name="systemKeyword">The system keyword</param>
        /// <param name="comment">The activity comment</param>
        /// <param name="commentParams">The activity comment parameters for string.Format() function.</param>
        /// <returns>Activity log item</returns>
        public virtual ActivityLog InsertActivity(string systemKeyword,
            string comment, params object[] commentParams) {
            return InsertActivity(systemKeyword, comment, _workContext.CurrentUser, commentParams);
        }


        /// <summary>
        /// Inserts the activity.
        /// </summary>
        /// <param name="systemKeyword">The system keyword.</param>
        /// <param name="comment">The comment.</param>
        /// <param name="user">The user.</param>
        /// <param name="commentParams">The comment parameters.</param>
        /// <returns></returns>
        public virtual ActivityLog InsertActivity(string systemKeyword,
            string comment, User user, params object[] commentParams) {
            if (user == null)
                return null;

            var activityTypes = GetAllActivityTypesCached();
            var activityType = activityTypes.ToList().Find(at => at.SystemKeyword == systemKeyword);
            if (activityType == null || !activityType.Enabled)
                return null;

            comment = CommonHelper.EnsureNotNull(comment);
            comment = string.Format(comment, commentParams);
            comment = CommonHelper.EnsureMaximumLength(comment, 4000);



            var activity = new ActivityLog();
            activity.ActivityLogTypeId = activityType.Id;
            activity.User = user;
            activity.Comment = comment;
            activity.CreatedOnUtc = DateTime.UtcNow;

            _activityLogRepository.Insert(activity);

            return activity;
        }

        /// <summary>
        /// Deletes an activity log item
        /// </summary>
        /// <param name="activityLog">Activity log type</param>
        public virtual void DeleteActivity(ActivityLog activityLog) {
            if (activityLog == null)
                throw new ArgumentNullException("activityLog");

            _activityLogRepository.Delete(activityLog);
        }

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
        public virtual IPagedList<ActivityLog> GetAllActivities(DateTime? createdOnFrom = null,
            DateTime? createdOnTo = null, int? userId = null, int activityLogTypeId = 0,
            int pageIndex = 0, int pageSize = int.MaxValue) {
            var query = _activityLogRepository.Table;
            if (createdOnFrom.HasValue)
                query = query.Where(al => createdOnFrom.Value <= al.CreatedOnUtc);
            if (createdOnTo.HasValue)
                query = query.Where(al => createdOnTo.Value >= al.CreatedOnUtc);
            if (activityLogTypeId > 0)
                query = query.Where(al => activityLogTypeId == al.ActivityLogTypeId);
            if (userId.HasValue)
                query = query.Where(al => userId.Value == al.UserId);

            query = query.OrderByDescending(al => al.CreatedOnUtc);

            var activityLog = new PagedList<ActivityLog>(query, pageIndex, pageSize);
            return activityLog;
        }

        /// <summary>
        /// Gets an activity log item
        /// </summary>
        /// <param name="activityLogId">Activity log identifier</param>
        /// <returns>Activity log item</returns>
        public virtual ActivityLog GetActivityById(int activityLogId) {
            if (activityLogId == 0)
                return null;

            return _activityLogRepository.GetById(activityLogId);
        }

        /// <summary>
        /// Clears activity log
        /// </summary>
        public virtual void ClearAllActivities() {
            var activityLog = _activityLogRepository.Table.ToList();
            foreach (var activityLogItem in activityLog)
                _activityLogRepository.Delete(activityLogItem);
        }
        #endregion
    }
}
