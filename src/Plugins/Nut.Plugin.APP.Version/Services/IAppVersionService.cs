using System.Collections.Generic;
using Nut.Core;
using Nut.Plugin.APP.Version.Domain;

namespace Nut.Plugin.APP.Version.Services {
    public interface IAppVersionService {
        /// <summary>
        /// Gets all AppVersions
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>AppVersion collection</returns>
        IList<AppVersion> GetAll(bool showHidden = false);


        IPagedList<AppVersion> GetPaged(int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false);

        /// <summary>
        /// Gets a appVersion 
        /// </summary>
        /// <param name="countryId">AppVersion identifier</param>
        /// <returns>AppVersion</returns>
        AppVersion GetById(int id);

        /// <summary>
        /// Inserts a appVersion
        /// </summary>
        /// <param name="appVersion">AppVersion</param>
        void Insert(AppVersion appVersion);

        /// <summary>
        /// Updates the appVersion
        /// </summary>
        /// <param name="appVersion">AppVersion</param>
        void Update(AppVersion appVersion);
        /// <summary>
        /// Deletes a appVersion
        /// </summary>
        /// <param name="appVersion">AppVersion</param>
        void Delete(AppVersion appversion);
    }
}
