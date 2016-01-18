using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Web.Hosting;
using System.Web.Mvc;
using Nut.Core;
using Nut.Core.Data;
using Nut.Core.Infrastructure;
using Nut.Core.Plugins;
using Nut.Services.Installation;
using Nut.Services.Security;
using Nut.Web.Framework.Security;
using Nut.Web.Infrastructure.Installation;
using Nut.Web.Models.Install;
using MySql.Data.MySqlClient;

namespace Nut.Web.Controllers {
    public partial class InstallController : BasePublicController {
        #region Fields

        private readonly IInstallationLocalizationService _locService;

        #endregion

        #region Ctor

        public InstallController(IInstallationLocalizationService locService) {
            this._locService = locService;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// A value indicating whether we use MARS (Multiple Active Result Sets)
        /// </summary>
        protected bool UseMars {
            get { return false; }
        }

        /// <summary>
        /// Checks if the specified database exists, returns true if database exists
        /// </summary>
        /// <param name="connectionString">Connection string</param>
        /// <returns>Returns true if the database exists.</returns>
        [NonAction]
        protected bool SqlServerDatabaseExists(string connectionString) {
            try {
                //just try to connect
                using (var conn = new SqlConnection(connectionString)) {
                    conn.Open();
                }
                return true;
            } catch {
                return false;
            }
        }

        /// <summary>
        /// Creates a database on the server.
        /// </summary>
        /// <param name="connectionString">Connection string</param>
        /// <param name="collation">Server collation; the default one will be used if not specified</param>
        /// <returns>Error</returns>
        [NonAction]
        protected string CreateDatabase(string connectionString, string collation) {
            try {
                //parse database name
                var builder = new SqlConnectionStringBuilder(connectionString);
                var databaseName = builder.InitialCatalog;
                //now create connection string to 'master' dabatase. It always exists.
                builder.InitialCatalog = "master";
                var masterCatalogConnectionString = builder.ToString();
                string query = string.Format("CREATE DATABASE [{0}]", databaseName);
                if (!String.IsNullOrWhiteSpace(collation))
                    query = string.Format("{0} COLLATE {1}", query, collation);
                using (var conn = new SqlConnection(masterCatalogConnectionString)) {
                    conn.Open();
                    using (var command = new SqlCommand(query, conn)) {
                        command.ExecuteNonQuery();
                    }
                }

                return string.Empty;
            } catch (Exception ex) {
                return string.Format(_locService.GetResource("DatabaseCreationError"), ex.Message);
            }
        }



        /// <summary>
        /// Mies the SQL database exists.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns></returns>
        [NonAction]
        private bool mySqlDatabaseExists(string connectionString) {
            try {
                //just try to connect
                using (var conn = new MySqlConnection(connectionString)) {
                    conn.Open();
                }
                return true;
            } catch {
                return false;
            }
        }
        /// <summary>
        /// Creates my SQL database.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns></returns>
        [NonAction]
        private string createMySqlDatabase(string connectionString) {
            try {
                //parse database name
                var builder = new MySqlConnectionStringBuilder(connectionString);
                var databaseName = builder.Database;
                //now create connection string to 'master' dabatase. It always exists.
                builder.Database = string.Empty; // = "master";
                var masterCatalogConnectionString = builder.ToString();
                string query = string.Format("CREATE DATABASE {0} COLLATE utf8_unicode_ci", databaseName);

                using (var conn = new MySqlConnection(masterCatalogConnectionString)) {
                    conn.Open();
                    using (var command = new MySqlCommand(query, conn)) {
                        command.ExecuteNonQuery();
                    }
                }

                return string.Empty;
            } catch (Exception ex) {
                return string.Format("An error occured when creating database: {0}", ex.Message);
            }
        }

        #endregion

        #region Methods

        public ActionResult Index() {
            if (DataSettingsHelper.DatabaseIsInstalled())
                return RedirectToRoute("HomePage");

            //set page timeout to 5 minutes
            this.Server.ScriptTimeout = 300;


            var model = new InstallModel {
                AdminUsername = "admin",
                InstallSampleData = false,
                DataProvider = "sqlserver",

                SqlServerConnectionString = "",
                SqlServerCreateDatabase = false,
                UseCustomCollation = false,
                Collation = "SQL_Latin1_General_CP1_CI_AS",

                MySqlConnectionString = "",
                MySqlCreateDatabase = false,
                //fast installation service does not support SQL compact
                DisableSqlCompact = !String.IsNullOrEmpty(ConfigurationManager.AppSettings["UseFastInstallationService"]) &&
                    Convert.ToBoolean(ConfigurationManager.AppSettings["UseFastInstallationService"]),
                DisableSampleDataOption = !String.IsNullOrEmpty(ConfigurationManager.AppSettings["DisableSampleDataDuringInstallation"]) &&
                    Convert.ToBoolean(ConfigurationManager.AppSettings["DisableSampleDataDuringInstallation"]),


            };
            foreach (var lang in _locService.GetAvailableLanguages()) {
                model.AvailableLanguages.Add(new SelectListItem {
                    Value = Url.Action("ChangeLanguage", "Install", new { language = lang.Code }),
                    Text = lang.Name,
                    Selected = _locService.GetCurrentLanguage().Code == lang.Code,
                });
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(InstallModel model) {
            if (DataSettingsHelper.DatabaseIsInstalled())
                return RedirectToRoute("HomePage");

            //set page timeout to 5 minutes
            this.Server.ScriptTimeout = 300;

            if (model.SqlServerConnectionString != null)
                model.SqlServerConnectionString = model.SqlServerConnectionString.Trim();

            if (model.MySqlConnectionString != null)
                model.MySqlConnectionString = model.MySqlConnectionString.Trim();

            //prepare language list
            foreach (var lang in _locService.GetAvailableLanguages()) {
                model.AvailableLanguages.Add(new SelectListItem {
                    Value = Url.Action("ChangeLanguage", "Install", new { language = lang.Code }),
                    Text = lang.Name,
                    Selected = _locService.GetCurrentLanguage().Code == lang.Code,
                });
            }
            model.DisableSqlCompact = !String.IsNullOrEmpty(ConfigurationManager.AppSettings["UseFastInstallationService"]) &&
                Convert.ToBoolean(ConfigurationManager.AppSettings["UseFastInstallationService"]);
            model.DisableSampleDataOption = !String.IsNullOrEmpty(ConfigurationManager.AppSettings["DisableSampleDataDuringInstallation"]) &&
                Convert.ToBoolean(ConfigurationManager.AppSettings["DisableSampleDataDuringInstallation"]);



            if (model.DataProvider.Equals("sqlserver", StringComparison.InvariantCultureIgnoreCase)) {
                //raw connection string
                if (string.IsNullOrEmpty(model.SqlServerConnectionString))
                    ModelState.AddModelError("", _locService.GetResource("ConnectionStringRequired"));
                else {
                    try {
                        //try to create connection string

                        new SqlConnectionStringBuilder(model.SqlServerConnectionString);

                    } catch {
                        ModelState.AddModelError("", _locService.GetResource("ConnectionStringWrongFormat"));
                    }
                }
            }

            if (model.DataProvider.Equals("mysql", StringComparison.InvariantCultureIgnoreCase)) {
                //raw connection string
                if (string.IsNullOrEmpty(model.MySqlConnectionString))
                    ModelState.AddModelError("", _locService.GetResource("ConnectionStringRequired"));
                else {
                    try {
                        //try to create connection string

                        new MySqlConnectionStringBuilder(model.MySqlConnectionString);

                    } catch {
                        ModelState.AddModelError("", _locService.GetResource("ConnectionStringWrongFormat"));
                    }
                }
            }


            //Consider granting access rights to the resource to the ASP.NET request identity. 
            //ASP.NET has a base process identity 
            //(typically {MACHINE}\ASPNET on IIS 5 or Network Service on IIS 6 and IIS 7, 
            //and the configured application pool identity on IIS 7.5) that is used if the application is not impersonating.
            //If the application is impersonating via <identity impersonate="true"/>, 
            //the identity will be the anonymous user (typically IUSR_MACHINENAME) or the authenticated request user.
            var webHelper = EngineContext.Current.Resolve<IWebHelper>();
            //validate permissions
            var dirsToCheck = FilePermissionHelper.GetDirectoriesWrite(webHelper);
            foreach (string dir in dirsToCheck)
                if (!FilePermissionHelper.CheckPermissions(dir, false, true, true, false))
                    ModelState.AddModelError("", string.Format(_locService.GetResource("ConfigureDirectoryPermissions"), WindowsIdentity.GetCurrent().Name, dir));

            var filesToCheck = FilePermissionHelper.GetFilesWrite(webHelper);
            foreach (string file in filesToCheck)
                if (!FilePermissionHelper.CheckPermissions(file, false, true, true, true))
                    ModelState.AddModelError("", string.Format(_locService.GetResource("ConfigureFilePermissions"), WindowsIdentity.GetCurrent().Name, file));

            if (ModelState.IsValid) {
                var settingsManager = new DataSettingsManager();
                try {
                    string connectionString;
                    if (model.DataProvider.Equals("sqlserver", StringComparison.InvariantCultureIgnoreCase)) {
                        //SQL Server
                        //we know that MARS option is required when using Entity Framework
                        //let's ensure that it's specified
                        var sqlCsb = new SqlConnectionStringBuilder(model.SqlServerConnectionString);
                        if (this.UseMars) {
                            sqlCsb.MultipleActiveResultSets = true;
                        }
                        connectionString = sqlCsb.ToString();


                        if (model.SqlServerCreateDatabase) {
                            if (!SqlServerDatabaseExists(connectionString)) {
                                //create database
                                var collation = model.UseCustomCollation ? model.Collation : "";
                                var errorCreatingDatabase = CreateDatabase(connectionString, collation);
                                if (!String.IsNullOrEmpty(errorCreatingDatabase))
                                    throw new Exception(errorCreatingDatabase);

                                //Database cannot be created sometimes. Weird! Seems to be Entity Framework issue
                                //that's just wait 3 seconds
                                Thread.Sleep(3000);
                            }
                        } else {
                            //check whether database exists
                            if (!SqlServerDatabaseExists(connectionString))
                                throw new Exception(_locService.GetResource("DatabaseNotExists"));
                        }
                    } else if (model.DataProvider.Equals("mysql", StringComparison.InvariantCultureIgnoreCase)) {
                        //SQL Server
                        //we know that MARS option is required when using Entity Framework
                        //let's ensure that it's specified
                        var sqlCsb = new MySqlConnectionStringBuilder(model.MySqlConnectionString);
                        if (this.UseMars) {
                            sqlCsb.AllowUserVariables = true;
                        }
                        connectionString = sqlCsb.ToString();

                        if (model.MySqlCreateDatabase) {
                            if (!mySqlDatabaseExists(connectionString)) {
                                //create database
                                var errorCreatingDatabase = CreateDatabase(connectionString, "");
                                if (!String.IsNullOrEmpty(errorCreatingDatabase))
                                    throw new Exception(errorCreatingDatabase);

                                //Database cannot be created sometimes. Weird! Seems to be Entity Framework issue
                                //that's just wait 3 seconds
                                Thread.Sleep(3000);
                            }
                        } else {
                            //check whether database exists
                            if (!mySqlDatabaseExists(connectionString))
                                throw new Exception(_locService.GetResource("DatabaseNotExists"));
                        }
                    } else {
                        //SQL CE
                        string databaseFileName = "Nut.Db.sdf";
                        string databasePath = @"|DataDirectory|\" + databaseFileName;
                        connectionString = "Data Source=" + databasePath + ";Persist Security Info=False";

                        //drop database if exists
                        string databaseFullPath = HostingEnvironment.MapPath("~/App_Data/") + databaseFileName;
                        if (System.IO.File.Exists(databaseFullPath)) {
                            System.IO.File.Delete(databaseFullPath);
                        }
                    }

                    //save settings
                    var dataProvider = model.DataProvider;
                    var settings = new DataSettings {
                        DataProvider = dataProvider,
                        DataConnectionString = connectionString
                    };
                    settingsManager.SaveSettings(settings);

                    //init data provider
                    var dataProviderInstance = EngineContext.Current.Resolve<BaseDataProviderManager>().LoadDataProvider();
                    dataProviderInstance.InitDatabase();


                    //now resolve installation service
                    var installationService = EngineContext.Current.Resolve<IInstallationService>();
                    installationService.InstallData(model.AdminUsername, model.AdminPassword, model.InstallSampleData);

                    //reset cache
                    DataSettingsHelper.ResetCache();

                    //install plugins
                    PluginManager.MarkAllPluginsAsUninstalled();
                    var pluginFinder = EngineContext.Current.Resolve<IPluginFinder>();
                    var plugins = pluginFinder.GetPlugins<IPlugin>(LoadPluginsMode.All)
                        .ToList()
                        .OrderBy(x => x.PluginDescriptor.Group)
                        .ThenBy(x => x.PluginDescriptor.DisplayOrder)
                        .ToList();
                    var pluginsIgnoredDuringInstallation = String.IsNullOrEmpty(ConfigurationManager.AppSettings["PluginsIgnoredDuringInstallation"]) ?
                        new List<string>() :
                        ConfigurationManager.AppSettings["PluginsIgnoredDuringInstallation"]
                            .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(x => x.Trim())
                            .ToList();
                    foreach (var plugin in plugins) {
                        if (pluginsIgnoredDuringInstallation.Contains(plugin.PluginDescriptor.SystemName))
                            continue;
                        plugin.Install();
                    }

                    //register default permissions
                    //var permissionProviders = EngineContext.Current.Resolve<ITypeFinder>().FindClassesOfType<IPermissionProvider>();
                    var permissionProviders = new List<Type>();
                    permissionProviders.Add(typeof(StandardPermissionProvider));
                    foreach (var providerType in permissionProviders) {
                        dynamic provider = Activator.CreateInstance(providerType);
                        EngineContext.Current.Resolve<IPermissionService>().InstallPermissions(provider);
                    }

                    //restart application
                    webHelper.RestartAppDomain();

                    //Redirect to home page
                    return RedirectToRoute("HomePage");
                } catch (Exception exception) {
                    //reset cache
                    DataSettingsHelper.ResetCache();

                    //clear provider settings if something got wrong
                    settingsManager.SaveSettings(new DataSettings {
                        DataProvider = null,
                        DataConnectionString = null
                    });

                    ModelState.AddModelError("", string.Format(_locService.GetResource("SetupFailed"), exception.Message));
                }
            }
            return View(model);
        }

        public ActionResult ChangeLanguage(string language) {
            if (DataSettingsHelper.DatabaseIsInstalled())
                return RedirectToRoute("HomePage");

            _locService.SaveCurrentLanguage(language);

            //Reload the page
            return RedirectToAction("Index", "Install");
        }

        public ActionResult RestartInstall() {
            if (DataSettingsHelper.DatabaseIsInstalled())
                return RedirectToRoute("HomePage");

            //restart application
            var webHelper = EngineContext.Current.Resolve<IWebHelper>();
            webHelper.RestartAppDomain();

            //Redirect to home page
            return RedirectToRoute("HomePage");
        }

        #endregion
    }
}
