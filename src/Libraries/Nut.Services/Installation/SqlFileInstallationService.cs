using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Nut.Core;
using Nut.Core.Data;
using Nut.Core.Domain.Users;
using Nut.Core.Domain.Localization;
using Nut.Core.Domain.Stores;
using Nut.Core.Infrastructure;
using Nut.Data;
using Nut.Services.Users;
using Nut.Services.Localization;

namespace Nut.Services.Installation
{
    public partial class SqlFileInstallationService : IInstallationService
    {
        #region Fields

        private readonly IRepository<Language> _languageRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Store> _storeRepository;
        private readonly IDbContext _dbContext;
        private readonly IWebHelper _webHelper;

        #endregion

        #region Ctor

        public SqlFileInstallationService(IRepository<Language> languageRepository,
            IRepository<User> userRepository, 
            IRepository<Store> storeRepository,
            IDbContext dbContext,
            IWebHelper webHelper)
        {
            this._languageRepository = languageRepository;
            this._userRepository = userRepository;
            this._storeRepository = storeRepository;
            this._dbContext = dbContext;
            this._webHelper = webHelper;
        }

        #endregion

        #region Utilities

        protected virtual void InstallLocaleResources()
        {
            //'English' language
            var language = _languageRepository.Table.Single(l => l.Name == "English");

            //save resources
            foreach (var filePath in System.IO.Directory.EnumerateFiles(_webHelper.MapPath("~/App_Data/Localization/"), "*.nopres.xml", SearchOption.TopDirectoryOnly))
            {
                var localesXml = File.ReadAllText(filePath);
                var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
                localizationService.ImportResourcesFromXml(language, localesXml);
            }

        }

        protected virtual void UpdateDefaultCustomer(string defaultUserEmail, string defaultUserPassword)
        {
            var adminUser = _userRepository.Table.Single(x => !x.IsSystemAccount);
            if (adminUser == null)
                throw new Exception("Admin user cannot be loaded");

            adminUser.UserGuid = Guid.NewGuid();
            adminUser.Email = defaultUserEmail;
            adminUser.Username = defaultUserEmail;
            _userRepository.Update(adminUser);

            var customerRegistrationService = EngineContext.Current.Resolve<IUserRegistrationService>();
            customerRegistrationService.ChangePassword(new ChangePasswordRequest(defaultUserEmail, false,
                 PasswordFormat.Hashed, defaultUserPassword));
        }

        protected virtual void UpdateDefaultStoreUrl()
        {
            var store = _storeRepository.Table.FirstOrDefault();
            if (store == null)
                throw new Exception("Default store cannot be loaded");

            store.Url = _webHelper.GetStoreLocation(false);
            _storeRepository.Update(store);
        }

        protected virtual void ExecuteSqlFile(string path)
        {
            var statements = new List<string>();

            using (var stream = File.OpenRead(path))
            using (var reader = new StreamReader(stream))
            {
                string statement;
                while ((statement = ReadNextStatementFromStream(reader)) != null)
                    statements.Add(statement);
            }

            foreach (string stmt in statements)
                _dbContext.ExecuteSqlCommand(stmt);
        }

        protected virtual string ReadNextStatementFromStream(StreamReader reader)
        {
            var sb = new StringBuilder();
            while (true)
            {
                var lineOfText = reader.ReadLine();
                if (lineOfText == null)
                {
                    if (sb.Length > 0)
                        return sb.ToString();
                    
                    return null;
                }

                if (lineOfText.TrimEnd().ToUpper() == "GO")
                    break;

                sb.Append(lineOfText + Environment.NewLine);
            }

            return sb.ToString();
        }

        #endregion

        #region Methods

        public virtual void InstallData(string defaultUserEmail,
            string defaultUserPassword, bool installSampleData = true)
        {
            ExecuteSqlFile(_webHelper.MapPath("~/App_Data/Install/create_required_data.sql"));
            InstallLocaleResources();
            UpdateDefaultCustomer(defaultUserEmail, defaultUserPassword);
            UpdateDefaultStoreUrl();

            if (installSampleData)
            {
                ExecuteSqlFile(_webHelper.MapPath("~/App_Data/Install/create_sample_data.sql"));
            }
        }

        #endregion
    }
}