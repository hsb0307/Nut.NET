using System.Collections.Generic;
using Nut.Core.Logging;
using Nut.Core.Message;

namespace Nut.Services.Message {
   public  class DefaultMessageService : IMessageService {

        private readonly IMessageChannelManager _messageChannelManager;

        public DefaultMessageService(IMessageChannelManager messageChannelManager) {
            _messageChannelManager = messageChannelManager;

            this.Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public void Send(string type, IDictionary<string, object> parameters) {
            var messageChannel = _messageChannelManager.GetMessageChannel(type, parameters);

            if (messageChannel == null) {
                Logger.Information("No channels where found to process a message of type {0}", type);
                return;
            }

            messageChannel.Process(parameters);
        }
    }
}
