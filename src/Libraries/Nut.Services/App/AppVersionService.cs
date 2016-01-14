using Nut.Core;
using Nut.Core.Caching;
using Nut.Core.Data;
using Nut.Core.Domain.App;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nut.Services.App {
    public class AppVersionService: IAppVersionService {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>
        private const string APPVERSION_ALL_KEY = "Nuts.appVersion.all-{0}";

        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string APPVERSION_PATTERN_KEY = "Nuts.appversion.";

        #endregion

        #region Fields

        private readonly IRepository<AppVersion> _appVersionRepository;
        private readonly ICacheManager _cacheManager;
        private readonly ISignals _signals;
        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="appVersionRepository">AppVersion repository</param>
        /// <param name="eventPublisher">Event published</param>
        public AppVersionService(ICacheManager cacheManager,
            IRepository<AppVersion> appVersionRepository,
            ISignals signals) {
            _cacheManager = cacheManager;
            _appVersionRepository = appVersionRepository;
            _signals = signals;
        }

        #endregion

        #region Methods



        /// <summary>
        /// Gets all countries
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>AppVersion collection</returns>
        public virtual IList<AppVersion> GetAll(bool showHidden = false) {
            string key = string.Format(APPVERSION_ALL_KEY, showHidden);
            return _cacheManager.Get(key, ctx => {
                ctx.Monitor(_signals.When(APPVERSION_PATTERN_KEY));
                var query = from c in _appVersionRepository.Table
                                //orderby c.DisplayOrder, c.Name
                                //where showHidden || c.Published
                            select c;
                if (!showHidden)
                    query = query.Where(c => !c.Deleted).OrderBy(c => c.VersionNum);
                var appVersions = query.ToList();
                return appVersions;
            });
        }


        public virtual IPagedList<AppVersion> GetPaged(int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false) {
            var query = _appVersionRepository.Table;
            if (!showHidden)
                query = query.Where(v => !v.Deleted);
            query = query.OrderBy(v => v.VersionNum).ThenBy(v => v.Version);

            var appVersions = new PagedList<AppVersion>(query, pageIndex, pageSize);
            return appVersions;
        }

        /// <summary>
        /// Gets a appVersion 
        /// </summary>
        /// <param name="appVersionId">AppVersion identifier</param>
        /// <returns>AppVersion</returns>
        public virtual AppVersion GetById(int appVersionId) {
            if (appVersionId == 0)
                return null;

            return _appVersionRepository.GetById(appVersionId);
        }


        /// <summary>
        /// Inserts a appVersion
        /// </summary>
        /// <param name="appVersion">AppVersion</param>
        public virtual void Insert(AppVersion appVersion) {
            if (appVersion == null)
                throw new ArgumentNullException("appVersion");

            _appVersionRepository.Insert(appVersion);

            _signals.Trigger(APPVERSION_PATTERN_KEY);

        }

        /// <summary>
        /// Updates the appVersion
        /// </summary>
        /// <param name="appVersion">AppVersion</param>
        public virtual void Update(AppVersion appVersion) {
            if (appVersion == null)
                throw new ArgumentNullException("appVersion");

            _appVersionRepository.Update(appVersion);

            _signals.Trigger(APPVERSION_PATTERN_KEY);

        }

        /// <summary>
        /// Deletes a appVersion
        /// </summary>
        /// <param name="appVersion">AppVersion</param>
        public virtual void Delete(AppVersion appVersion) {
            if (appVersion == null)
                throw new ArgumentNullException("appVersion");

            _appVersionRepository.Delete(appVersion);

            _signals.Trigger(APPVERSION_PATTERN_KEY);

        }

        #endregion
    }
}
