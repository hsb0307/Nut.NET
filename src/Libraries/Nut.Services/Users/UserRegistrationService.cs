using System;
using System.Linq;
using Nut.Core;
using Nut.Core.Domain.Users;
using Nut.Services.Localization;
using Nut.Services.Security;
using Nut.Services.Stores;

namespace Nut.Services.Users {
    public class UserRegistrationService : IUserRegistrationService {
        #region Fields

        private readonly IUserService _userService;
        private readonly IEncryptionService _encryptionService;
        private readonly ILocalizationService _localizationService;
        private readonly IStoreService _storeService;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="customerService">Customer service</param>
        /// <param name="encryptionService">Encryption service</param>
        /// <param name="newsLetterSubscriptionService">Newsletter subscription service</param>
        /// <param name="localizationService">Localization service</param>
        /// <param name="storeService">Store service</param>
        /// <param name="rewardPointsSettings">Reward points settings</param>
        /// <param name="customerSettings">Customer settings</param>
        public UserRegistrationService(IUserService userService,
            IEncryptionService encryptionService,
            ILocalizationService localizationService,
            IStoreService storeService) {
            this._userService = userService;
            this._encryptionService = encryptionService;
            this._localizationService = localizationService;
            this._storeService = storeService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Validates the user.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public UserLoginResults ValidateUser(string username, string password) {
            User user;

            user = _userService.GetUserByUsername(username);

            if (user == null)
                return UserLoginResults.CustomerNotExist;
            if (user.Deleted)
                return UserLoginResults.Deleted;
            if (!user.Active)
                return UserLoginResults.NotActive;
            //only registered can login
            if (!user.IsRegistered())
                return UserLoginResults.NotRegistered;

            string pwd = "";
            switch (user.PasswordFormat) {
                case PasswordFormat.Encrypted:
                    pwd = _encryptionService.EncryptText(password);
                    break;
                case PasswordFormat.Hashed:
                    pwd = _encryptionService.CreatePasswordHash(password, user.PasswordSalt);
                    break;
                default:
                    pwd = password;
                    break;
            }

            bool isValid = pwd == user.Password;
            if (!isValid)
                return UserLoginResults.WrongPassword;

            //save last login date
            user.LastLoginDateUtc = DateTime.UtcNow;
            _userService.UpdateUser(user);

            return UserLoginResults.Successful;
        }

        /// <summary>
        /// Change password
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Result</returns>
        public virtual ChangePasswordResult ChangePassword(ChangePasswordRequest request) {
            if (request == null)
                throw new ArgumentNullException("request");

            var result = new ChangePasswordResult();
            if (String.IsNullOrWhiteSpace(request.Username)) {
                result.AddError(_localizationService.GetResource("Account.ChangePassword.Errors.EmailIsNotProvided"));
                return result;
            }
            if (String.IsNullOrWhiteSpace(request.NewPassword)) {
                result.AddError(_localizationService.GetResource("Account.ChangePassword.Errors.PasswordIsNotProvided"));
                return result;
            }

            var user = _userService.GetUserByUsername(request.Username);
            if (user == null) {
                result.AddError(_localizationService.GetResource("Account.ChangePassword.Errors.UsernameNotFound"));
                return result;
            }


            var requestIsValid = false;
            if (request.ValidateRequest) {
                //password
                string oldPwd = "";
                switch (user.PasswordFormat) {
                    case PasswordFormat.Encrypted:
                        oldPwd = _encryptionService.EncryptText(request.OldPassword);
                        break;
                    case PasswordFormat.Hashed:
                        oldPwd = _encryptionService.CreatePasswordHash(request.OldPassword, user.PasswordSalt);
                        break;
                    default:
                        oldPwd = request.OldPassword;
                        break;
                }

                bool oldPasswordIsValid = oldPwd == user.Password;
                if (!oldPasswordIsValid)
                    result.AddError(_localizationService.GetResource("Account.ChangePassword.Errors.OldPasswordDoesntMatch"));

                if (oldPasswordIsValid)
                    requestIsValid = true;
            } else
                requestIsValid = true;


            //at this point request is valid
            if (requestIsValid) {
                switch (request.NewPasswordFormat) {
                    case PasswordFormat.Clear:
                        {
                            user.Password = request.NewPassword;
                        }
                        break;
                    case PasswordFormat.Encrypted:
                        {
                            user.Password = _encryptionService.EncryptText(request.NewPassword);
                        }
                        break;
                    case PasswordFormat.Hashed:
                        {
                            string saltKey = _encryptionService.CreateSaltKey(5);
                            user.PasswordSalt = saltKey;
                            user.Password = _encryptionService.CreatePasswordHash(request.NewPassword, saltKey);
                        }
                        break;
                    default:
                        break;
                }
                user.PasswordFormat = request.NewPasswordFormat;
                _userService.UpdateUser(user);
            }

            return result;
        }

        /// <summary>
        /// Sets a user email
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="newEmail">New email</param>
        public virtual void SetEmail(User user, string newEmail) {
            if (user == null)
                throw new ArgumentNullException("customer");

            if (newEmail == null)
                throw new NutException("Email cannot be null");

            newEmail = newEmail.Trim();
            string oldEmail = user.Email;

            if (!CommonHelper.IsValidEmail(newEmail))
                throw new NutException(_localizationService.GetResource("Account.EmailUsernameErrors.NewEmailIsNotValid"));

            if (newEmail.Length > 100)
                throw new NutException(_localizationService.GetResource("Account.EmailUsernameErrors.EmailTooLong"));

            var customer2 = _userService.GetUserByEmail(user.Email);
            if (customer2 != null && user.Id != customer2.Id)
                throw new NutException(_localizationService.GetResource("Account.EmailUsernameErrors.EmailAlreadyExists"));

            user.Email = newEmail;
            _userService.UpdateUser(user);

        }

        /// <summary>
        /// Sets a customer username
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="newUsername">New Username</param>
        public virtual void SetUsername(User user, string newUsername) {
            if (user == null)
                throw new ArgumentNullException("customer");

            newUsername = newUsername.Trim();

            if (newUsername.Length > 100)
                throw new NutException(_localizationService.GetResource("Account.EmailUsernameErrors.UsernameTooLong"));

            var user2 = _userService.GetUserByUsername(user.Username);
            if (user2 != null && user.Id != user2.Id)
                throw new NutException(_localizationService.GetResource("Account.EmailUsernameErrors.UsernameAlreadyExists"));

            user.Username = newUsername;
            _userService.UpdateUser(user);
        }

        #endregion
    }
}
