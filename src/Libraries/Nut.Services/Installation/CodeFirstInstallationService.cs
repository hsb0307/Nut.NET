using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nut.Core;
using Nut.Core.Data;
using Nut.Core.Domain.Localization;
using Nut.Core.Domain.Stores;
using Nut.Core.Domain.Tasks;
using Nut.Core.Domain.Users;
using Nut.Core.Infrastructure;
using Nut.Services.Localization;
using Nut.Services.Configuration;
using Nut.Services.Users;
using Nut.Core.Domain.Logging;

namespace Nut.Services.Installation {
    public partial class CodeFirstInstallationService : IInstallationService {
        #region Fields

        private readonly IRepository<Store> _storeRepository;
        private readonly IRepository<Language> _languageRepository;
        private readonly IRepository<Department> _departmentRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<UserRole> _userRoleRepository;
        private readonly IRepository<ScheduleTask> _scheduleTaskRepository;
        private readonly IRepository<ActivityLogType> _activityLogTypeRepository;
        private readonly IWebHelper _webHelper;

        #endregion

        #region Ctor

        public CodeFirstInstallationService(IRepository<Store> storeRepository,
            IRepository<Language> languageRepository,
            IRepository<Department> departmentRepository,
            IRepository<User> userRepository,
            IRepository<UserRole> userRoleRepository,
            IRepository<ScheduleTask> scheduleTaskRepository,
            IRepository<ActivityLogType> activityLogTypeRepository,
            IWebHelper webHelper) {
            this._storeRepository = storeRepository;
            this._languageRepository = languageRepository;
            this._departmentRepository = departmentRepository;
            this._userRepository = userRepository;
            this._userRoleRepository = userRoleRepository;

            this._scheduleTaskRepository = scheduleTaskRepository;
            this._activityLogTypeRepository = activityLogTypeRepository;
            this._webHelper = webHelper;
        }

        #endregion

        #region Utilities

        protected virtual void InstallStores() {
            //var storeUrl = "http://www.yourStore.com/";
            var storeUrl = _webHelper.GetStoreLocation(false);
            var stores = new List<Store>
           {
                new Store
                {
                    Name = "Your store name",
                    Url = storeUrl,
                    SslEnabled = false,
                    Hosts = "yourstore.com,www.yourstore.com",
                    DisplayOrder = 1
                },
            };

            _storeRepository.Insert(stores);
        }

        protected virtual void InstallLanguages() {
            var language = new Language {
                Name = "English",
                LanguageCulture = "en-US",
                UniqueSeoCode = "en",
                FlagImageFileName = "us.png",
                Published = true,
                DisplayOrder = 1
            };
            _languageRepository.Insert(language);
        }

        protected virtual void InstallLocaleResources() {
            //'English' language
            var language = _languageRepository.Table.Single(l => l.Name == "English");

            //save resources
            foreach (var filePath in System.IO.Directory.EnumerateFiles(_webHelper.MapPath("~/App_Data/Localization/"), "*.nutres.xml", SearchOption.TopDirectoryOnly)) {
                var localesXml = File.ReadAllText(filePath);
                var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
                localizationService.ImportResourcesFromXml(language, localesXml);
            }

        }

        protected virtual void InstallDepartments() {

            var departments = new List<Department>
           {
                new Department
                {
                    Name = "Your depatrment Name",
                    Code= "01",
                    Deleted = false,
                    ParentId =0,
                    StoreId=1,
                    DisplayOrder = 1
                },
            };

            _departmentRepository.Insert(departments);
        }

        protected virtual void InstallCustomersAndUsers(string defaultUserEmail, string defaultUserPassword) {
            var crAdministrators = new UserRole {
                Name = "Administrators",
                Active = true,
                IsSystemRole = true,
                SystemName = SystemUserRoleNames.Administrators,
            };
            _userRoleRepository.Insert(crAdministrators);

            var crRegistered = new UserRole {
                Name = "Registered",
                Active = true,
                IsSystemRole = true,
                SystemName = SystemUserRoleNames.Registered,
            };
            _userRoleRepository.Insert(crRegistered);

            var crGuests = new UserRole {
                Name = "Guests",
                Active = true,
                IsSystemRole = true,
                SystemName = SystemUserRoleNames.Guests,
            };
            _userRoleRepository.Insert(crGuests);

            //admin user
            var adminUser = new User {
                UserGuid = Guid.NewGuid(),
                Email = defaultUserEmail,
                Username = defaultUserEmail,
                Password = defaultUserPassword,
                PasswordFormat = PasswordFormat.Clear,
                PasswordSalt = "",
                Active = true,
                DepartmentId = 1,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
            };

            adminUser.UserRoles.Add(crAdministrators);
            adminUser.UserRoles.Add(crRegistered);
            _userRepository.Insert(adminUser);

        }

        protected virtual void HashDefaultCustomerPassword(string defaultUserEmail, string defaultUserPassword) {
            var customerRegistrationService = EngineContext.Current.Resolve<IUserRegistrationService>();
            customerRegistrationService.ChangePassword(new ChangePasswordRequest(defaultUserEmail, false,
                 PasswordFormat.Hashed, defaultUserPassword));
        }


        protected virtual void InstallSettings() {
            var settingService = EngineContext.Current.Resolve<ISettingService>();
            //
        }

        protected virtual void InstallScheduleTasks() {
            var tasks = new List<ScheduleTask>
            {
                new ScheduleTask
                {
                    Name = "Send emails",
                    Seconds = 60,
                    Type = "Nut.Services.Messages.QueuedMessagesSendTask, Nut.Services",
                    Enabled = true,
                    StopOnError = false,
                },
                new ScheduleTask
                {
                    Name = "Keep alive",
                    Seconds = 300,
                    Type = "Nut.Services.Common.KeepAliveTask, Nut.Services",
                    Enabled = true,
                    StopOnError = false,
                },
                new ScheduleTask
                {
                    Name = "Delete guests",
                    Seconds = 600,
                    Type = "Nut.Services.Customers.DeleteGuestsTask, Nut.Services",
                    Enabled = true,
                    StopOnError = false,
                },
                new ScheduleTask
                {
                    Name = "Clear cache",
                    Seconds = 600,
                    Type = "Nut.Services.Caching.ClearCacheTask, Nut.Services",
                    Enabled = false,
                    StopOnError = false,
                },
                new ScheduleTask
                {
                    Name = "Clear log",
                    //60 minutes
                    Seconds = 3600,
                    Type = "Nut.Services.Logging.ClearLogTask, Nut.Services",
                    Enabled = false,
                    StopOnError = false,
                },
                new ScheduleTask
                {
                    Name = "Update currency exchange rates",
                    Seconds = 900,
                    Type = "Nut.Services.Directory.UpdateExchangeRateTask, Nut.Services",
                    Enabled = true,
                    StopOnError = false,
                },
            };

            _scheduleTaskRepository.Insert(tasks);
        }

        protected virtual void InstallActivityLogTypes() {
            var activityLogTypes = new List<ActivityLogType>
                                      {
                                          //admin area activities
                                         
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "AddNewUser",
                                                  Enabled = true,
                                                  Name = "Add a new User"
                                              },
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "AddNewUserRole",
                                                  Enabled = true,
                                                  Name = "Add a new user role"
                                              },

                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "AddNewSetting",
                                                  Enabled = true,
                                                  Name = "Add a new setting"
                                              },

                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "DeleteUser",
                                                  Enabled = true,
                                                  Name = "Delete a user"
                                              },
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "DeleteUserRole",
                                                  Enabled = true,
                                                  Name = "Delete a user role"
                                              },

                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "DeleteSetting",
                                                  Enabled = true,
                                                  Name = "Delete a setting"
                                              },


                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "EditUser",
                                                  Enabled = true,
                                                  Name = "Edit a user"
                                              },
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "EditUserRole",
                                                  Enabled = true,
                                                  Name = "Edit a user role"
                                              },

                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "EditSettings",
                                                  Enabled = true,
                                                  Name = "Edit setting(s)"
                                              },
                                          
                                              //public store activities
                                        
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "PublicStore.Login",
                                                  Enabled = false,
                                                  Name = "Public store. Login"
                                              },
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "PublicStore.Logout",
                                                  Enabled = false,
                                                  Name = "Public store. Logout"
                                              },

                                      };
            _activityLogTypeRepository.Insert(activityLogTypes);
        }
        #endregion

        #region Methods

        public virtual void InstallData(string defaultUserEmail,
            string defaultUserPassword, bool installSampleData = true) {
            InstallStores();
            InstallLanguages();
            InstallDepartments();
            InstallCustomersAndUsers(defaultUserEmail, defaultUserPassword);
            InstallSettings();
            InstallLocaleResources();
            HashDefaultCustomerPassword(defaultUserEmail, defaultUserPassword);
            InstallScheduleTasks();

        }

        #endregion
    }
}