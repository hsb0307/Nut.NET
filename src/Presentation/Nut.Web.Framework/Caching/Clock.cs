using System;
using Nut.Core.Caching;

namespace Nut.Web.Framework.Caching {
    public class Clock : IClock {
        public DateTime UtcNow {
            get { return DateTime.Now; }
        }

        public IVolatileToken When(TimeSpan duration) {
            return new AbsoluteExpirationToken(this, duration);
        }

        public IVolatileToken WhenUtc(DateTime absoluteUtc) {
            return new AbsoluteExpirationToken(this, absoluteUtc);
        }

        public class AbsoluteExpirationToken : IVolatileToken {
            private readonly IClock _clock;
            private readonly DateTime _invalidateUtc;

            public AbsoluteExpirationToken(IClock clock, DateTime invalidateUtc) {
                _clock = clock;
                _invalidateUtc = invalidateUtc;
            }

            public AbsoluteExpirationToken(IClock clock, TimeSpan duration) {
                _clock = clock;
                _invalidateUtc = _clock.UtcNow.Add(duration);
            }

            public bool IsCurrent {
                get {
                    return _clock.UtcNow < _invalidateUtc;
                }
            }
        }
    }
}
