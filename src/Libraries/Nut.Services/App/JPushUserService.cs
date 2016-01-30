using System;
using System.Collections.Generic;
using System.Linq;
using Nut.Core;
using Nut.Core.Caching;
using Nut.Core.Data;
using Nut.Core.Domain.App;

namespace Nut.Services.App {
    public partial class JPushUserService : IJPushUserService {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>
        private const string JPUSHUSER_ALL_KEY = "Nuts.jPushUser.all-{0}";

        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string JPUSHUSER_PATTERN_KEY = "Nuts.jpushuser.";

        #endregion

        #region Fields

        private readonly IRepository<JPushUser> _jPushUserRepository;
        private readonly ICacheManager _cacheManager;
        private readonly ISignals _signals;
        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="jPushUserRepository">JPushUser repository</param>
        /// <param name="eventPublisher">Event published</param>
        public JPushUserService(ICacheManager cacheManager,
            IRepository<JPushUser> jPushUserRepository,
            ISignals signals) {
            _cacheManager = cacheManager;
            _jPushUserRepository = jPushUserRepository;
            _signals = signals;
        }

        #endregion

        #region Methods



        /// <summary>
        /// Gets all countries
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>JPushUser collection</returns>
        public virtual IList<JPushUser> GetAll() {
            string key = string.Format(JPUSHUSER_ALL_KEY);
            return _cacheManager.Get(key, ctx => {
                ctx.Monitor(_signals.When(JPUSHUSER_PATTERN_KEY));
                var query = from c in _jPushUserRepository.Table
                                //orderby c.DisplayOrder, c.Name
                                //where showHidden || c.Published
                            select c;

                var jPushUsers = query.ToList();
                return jPushUsers;
            });
        }


        public IPagedList<JPushUser> GetPaged(string registerId = "", int pageIndex = 0, int pageSize = int.MaxValue) {
            var query = _jPushUserRepository.Table;

            if (!string.IsNullOrEmpty(registerId)) {
                query = query.Where(t => t.RegisterId == registerId);
            }

            query = query.OrderByDescending(v => v.CreatedOn).ThenBy(v => v.RegisterId);

            var jPushUsers = new PagedList<JPushUser>(query, pageIndex, pageSize);
            return jPushUsers;
        }

        /// <summary>
        /// Gets a jPushUser 
        /// </summary>
        /// <param name="jPushUserId">JPushUser identifier</param>
        /// <returns>JPushUser</returns>
        public virtual JPushUser GetById(int jPushUserId) {
            if (jPushUserId == 0)
                return null;

            return _jPushUserRepository.GetById(jPushUserId);
        }


        /// <summary>
        /// Inserts a jPushUser
        /// </summary>
        /// <param name="jPushUser">JPushUser</param>
        public virtual void Insert(JPushUser jPushUser) {
            if (jPushUser == null)
                throw new ArgumentNullException("jPushUser");

            _jPushUserRepository.Insert(jPushUser);

            _signals.Trigger(JPUSHUSER_PATTERN_KEY);

        }

        /// <summary>
        /// Updates the jPushUser
        /// </summary>
        /// <param name="jPushUser">JPushUser</param>
        public virtual void Update(JPushUser jPushUser) {
            if (jPushUser == null)
                throw new ArgumentNullException("jPushUser");

            _jPushUserRepository.Update(jPushUser);

            _signals.Trigger(JPUSHUSER_PATTERN_KEY);

        }

        /// <summary>
        /// Deletes a jPushUser
        /// </summary>
        /// <param name="jPushUser">JPushUser</param>
        public virtual void Delete(JPushUser jPushUser) {
            if (jPushUser == null)
                throw new ArgumentNullException("jPushUser");

            _jPushUserRepository.Delete(jPushUser);

            _signals.Trigger(JPUSHUSER_PATTERN_KEY);

        }

        /// <summary>
        /// Deletes the specified register identifier.
        /// </summary>
        /// <param name="registerId">The register identifier.</param>
        /// <exception cref="System.ArgumentNullException">registerId</exception>
        public virtual void Delete(string registerId) {
            if (registerId == null)
                throw new ArgumentNullException("registerId");

            var jPushUsers = GetPaged(registerId);

            foreach (var jPushUser in jPushUsers)
                Delete(jPushUser);

            _signals.Trigger(JPUSHUSER_PATTERN_KEY);

        }

        #endregion
    }
}
