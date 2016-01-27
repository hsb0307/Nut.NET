using Nut.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nut.Core.Domain.Common {
    public class WebAPISettings : ISettings {
        public string UserEncryptionKey { get; set; }
    }
}
