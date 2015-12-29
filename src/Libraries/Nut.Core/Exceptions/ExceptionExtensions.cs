using System;
using System.Threading;
using System.Security;
using System.Runtime.InteropServices;

namespace Nut.Core.Exceptions {
    public static class ExceptionExtensions {
        public static bool IsFatal(this Exception ex) {
            return ex is StackOverflowException ||
                ex is OutOfMemoryException ||
                ex is AccessViolationException ||
                ex is AppDomainUnloadedException ||
                ex is ThreadAbortException ||
                ex is SecurityException ||
                ex is SEHException;

        }
    }
}
