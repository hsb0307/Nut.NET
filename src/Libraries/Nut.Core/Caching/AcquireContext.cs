using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nut.Core.Caching {
    public interface IAcquireContext {
        Action<IVolatileToken> Monitor { get; }
    }

    public class AcquireContext<TKey> : IAcquireContext {
        public AcquireContext(TKey key, Action<IVolatileToken> monitor) {
            Key = key;
            Monitor = monitor;
        }

        public TKey Key { get; private set; }
        public Action<IVolatileToken> Monitor { get; private set; }
    }

    /// <summary>
    /// Simple implementation of "IAcquireContext" given a lamdba
    /// </summary>
    public class SimpleAcquireContext : IAcquireContext {
        private readonly Action<IVolatileToken> _monitor;

        public SimpleAcquireContext(Action<IVolatileToken> monitor) {
            _monitor = monitor;
        }

        public Action<IVolatileToken> Monitor {
            get { return _monitor; }
        }
    }
}
