using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nut.Core.Message {

    public interface IMessageChannelManager {
        /// <summary>
        /// Gets the message channel.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        IMessageChannel GetMessageChannel(string type, IDictionary<string, object> parameters);
    }
}
