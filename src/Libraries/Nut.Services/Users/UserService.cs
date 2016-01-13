using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Nut.Core;
using Nut.Core.Caching;
using Nut.Core.Data;
using Nut.Core.Domain.Users;
using Nut.Data;

namespace Nut.Services.Users {
    /// <summary>
    /// User service
    /// </summary>
    public partial class UserService : IUserService {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>
        private const string USERROLES_ALL_KEY = "Nop.Userrole.all-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : system name
        /// </remarks>
        private const string USERROLES_BY_SYSTEMNAME_KEY = "Nop.Userrole.systemname-{0}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string USERROLES_PATTERN_KEY = "Nop.Userrole.";

        #endregion

        #region Fields

        private readonly IRepository<User> _userRepository;
        private readonly IRepository<UserRole> _userRoleRepository;
        private readonly IDataProvider _dataProvider;
        private readonly IDbContext _dbContext;
        private readonly ICacheManager _cacheManager;
        private readonly ISignals _signals;

        #endregion

        #region Ctor

        public UserService(ICacheManager cacheManager,
            IRepository<User> userRepository,
            IRepository<UserRole> userRoleRepository,
            ISignals signals,
            IDataProvider dataProvider,
            IDbContext dbContext) {
            this._cacheManager = cacheManager;
            this._userRepository = userRepository;
            this._userRoleRepository = userRoleRepository;
            this._signals = signals;

            this._dataProvider = dataProvider;
            this._dbContext = dbContext;
        }

        #endregion

        #region Methods

        #region Users

        /// <summary>
        /// Gets all users.
        /// </summary>
        /// <param name="departmentId"></param>
        /// <param name="createdFromUtc">The created from UTC.</param>
        /// <param name="createdToUtc">The created to UTC.</param>
        /// <param name="UserRoleIds">The User role ids.</param>
        /// <param name="email">The email.</param>
        /// <param name="username">The username.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns></returns>
        public IPagedList<User> GetAllUsers(int departmentId = 0, DateTime? createdFromUtc = null,
             DateTime? createdToUtc = null, int[] UserRoleIds = null, string email = null, string username = null,
             int pageIndex = 0, int pageSize = 2147483647) //Int32.MaxValue
        {
            var query = _userRepository.Table;

            if (departmentId > 0)
                query = query.Where(c => c.DepartmentId == departmentId);

            if (createdFromUtc.HasValue)
                query = query.Where(c => createdFromUtc.Value <= c.CreatedOnUtc);
            if (createdToUtc.HasValue)
                query = query.Where(c => createdToUtc.Value >= c.CreatedOnUtc);

            query = query.Where(c => !c.Deleted);
            if (UserRoleIds != null && UserRoleIds.Length > 0)
                query = query.Where(c => c.UserRoles.Select(cr => cr.Id).Intersect(UserRoleIds).Any());
            if (!String.IsNullOrWhiteSpace(email))
                query = query.Where(c => c.Email.Contains(email));
            if (!String.IsNullOrWhiteSpace(username))
                query = query.Where(c => c.Username.Contains(username));

            query = query.OrderByDescending(c => c.CreatedOnUtc);

            var Users = new PagedList<User>(query, pageIndex, pageSize);
            return Users;
        }

        /// <summary>
        /// Gets all Users by User format (including deleted ones)
        /// </summary>
        /// <param name="passwordFormat">Password format</param>
        /// <returns>Users</returns>
        public virtual IList<User> GetAllUsersByPasswordFormat(PasswordFormat passwordFormat) {
            var passwordFormatId = (int)passwordFormat;

            var query = _userRepository.Table;
            query = query.Where(c => c.PasswordFormatId == passwordFormatId);
            query = query.OrderByDescending(c => c.CreatedOnUtc);
            var Users = query.ToList();
            return Users;
        }

        /// <summary>
        /// Gets online Users
        /// </summary>
        /// <param name="lastActivityFromUtc">User last activity date (from)</param>
        /// <param name="UserRoleIds">A list of User role identifiers to filter by (at least one match); pass null or empty list in order to load all Users; </param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Users</returns>
        public virtual IPagedList<User> GetOnlineUsers(DateTime lastActivityFromUtc,
            int[] UserRoleIds, int pageIndex = 0, int pageSize = int.MaxValue) {
            var query = _userRepository.Table;
            query = query.Where(c => lastActivityFromUtc <= c.LastActivityDateUtc);
            query = query.Where(c => !c.Deleted);
            if (UserRoleIds != null && UserRoleIds.Length > 0)
                query = query.Where(c => c.UserRoles.Select(cr => cr.Id).Intersect(UserRoleIds).Any());

            query = query.OrderByDescending(c => c.LastActivityDateUtc);
            var Users = new PagedList<User>(query, pageIndex, pageSize);
            return Users;
        }

        /// <summary>
        /// Delete a User
        /// </summary>
        /// <param name="User">User</param>
        public virtual void DeleteUser(User User) {
            if (User == null)
                throw new ArgumentNullException("User");

            if (User.IsSystemAccount)
                throw new NutException(string.Format("System User account ({0}) could not be deleted", User.Username));

            User.Deleted = true;

            UpdateUser(User);
        }

        /// <summary>
        /// Gets a User
        /// </summary>
        /// <param name="UserId">User identifier</param>
        /// <returns>A User</returns>
        public virtual User GetUserById(int UserId) {
            if (UserId == 0)
                return null;

            return _userRepository.GetById(UserId);
        }

        /// <summary>
        /// Get Users by identifiers
        /// </summary>
        /// <param name="UserIds">User identifiers</param>
        /// <returns>Users</returns>
        public virtual IList<User> GetUsersByIds(int[] UserIds) {
            if (UserIds == null || UserIds.Length == 0)
                return new List<User>();

            var query = from c in _userRepository.Table
                        where UserIds.Contains(c.Id)
                        select c;
            var Users = query.ToList();
            //sort by passed identifiers
            var sortedUsers = new List<User>();
            foreach (int id in UserIds) {
                var User = Users.Find(x => x.Id == id);
                if (User != null)
                    sortedUsers.Add(User);
            }
            return sortedUsers;
        }

        /// <summary>
        /// Gets a User by GUID
        /// </summary>
        /// <param name="UserGuid">User GUID</param>
        /// <returns>A User</returns>
        public virtual User GetUserByGuid(Guid UserGuid) {
            if (UserGuid == Guid.Empty)
                return null;

            var query = from c in _userRepository.Table
                        where c.UserGuid == UserGuid
                        orderby c.Id
                        select c;
            var User = query.FirstOrDefault();
            return User;
        }

        /// <summary>
        /// Get User by email
        /// </summary>
        /// <param name="email">Email</param>
        /// <returns>User</returns>
        public virtual User GetUserByEmail(string email) {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            var query = from c in _userRepository.Table
                        orderby c.Id
                        where c.Email == email
                        select c;
            var User = query.FirstOrDefault();
            return User;
        }

        /// <summary>
        /// Get User by username
        /// </summary>
        /// <param name="username">Username</param>
        /// <returns>User</returns>
        public virtual User GetUserByUsername(string username) {
            if (string.IsNullOrWhiteSpace(username))
                return null;

            var query = from c in _userRepository.Table
                        orderby c.Id
                        where c.Username == username
                        select c;
            var User = query.FirstOrDefault();
            return User;
        }

        /// <summary>
        /// Insert a guest User
        /// </summary>
        /// <returns>User</returns>
        public virtual User InsertGuestUser() {
            var User = new User {
                UserGuid = Guid.NewGuid(),
                Active = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                DepartmentId = 1
            };

            //add to 'Guests' role
            var guestRole = GetUserRoleBySystemName(SystemUserRoleNames.Guests);
            if (guestRole == null)
                throw new NutException("'Guests' role could not be loaded");
            User.UserRoles.Add(guestRole);

            _userRepository.Insert(User);

            return User;
        }

        /// <summary>
        /// Insert a User
        /// </summary>
        /// <param name="User">User</param>
        public virtual void InsertUser(User User) {
            if (User == null)
                throw new ArgumentNullException("User");

            _userRepository.Insert(User);

        }

        /// <summary>
        /// Updates the User
        /// </summary>
        /// <param name="User">User</param>
        public virtual void UpdateUser(User User) {
            if (User == null)
                throw new ArgumentNullException("User");

            _userRepository.Update(User);
        }


        /// <summary>
        /// Delete guest User records
        /// </summary>
        /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
        /// <param name="onlyWithoutShoppingCart">A value indicating whether to delete Users only without shopping cart</param>
        /// <returns>Number of deleted Users</returns>
        public virtual int DeleteGuestUsers(DateTime? createdFromUtc, DateTime? createdToUtc, bool onlyWithoutShoppingCart) {

            //stored procedures aren't supported. Use LINQ

            var guestRole = GetUserRoleBySystemName(SystemUserRoleNames.Guests);
            if (guestRole == null)
                throw new NutException("'Guests' role could not be loaded");

            var query = _userRepository.Table;
            if (createdFromUtc.HasValue)
                query = query.Where(c => createdFromUtc.Value <= c.CreatedOnUtc);
            if (createdToUtc.HasValue)
                query = query.Where(c => createdToUtc.Value >= c.CreatedOnUtc);
            query = query.Where(c => c.UserRoles.Select(cr => cr.Id).Contains(guestRole.Id));


            //don't delete system accounts
            query = query.Where(c => !c.IsSystemAccount);

            //only distinct Users (group by ID)
            query = from c in query
                    group c by c.Id
                        into cGroup
                    orderby cGroup.Key
                    select cGroup.FirstOrDefault();
            query = query.OrderBy(c => c.Id);
            var Users = query.ToList();


            int totalRecordsDeleted = 0;
            foreach (var c in Users) {
                try {
                    //delete from database
                    _userRepository.Delete(c);
                    totalRecordsDeleted++;
                } catch (Exception exc) {
                    Debug.WriteLine(exc);
                }
            }
            return totalRecordsDeleted;

        }

        #endregion

        #region User roles

        /// <summary>
        /// Delete a User role
        /// </summary>
        /// <param name="UserRole">User role</param>
        public virtual void DeleteUserRole(UserRole UserRole) {
            if (UserRole == null)
                throw new ArgumentNullException("UserRole");

            if (UserRole.IsSystemRole)
                throw new NutException("System role could not be deleted");

            _userRoleRepository.Delete(UserRole);

            _signals.Trigger(USERROLES_PATTERN_KEY);

        }

        /// <summary>
        /// Gets a User role
        /// </summary>
        /// <param name="UserRoleId">User role identifier</param>
        /// <returns>User role</returns>
        public virtual UserRole GetUserRoleById(int UserRoleId) {
            if (UserRoleId == 0)
                return null;

            return _userRoleRepository.GetById(UserRoleId);
        }

        /// <summary>
        /// Gets a User role
        /// </summary>
        /// <param name="systemName">User role system name</param>
        /// <returns>User role</returns>
        public virtual UserRole GetUserRoleBySystemName(string systemName) {
            if (String.IsNullOrWhiteSpace(systemName))
                return null;

            string key = string.Format(USERROLES_BY_SYSTEMNAME_KEY, systemName);
            return _cacheManager.Get(key, ctx => {

                ctx.Monitor(_signals.When("USERROLES_PATTERN_KEY"));

                var query = from cr in _userRoleRepository.Table
                            orderby cr.Id
                            where cr.SystemName == systemName
                            select cr;
                var UserRole = query.FirstOrDefault();
                return UserRole;
            });
        }

        /// <summary>
        /// Gets all User roles
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>User roles</returns>
        public virtual IList<UserRole> GetAllUserRoles(bool showHidden = false) {
            string key = string.Format(USERROLES_ALL_KEY, showHidden);
            return _cacheManager.Get(key, ctx => {
                ctx.Monitor(_signals.When("USERROLES_PATTERN_KEY"));

                var query = from cr in _userRoleRepository.Table
                            orderby cr.Name
                            where (showHidden || cr.Active)
                            select cr;
                var UserRoles = query.ToList();
                return UserRoles;
            });
        }

        /// <summary>
        /// Inserts a User role
        /// </summary>
        /// <param name="UserRole">User role</param>
        public virtual void InsertUserRole(UserRole UserRole) {
            if (UserRole == null)
                throw new ArgumentNullException("UserRole");

            _userRoleRepository.Insert(UserRole);

            _signals.Trigger(USERROLES_PATTERN_KEY);

        }

        /// <summary>
        /// Updates the User role
        /// </summary>
        /// <param name="UserRole">User role</param>
        public virtual void UpdateUserRole(UserRole UserRole) {
            if (UserRole == null)
                throw new ArgumentNullException("UserRole");

            _userRoleRepository.Update(UserRole);

            _signals.Trigger(USERROLES_PATTERN_KEY);
        }

        #endregion

        #endregion
    }
}