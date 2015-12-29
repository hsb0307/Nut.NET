using Nop.Admin.Models.Plugins;
using Nut.Admin.Extensions;
using Nut.Core;
using Nut.Core.Plugins;
using Nut.Services.Configuration;
using Nut.Services.Localization;
using Nut.Services.Security;
using Nut.Services.Stores;
using Nut.Web.Framework;
using Nut.Web.Framework.Controllers;
using Nut.Web.Framework.Kendoui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Nut.Admin.Controllers {
    public class PluginController : BaseAdminController {
        #region Fields

        private readonly IPluginFinder _pluginFinder;
        private readonly ILocalizationService _localizationService;
        private readonly IWebHelper _webHelper;
        private readonly IPermissionService _permissionService;
        private readonly ILanguageService _languageService;
        private readonly ISettingService _settingService;
        private readonly IStoreService _storeService;

        #endregion

        #region Constructors

        public PluginController(IPluginFinder pluginFinder,
            ILocalizationService localizationService,
            IWebHelper webHelper,
            IPermissionService permissionService,
            ILanguageService languageService,
            ISettingService settingService,
            IStoreService storeService) {
            this._pluginFinder = pluginFinder;
            this._localizationService = localizationService;
            this._webHelper = webHelper;
            this._permissionService = permissionService;
            this._languageService = languageService;
            this._settingService = settingService;
            this._storeService = storeService;
        }

        #endregion

        #region Utilities

        [NonAction]
        protected virtual PluginModel PreparePluginModel(PluginDescriptor pluginDescriptor,
            bool prepareLocales = true, bool prepareStores = true) {
            var pluginModel = pluginDescriptor.ToModel();
            //logo
            pluginModel.LogoUrl = pluginDescriptor.GetLogoUrl(_webHelper);

            if (prepareLocales) {
                //locales
                AddLocales(_languageService, pluginModel.Locales, (locale, languageId) => {
                    locale.FriendlyName = pluginDescriptor.Instance().GetLocalizedFriendlyName(_localizationService, languageId, false);
                });
            }
            if (prepareStores) {
                //stores
                pluginModel.AvailableStores = _storeService
                    .GetAllStores()
                    .Select(s => s.ToModel())
                    .ToList();
                pluginModel.SelectedStoreIds = pluginDescriptor.LimitedToStores.ToArray();
                pluginModel.LimitedToStores = pluginDescriptor.LimitedToStores.Count > 0;
            }


            //configuration URL

            return pluginModel;
        }

        #endregion

        #region Methods

        public ActionResult Index() {
            return RedirectToAction("List");
        }

        public ActionResult List() {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public ActionResult ListSelect(DataSourceRequest command) {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            var pluginDescriptors = _pluginFinder.GetPluginDescriptors(LoadPluginsMode.All).ToList();
            var gridModel = new DataSourceResult {
                Data = pluginDescriptors.Select(x => PreparePluginModel(x, false, false))
                .OrderBy(x => x.Group)
                .ToList(),
                Total = pluginDescriptors.Count()
            };

            return Json(gridModel);
        }

        public ActionResult Install(string systemName) {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            try {
                var pluginDescriptor = _pluginFinder.GetPluginDescriptors(LoadPluginsMode.All)
                    .FirstOrDefault(x => x.SystemName.Equals(systemName, StringComparison.InvariantCultureIgnoreCase));
                if (pluginDescriptor == null)
                    //No plugin found with the specified id
                    return RedirectToAction("List");

                //check whether plugin is not installed
                if (pluginDescriptor.Installed)
                    return RedirectToAction("List");

                //install plugin
                pluginDescriptor.Instance().Install();
                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Plugins.Installed"));

                //restart application
                _webHelper.RestartAppDomain();
            } catch (Exception exc) {
                ErrorNotification(exc);
            }

            return RedirectToAction("List");
        }
        public ActionResult Uninstall(string systemName) {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            try {
                var pluginDescriptor = _pluginFinder.GetPluginDescriptors(LoadPluginsMode.All)
                    .FirstOrDefault(x => x.SystemName.Equals(systemName, StringComparison.InvariantCultureIgnoreCase));
                if (pluginDescriptor == null)
                    //No plugin found with the specified id
                    return RedirectToAction("List");

                //check whether plugin is installed
                if (!pluginDescriptor.Installed)
                    return RedirectToAction("List");

                //uninstall plugin
                pluginDescriptor.Instance().Uninstall();
                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Plugins.Uninstalled"));

                //restart application
                _webHelper.RestartAppDomain();
            } catch (Exception exc) {
                ErrorNotification(exc);
            }

            return RedirectToAction("List");
        }

        public ActionResult ReloadList() {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            //restart application
            _webHelper.RestartAppDomain();
            return RedirectToAction("List");
        }


        //edit
        public ActionResult EditPopup(string systemName) {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            var pluginDescriptor = _pluginFinder.GetPluginDescriptorBySystemName(systemName, LoadPluginsMode.All);
            if (pluginDescriptor == null)
                //No plugin found with the specified id
                return RedirectToAction("List");

            var model = PreparePluginModel(pluginDescriptor);

            return View(model);
        }
        [HttpPost]
        public ActionResult EditPopup(string btnId, string formId, PluginModel model) {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            var pluginDescriptor = _pluginFinder.GetPluginDescriptorBySystemName(model.SystemName, LoadPluginsMode.All);
            if (pluginDescriptor == null)
                //No plugin found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid) {
                //we allow editing of 'friendly name', 'display order', store mappings
                pluginDescriptor.FriendlyName = model.FriendlyName;
                pluginDescriptor.DisplayOrder = model.DisplayOrder;
                pluginDescriptor.LimitedToStores.Clear();
                if (model.LimitedToStores && model.SelectedStoreIds != null) {
                    pluginDescriptor.LimitedToStores = model.SelectedStoreIds.ToList();
                }
                PluginFileParser.SavePluginDescriptionFile(pluginDescriptor);
                //reset plugin cache
                _pluginFinder.ReloadPlugins();
                //locales
                foreach (var localized in model.Locales) {
                    pluginDescriptor.Instance().SaveLocalizedFriendlyName(_localizationService, localized.LanguageId, localized.FriendlyName);
                }
                //enabled/disabled
                if (pluginDescriptor.Installed) {

                }

                ViewBag.RefreshPage = true;
                ViewBag.btnId = btnId;
                ViewBag.formId = formId;
                return View(model);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion
    }
}