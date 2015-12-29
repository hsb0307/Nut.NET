using System;
using System.Linq;
using Nut.Core;
using Nut.Services.Common;
using Nut.Core.Domain.Users;

namespace Nut.Web.Framework.Themes
{
    /// <summary>
    /// Theme context
    /// </summary>
    public partial class ThemeContext : IThemeContext
    {
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IThemeProvider _themeProvider;
        private readonly IGenericAttributeService _genericAttributeService;

        private bool _themeIsCached;
        private string _cachedThemeName;

        public ThemeContext(IWorkContext workContext,
            IStoreContext storeContext,
            IThemeProvider themeProvider,
            IGenericAttributeService genericAttributeService)
        {
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._themeProvider = themeProvider;
            this._genericAttributeService = genericAttributeService;
        }

        /// <summary>
        /// Get or set current theme system name
        /// </summary>
        public string WorkingThemeName
        {
            get
            {
                if (_themeIsCached)
                    return _cachedThemeName;

                string theme = "";

                //default store theme
                //if (string.IsNullOrEmpty(theme))
                //    theme = _storeInformationSettings.DefaultStoreTheme;

                //ensure that theme exists
                if (!_themeProvider.ThemeConfigurationExists(theme))
                {
                    var themeInstance = _themeProvider.GetThemeConfigurations()
                        .FirstOrDefault();
                    if (themeInstance == null)
                        throw new Exception("No theme could be loaded");
                    theme = themeInstance.ThemeName;
                }
                
                //cache theme
                this._cachedThemeName = theme;
                this._themeIsCached = true;
                return theme;
            }
            set
            {

                if (_workContext.CurrentUser == null)
                    return;

                _genericAttributeService.SaveAttribute(_workContext.CurrentUser, SystemUserAttributeNames.WorkingThemeName, value, _storeContext.CurrentStore.Id);

                //clear cache
                this._themeIsCached = false;
            }
        }
    }
}
