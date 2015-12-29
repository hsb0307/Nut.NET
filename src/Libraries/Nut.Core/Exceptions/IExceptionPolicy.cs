using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nut.Core.Exceptions {
    public interface IExceptionPolicy {

        /// <summary>
        /// Handles the exception.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="exception">The exception.</param>
        /// <returns></returns>
        bool HandleException(object sender, Exception exception);
    }
}
