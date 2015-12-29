using System;
using Nut.Core.Domain.Users;

namespace Nut.Services.Users {
    public interface IUserRegistrationService {

        /// <summary>
        /// Validates the user.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        UserLoginResults ValidateUser(string username, string password);

        /// <summary>
        /// Change password
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Result</returns>
        ChangePasswordResult ChangePassword(ChangePasswordRequest request);

        /// <summary>
        /// Sets a user email
        /// </summary>
        /// <param name="user">user</param>
        /// <param name="newEmail">New email</param>
        void SetEmail(User user, string newEmail);

        /// <summary>
        /// Sets a customer username
        /// </summary>
        /// <param name="user">user</param>
        /// <param name="newUsername">New Username</param>
        void SetUsername(User user, string newUsername);
    }
}
