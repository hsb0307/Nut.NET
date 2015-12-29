using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Management;
using System.Configuration;

namespace Nut.Core.License {
    public class DefaultMachineCodeProvider : IMachineCodeProvider {
        /// <summary>
        /// 获取机器码
        /// </summary>
        /// <returns>机器码 24位</returns>
        public string GetMachineCode() {
            //需保证唯一性
            Hash hash = new Hash();
            //硬盘卷标号
            //hash.AddString(GetDiskVolumeSerialNumber());
            //CPU序列号
            hash.AddString(getCpu());
            //网卡
            hash.AddString(GetMacAddress());
            return hash.Value;
        }

        /// <summary>
        /// 取得设备硬盘的卷标号
        /// </summary>
        /// <returns></returns>
        private string GetDiskVolumeSerialNumber() {
            try {
                ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObject disk = new ManagementObject("win32_logicaldisk.deviceid=\"c:\"");
                disk.Get();
                return disk.GetPropertyValue("VolumeSerialNumber").ToString();
            } catch {
                return "unknow";
            } finally {

            }
        }

        /// <summary>
        /// 获得CPU的序列号
        /// </summary>
        /// <returns></returns>
        private string getCpu() {
            try {
                string strCpu = null;
                ManagementClass myCpu = new ManagementClass("win32_Processor");
                ManagementObjectCollection myCpuConnection = myCpu.GetInstances();
                foreach (ManagementObject myObject in myCpuConnection) {
                    strCpu = myObject.Properties["Processorid"].Value.ToString();
                    break;
                }
                return strCpu;
            } catch {
                return "unknow";
            } finally {
            }
        }

        /// <summary>
        /// 获取Mac Address
        /// </summary>
        /// <returns></returns>
        private static string GetMacAddress() {
            try {
                //获取网卡硬件地址 
                string mac = "";
                ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc) {
                    if ((bool)mo["IPEnabled"] == true) {
                        mac = mo["MacAddress"].ToString();
                        break;
                    }
                }
                moc = null;
                mc = null;
                return mac;
            } catch {
                return "unknow";
            } finally {
            }

        }
    }
}
