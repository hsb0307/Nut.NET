using Nut.Core.Message;

namespace Nut.Services.Message {
    public class NullMessageChannelSelector : IMessageChannelSelector {
        /// <summary>
        /// Gets the channel.
        /// </summary>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="payload">The payload.</param>
        /// <returns></returns>
        public MessageChannelSelectorResult GetChannel(string messageType, object payload) {
            return null;
        }
    }
}
