using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nut.Core.Message {
    public interface IMessageChannel {
        void Process(IDictionary<string, object> parameters);
    }
}
