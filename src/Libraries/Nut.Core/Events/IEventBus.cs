using System.Collections.Generic;
using System.Collections;

namespace Nut.Core.Events {
    public interface IEventBus {
        IEnumerable Notify(string messageName, IDictionary<string, object> eventData);
    }
}
