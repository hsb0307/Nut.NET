using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nut.Core.Message {
    public interface IMessageChannelSelector {
        MessageChannelSelectorResult GetChannel(string messageType, object payload);
    }

    public class MessageChannelSelectorResult {
        public int Priority { get; set; }
        public Func<IMessageChannel> MessageChannel { get; set; }
    }
}
