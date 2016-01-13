using System;
using System.Collections.Generic;
using Nut.Core;
using Nut.Core.Domain.Users;

namespace Nut.Services.Users {
    /// <summary>
    /// User service interface
    /// </summary>
    public partial interface IUserService {
        #region Users

        /// <summary>
        /// Gets all users.
        /// </summary>
        /// <param name="departmentId">The department identifier.</param>
        /// <param name="createdFromUtc">The created from UTC.</param>
        /// <param name="createdToUtc">The created to UTC.</param>
        /// <param name="UserRoleIds">The user role ids.</param>
        /// <param name="email">The email.</param>
        /// <param name="username">The username.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns></returns>
        IPagedList<User> GetAllUsers(int departmentId = 0, DateTime? createdFromUtc = null,
            DateTime? createdToUtc = null, int[] UserRoleIds = null, string email = null, string username = null,
            int pageIndex = 0, int pageSize = 2147483647); //Int32.MaxValue

        /// <summary>
        /// Gets all Users by password format.
        /// </summary>
        /// <param name="passwordFormat">The password format.</param>
        /// <returns>User</returns>
        IList<User> GetAllUsersByPasswordFormat(PasswordFormat passwordFormat);

        /// <summary>
        /// Gets the online users.
        /// </summary>
        /// <param name="lastActivityFromUtc">The last activity from UTC.</param>
        /// <param name="UserRoleIds">The User role ids.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns></returns>
        IPagedList<User> GetOnlineUsers(DateTime lastActivityFromUtc,
            int[] UserRoleIds, int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Deletes the user.
        /// </summary>
        /// <param name="User">The User.</param>
        void DeleteUser(User User);

        /// <summary>
        /// Gets the user by identifier.
        /// </summary>
        /// <param name="UserId">The User identifier.</param>
        /// <returns></returns>
        User GetUserById(int userId);

        /// <summary>
        /// Gets the users by ids.
        /// </summary>
        /// <param name="userIds">The user ids.</param>
        /// <returns></returns>
        IList<User> GetUsersByIds(int[] userIds);

        /// <summary>
        /// Gets the user by unique identifier.
        /// </summary>
        /// <param name="userGuid">The user unique identifier.</param>
        /// <returns></returns>
        User GetUserByGuid(Guid userGuid);

        /// <summary>
        /// Gets the user by email.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        User GetUserByEmail(string email);

        /// <summary>
        /// Get User by username
        /// </summary>
        /// <param name="username">Username</param>
        /// <returns>User</returns>
        User GetUserByUsername(string username);

        /// <summary>
        /// Insert a guest User
        /// </summary>
        /// <returns>User</returns>
        User InsertGuestUser();

        /// <summary>
        /// Insert a User
        /// </summary>
        /// <param name="User">User</param>
        void InsertUser(User User);

        /// <summary>
        /// Updates the User
        /// </summary>
        /// <param name="User">User</param>
        void UpdateUser(User User);

        /// <summary>
        /// Delete guest User records
        /// </summary>
        /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
        /// <param name="onlyWithoutShoppingCart">A value indicating whether to delete Users only without shopping cart</param>
        /// <returns>Number of deleted Users</returns>
        int DeleteGuestUsers(DateTime? createdFromUtc, DateTime? createdToUtc, bool onlyWithoutShoppingCart);

        #endregion

        #region User roles

        /// <summary>
        /// Delete a User role
        /// </summary>
        /// <param name="UserRole">User role</param>
        void DeleteUserRole(UserRole UserRole);

        /// <summary>
        /// Gets a User role
        /// </summary>
        /// <param name="UserRoleId">User role identifier</param>
        /// <returns>User role</returns>
        UserRole GetUserRoleById(int UserRoleId);

        /// <summary>
        /// Gets a User role
        /// </summary>
        /// <param name="systemName">User role system name</param>
        /// <returns>User role</returns>
        UserRole GetUserRoleBySystemName(string systemName);

        /// <summary>
        /// Gets all User roles
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>User roles</returns>
        IList<UserRole> GetAllUserRoles(bool showHidden = false);

        /// <summary>
        /// Inserts a User role
        /// </summary>
        /// <param name="UserRole">User role</param>
        void InsertUserRole(UserRole UserRole);

        /// <summary>
        /// Updates the User role
        /// </summary>
        /// <param name="UserRole">User role</param>
        void UpdateUserRole(UserRole UserRole);

        #endregion
    }
}