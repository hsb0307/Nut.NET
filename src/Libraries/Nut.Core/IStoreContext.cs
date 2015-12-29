using Nut.Core.Domain.Stores;

namespace Nut.Core {
    public interface IStoreContext {
        /// <summary>
        /// Gets or sets the current store
        /// </summary>
        Store CurrentStore { get; }
    }
}
