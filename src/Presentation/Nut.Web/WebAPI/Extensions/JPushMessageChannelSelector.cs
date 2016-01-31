using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Nut.Core.Message;
using Nut.Core.Infrastructure;

namespace Nut.WebAPI.Extensions {
    public class JPushMessageChannelSelector : IMessageChannelSelector {


        public MessageChannelSelectorResult GetChannel(string messageType, object payload) {
            if (messageType == "JPush") {

                return new MessageChannelSelectorResult {
                    Priority = 50,
                    MessageChannel = () => EngineContext.Current.Resolve<IJPushChannel>()
                };
            }

            return null;
        }
    }
}