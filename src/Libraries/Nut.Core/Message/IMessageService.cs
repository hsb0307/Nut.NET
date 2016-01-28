using Nut.Core.Events;
using System.Collections.Generic;

namespace Nut.Core.Message {
    public interface IMessageService : IEventHandler {
        /// <summary>
        /// Sends the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="parameters">The parameters.</param>
        void Send(string type, IDictionary<string, object> parameters);
    }
}
