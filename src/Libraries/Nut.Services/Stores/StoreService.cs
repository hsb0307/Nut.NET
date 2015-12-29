using System;
using System.Collections.Generic;
using System.Linq;
using Nut.Core.Caching;
using Nut.Core.Data;
using Nut.Core.Domain.Stores;

namespace Nut.Services.Stores {
    /// <summary>
    /// Store service
    /// </summary>
    public partial class StoreService : IStoreService {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        private const string STORES_ALL_KEY = "Nop.stores.all";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : store ID
        /// </remarks>
        private const string STORES_BY_ID_KEY = "Nop.stores.id-{0}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string STORES_PATTERN_KEY = "Nop.stores.";

        #endregion

        #region Fields

        private readonly IRepository<Store> _storeRepository;
        private readonly ICacheManager _cacheManager;
        private readonly ISignals _signals;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="storeRepository">Store repository</param>
        public StoreService(ICacheManager cacheManager,
            IRepository<Store> storeRepository,
            ISignals signals) {
            this._cacheManager = cacheManager;
            this._storeRepository = storeRepository;
            this._signals = signals;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a store
        /// </summary>
        /// <param name="store">Store</param>
        public virtual void DeleteStore(Store store) {
            if (store == null)
                throw new ArgumentNullException("store");

            var allStores = GetAllStores();
            if (allStores.Count == 1)
                throw new Exception("You cannot delete the only configured store");

            _storeRepository.Delete(store);

            _signals.Trigger(STORES_PATTERN_KEY);
        }

        /// <summary>
        /// Gets all stores
        /// </summary>
        /// <returns>Stores</returns>
        public virtual IList<Store> GetAllStores() {
            string key = STORES_ALL_KEY;
            return _cacheManager.Get(key, ctx => {
                ctx.Monitor(_signals.When(STORES_PATTERN_KEY));

                var query = from s in _storeRepository.Table
                            orderby s.DisplayOrder, s.Id
                            select s;
                var stores = query.ToList();
                return stores;
            });
        }

        /// <summary>
        /// Gets a store 
        /// </summary>
        /// <param name="storeId">Store identifier</param>
        /// <returns>Store</returns>
        public virtual Store GetStoreById(int storeId) {
            if (storeId == 0)
                return null;

            string key = string.Format(STORES_BY_ID_KEY, storeId);
            return _cacheManager.Get(key, ctx => {
                ctx.Monitor(_signals.When(STORES_PATTERN_KEY));

                return _storeRepository.GetById(storeId);
            });
        }

        /// <summary>
        /// Inserts a store
        /// </summary>
        /// <param name="store">Store</param>
        public virtual void InsertStore(Store store) {
            if (store == null)
                throw new ArgumentNullException("store");

            _storeRepository.Insert(store);

            _signals.Trigger(STORES_PATTERN_KEY);
        }

        /// <summary>
        /// Updates the store
        /// </summary>
        /// <param name="store">Store</param>
        public virtual void UpdateStore(Store store) {
            if (store == null)
                throw new ArgumentNullException("store");

            _storeRepository.Update(store);

            _signals.Trigger(STORES_PATTERN_KEY);
        }

        #endregion
    }
}