using System;
using Nut.Core.Events;
using Nut.Core.Logging;

namespace Nut.Core.Exceptions {
    public class ExceptionPolicy : IExceptionPolicy {
        public ExceptionPolicy() {
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public bool HandleException(object sender, Exception exception) {
            if (exception.IsFatal())
                return false;

            if (sender is IEventBus)
                return false;

            Logger.Error(exception, "An Unexpected exception was caught");

            //Notifier

            return true;
        }
    }
}
