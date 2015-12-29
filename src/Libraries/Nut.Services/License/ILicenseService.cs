using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nut.Services.License {
    public interface ILicenseService {
        /// <summary>
        /// 安装注册码
        /// </summary>
        /// <param name="LicenseCode"></param>
        void Install(string LicenseCode);
        /// <summary>
        /// 验证注册码
        /// </summary>
        /// <param name="LicenseCode"></param>
        /// <returns></returns>
        bool TryCheckAccess(string LicenseCode);
        /// <summary>
        /// 检查授权
        /// </summary>
        /// <returns></returns>
        bool CheckAccess();
    }
}
