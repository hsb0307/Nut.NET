using Nut.Core;
using Nut.Core.Domain.App;
using System.Collections.Generic;

namespace Nut.Services.App {
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
