using System.Collections.Generic;

using Nut.Core;
using Nut.Core.Domain.App;

namespace Nut.Services.App {

    /// <summary>
    /// JPushUser service interface
    /// </summary>
    public interface IJPushUserService {
        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns></returns>
        IList<JPushUser> GetAll();


        IPagedList<JPushUser> GetPaged(string registerId = "", int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Gets a jPushUser 
        /// </summary>
        /// <param name="countryId">JPushUser identifier</param>
        /// <returns>JPushUser</returns>
        JPushUser GetById(int id);

        /// <summary>
        /// Inserts a jPushUser
        /// </summary>
        /// <param name="jPushUser">JPushUser</param>
        void Insert(JPushUser jPushUser);

        /// <summary>
        /// Updates the jPushUser
        /// </summary>
        /// <param name="jPushUser">JPushUser</param>
        void Update(JPushUser jPushUser);
        /// <summary>
        /// Deletes a jPushUser
        /// </summary>
        /// <param name="jPushUser">JPushUser</param>
        void Delete(JPushUser jpushuser);

        /// <summary>
        /// Deletes the specified register identifier.
        /// </summary>
        /// <param name="registerId"></param>
        void Delete(string registerId);
    }
}
