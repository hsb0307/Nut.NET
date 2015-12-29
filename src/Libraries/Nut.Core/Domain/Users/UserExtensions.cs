using System;
using System.Linq;

namespace Nut.Core.Domain.Users
{
    public static class UserExtensions
    {
        #region User role

        /// <summary>
        /// Gets a value indicating whether User is in a certain User role
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="UserRoleSystemName">User role system name</param>
        /// <param name="onlyActiveUserRoles">A value indicating whether we should look only in active User roles</param>
        /// <returns>Result</returns>
        public static bool IsInUserRole(this User user,
            string userRoleSystemName, bool onlyActiveUserRoles = true)
        {
            if (user == null)
                throw new ArgumentNullException("User");

            if (String.IsNullOrEmpty(userRoleSystemName))
                throw new ArgumentNullException("UserRoleSystemName");

            var result = user.UserRoles
                .FirstOrDefault(cr => (!onlyActiveUserRoles || cr.Active) && (cr.SystemName == userRoleSystemName)) != null;
            return result;
        }


        /// <summary>
        /// Gets a value indicating whether User is administrator
        /// </summary>
        /// <param name="User">User</param>
        /// <param name="onlyActiveUserRoles">A value indicating whether we should look only in active User roles</param>
        /// <returns>Result</returns>
        public static bool IsAdmin(this User User, bool onlyActiveUserRoles = true)
        {
            return IsInUserRole(User, SystemUserRoleNames.Administrators, onlyActiveUserRoles);
        }


        /// <summary>
        /// Gets a value indicating whether User is registered
        /// </summary>
        /// <param name="User">User</param>
        /// <param name="onlyActiveUserRoles">A value indicating whether we should look only in active User roles</param>
        /// <returns>Result</returns>
        public static bool IsRegistered(this User User, bool onlyActiveUserRoles = true)
        {
            return IsInUserRole(User, SystemUserRoleNames.Registered, onlyActiveUserRoles);
        }

        /// <summary>
        /// Gets a value indicating whether User is guest
        /// </summary>
        /// <param name="User">User</param>
        /// <param name="onlyActiveUserRoles">A value indicating whether we should look only in active User roles</param>
        /// <returns>Result</returns>
        public static bool IsGuest(this User User, bool onlyActiveUserRoles = true)
        {
            return IsInUserRole(User, SystemUserRoleNames.Guests, onlyActiveUserRoles);
        }

        #endregion

    }
}
