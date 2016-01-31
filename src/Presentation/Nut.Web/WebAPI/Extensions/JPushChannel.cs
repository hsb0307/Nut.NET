using System.Collections.Generic;
using Nut.Core.Logging;

namespace Nut.WebAPI.Extensions {
    public class JPushChannel : IJPushChannel {

        public JPushChannel() {
            this.Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public void Process(IDictionary<string, object> parameters) {

            Logger.Debug("JPush Test");
        }
    }
}