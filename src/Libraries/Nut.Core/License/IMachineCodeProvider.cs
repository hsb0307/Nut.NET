using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nut.Core.License {
    /// <summary>
    /// 机器码提供者
    /// </summary>
    public interface IMachineCodeProvider {
        /// <summary>
        /// 获取机器码
        /// </summary>
        /// <returns>机器码 24位</returns>
        string GetMachineCode();
    }
}
