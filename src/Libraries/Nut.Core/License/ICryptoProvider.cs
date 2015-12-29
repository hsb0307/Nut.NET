using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nut.Core.License {
    /// <summary>
    /// 加密 提供商
    /// </summary>
    public interface ICryptoProvider {

        string Encrypt(string clearText);

    }
}
