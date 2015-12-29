using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nut.Core.Domain.Users {
    public enum PasswordFormat : int {
        Clear = 0,
        Hashed = 1,
        Encrypted = 2
    }
}
