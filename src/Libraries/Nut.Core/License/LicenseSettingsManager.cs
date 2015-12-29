using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Hosting;

namespace Nut.Core.License {
    public partial class LicenseSettingsManager {
        protected const char separator = ':';
        protected const string filename = "License.txt";

        /// <summary>
        /// Maps a virtual path to a physical disk path.
        /// </summary>
        /// <param name="path">The path to map. E.g. "~/bin"</param>
        /// <returns>The physical path. E.g. "c:\inetpub\wwwroot\bin"</returns>
        protected virtual string MapPath(string path) {
            if (HostingEnvironment.IsHosted) {
                //hosted
                return HostingEnvironment.MapPath(path);
            } else {
                //not hosted. For example, run in unit tests
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                path = path.Replace("~/", "").TrimStart('/').Replace('/', '\\');
                return Path.Combine(baseDirectory, path);
            }
        }

        /// <summary>
        /// Load settings
        /// </summary>
        /// <param name="filePath">File path; pass null to use default settings file path</param>
        /// <returns></returns>
        public virtual string LoadSettings(string filePath = null) {
            if (String.IsNullOrEmpty(filePath)) {
                //use webHelper.MapPath instead of HostingEnvironment.MapPath which is not available in unit tests
                filePath = Path.Combine(MapPath("~/App_Data/"), filename);
            }
            if (File.Exists(filePath)) {
                string text = File.ReadAllText(filePath);
                return text;
            } else
                return string.Empty;
        }

        public virtual void SaveSettings(string licenseCode) {
            if (licenseCode == null)
                throw new ArgumentNullException("licenseCode");

            //use webHelper.MapPath instead of HostingEnvironment.MapPath which is not available in unit tests
            string filePath = Path.Combine(MapPath("~/App_Data/"), filename);
            if (File.Exists(filePath)) {
                File.Delete(filePath);
            }
            using (File.Create(filePath)) {
                //we use 'using' to close the file after it's created
            }
            File.WriteAllText(filePath, licenseCode);
        }

    }
}
