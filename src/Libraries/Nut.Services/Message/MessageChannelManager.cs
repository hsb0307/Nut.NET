using System.Collections.Generic;
using System.Linq;
using Nut.Core.Message;

namespace Nut.Services.Message {
    public class MessageChannelManager : IMessageChannelManager {

        private readonly IEnumerable<IMessageChannelSelector> _messageChannelSelectors;

        public MessageChannelManager(IEnumerable<IMessageChannelSelector> messageChannelSelectors) {
            _messageChannelSelectors = messageChannelSelectors;
        }

        public IMessageChannel GetMessageChannel(string type, IDictionary<string, object> parameters) {
            var messageChannelResult = _messageChannelSelectors
                .Select(x => x.GetChannel(type, parameters))
                .Where(x => x != null)
                .OrderByDescending(x => x.Priority)
                .FirstOrDefault();

            return messageChannelResult == null ? null : messageChannelResult.MessageChannel();
        }
    }
}
