using System;

namespace Nut.Core.Caching {
    public interface ICacheHolder {
        ICache<TKey, TResult> GetCache<TKey, TResult>(Type component);
    }
}
