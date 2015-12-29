using System;
using System.Linq;
using System.Web;
using Nut.Core;
using Nut.Core.Domain.Users;
using Nut.Core.Domain.Localization;
using Nut.Core.Fakes;
using Nut.Services.Authentication;
using Nut.Services.Common;
using Nut.Services.Users;
using Nut.Services.Localization;
using Nut.Services.Stores;
using Nut.Web.Framework.Localization;

namespace Nut.Web.Framework
{
    /// <summary>
    /// Work context for web application
    /// </summary>
    public partial class WebWorkContext : IWorkContext
    {
        #region Const

        private const string CustomerCookieName = "Nut.customer";

        #endregion

        #region Fields

        private readonly HttpContextBase _httpContext;
        private readonly IUserService _userService;
        private readonly IStoreContext _storeContext;
        private readonly IAuthenticationService _authenticationService;
        private readonly ILanguageService _languageService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly LocalizationSettings _localizationSettings;
        private readonly IStoreMappingService _storeMappingService;

        private User _cachedUser;
        private Language _cachedLanguage;

        #endregion

        #region Ctor

        public WebWorkContext(HttpContextBase httpContext,
            IUserService userService,
            IStoreContext storeContext,
            IAuthenticationService authenticationService,
            ILanguageService languageService,
            IGenericAttributeService genericAttributeService,
            LocalizationSettings localizationSettings,
            IStoreMappingService storeMappingService)
        {
            this._httpContext = httpContext;
            this._userService = userService;
            this._storeContext = storeContext;
            this._authenticationService = authenticationService;
            this._languageService = languageService;
            this._genericAttributeService = genericAttributeService;
            this._localizationSettings = localizationSettings;
            this._storeMappingService = storeMappingService;
        }

        #endregion

        #region Utilities

        protected virtual HttpCookie GetCustomerCookie()
        {
            if (_httpContext == null || _httpContext.Request == null)
                return null;

            return _httpContext.Request.Cookies[CustomerCookieName];
        }

        protected virtual void SetCustomerCookie(Guid customerGuid)
        {
            if (_httpContext != null && _httpContext.Response != null)
            {
                var cookie = new HttpCookie(CustomerCookieName);
                cookie.HttpOnly = true;
                cookie.Value = customerGuid.ToString();
                if (customerGuid == Guid.Empty)
                {
                    cookie.Expires = DateTime.Now.AddMonths(-1);
                }
                else
                {
                    int cookieExpires = 24*365; //TODO make configurable
                    cookie.Expires = DateTime.Now.AddHours(cookieExpires);
                }

                _httpContext.Response.Cookies.Remove(CustomerCookieName);
                _httpContext.Response.Cookies.Add(cookie);
            }
        }

        protected virtual Language GetLanguageFromUrl()
        {
            if (_httpContext == null || _httpContext.Request == null)
                return null;

            string virtualPath = _httpContext.Request.AppRelativeCurrentExecutionFilePath;
            string applicationPath = _httpContext.Request.ApplicationPath;
            if (!virtualPath.IsLocalizedUrl(applicationPath, false))
                return null;

            var seoCode = virtualPath.GetLanguageSeoCodeFromUrl(applicationPath, false);
            if (String.IsNullOrEmpty(seoCode))
                return null;

            var language = _languageService
                .GetAllLanguages()
                .FirstOrDefault(l => seoCode.Equals(l.UniqueSeoCode, StringComparison.InvariantCultureIgnoreCase));
            if (language != null && language.Published && _storeMappingService.Authorize(language))
            {
                return language;
            }

            return null;
        }

        protected virtual Language GetLanguageFromBrowserSettings()
        {
            if (_httpContext == null ||
                _httpContext.Request == null ||
                _httpContext.Request.UserLanguages == null)
                return null;

            var userLanguage = _httpContext.Request.UserLanguages.FirstOrDefault();
            if (String.IsNullOrEmpty(userLanguage))
                return null;

            var language = _languageService
                .GetAllLanguages()
                .FirstOrDefault(l => userLanguage.Equals(l.LanguageCulture, StringComparison.InvariantCultureIgnoreCase));
            if (language != null && language.Published && _storeMappingService.Authorize(language))
            {
                return language;
            }

            return null;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the current customer
        /// </summary>
        public virtual User CurrentUser {
            get
            {
                if (_cachedUser != null)
                    return _cachedUser;

                User user = null;
                if (_httpContext == null || _httpContext is FakeHttpContext)
                {
                    //check whether request is made by a background task
                    //in this case return built-in customer record for background task
                    
                }

                if (user == null || user.Deleted || !user.Active)
                    user = _authenticationService.GetAuthenticatedUser();

                //load guest customer
                if (user == null || user.Deleted || !user.Active)
                {
                    var customerCookie = GetCustomerCookie();
                    if (customerCookie != null && !String.IsNullOrEmpty(customerCookie.Value))
                    {
                        Guid customerGuid;
                        if (Guid.TryParse(customerCookie.Value, out customerGuid))
                        {
                            var customerByCookie = _userService.GetUserByGuid(customerGuid);
                            if (customerByCookie != null &&
                                //this customer (from cookie) should not be registered
                                !customerByCookie.IsRegistered())
                                user = customerByCookie;
                        }
                    }
                }

                //create guest if not exists
                if (user == null || user.Deleted || !user.Active)
                {
                    user = _userService.InsertGuestUser();
                }


                //validation
                if (!user.Deleted && user.Active)
                {
                    SetCustomerCookie(user.UserGuid);
                    _cachedUser = user;
                }

                return _cachedUser;
            }
            set
            {
                SetCustomerCookie(value.UserGuid);
                _cachedUser = value;
            }
        }


        /// <summary>
        /// Get or set current user working language
        /// </summary>
        public virtual Language WorkingLanguage
        {
            get
            {
                if (_cachedLanguage != null)
                    return _cachedLanguage;
                
                Language detectedLanguage = null;
                if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
                {
                    //get language from URL
                    detectedLanguage = GetLanguageFromUrl();
                }
                if (detectedLanguage != null)
                {
                    //the language is detected. now we need to save it
                    if (this.CurrentUser.GetAttribute<int>(SystemUserAttributeNames.LanguageId,
                        _genericAttributeService, _storeContext.CurrentStore.Id) != detectedLanguage.Id)
                    {
                        _genericAttributeService.SaveAttribute(this.CurrentUser, SystemUserAttributeNames.LanguageId,
                            detectedLanguage.Id, _storeContext.CurrentStore.Id);
                    }
                }

                var allLanguages = _languageService.GetAllLanguages(storeId: _storeContext.CurrentStore.Id);
                //find current customer language
                var languageId = this.CurrentUser.GetAttribute<int>(SystemUserAttributeNames.LanguageId,
                    _genericAttributeService, _storeContext.CurrentStore.Id);
                var language = allLanguages.FirstOrDefault(x => x.Id == languageId);
                if (language == null)
                {
                    //it not specified, then return the first (filtered by current store) found one
                    language = allLanguages.FirstOrDefault();
                }
                if (language == null)
                {
                    //it not specified, then return the first found one
                    language = _languageService.GetAllLanguages().FirstOrDefault();
                }

                //cache
                _cachedLanguage = language;
                return _cachedLanguage;
            }
            set
            {
                var languageId = value != null ? value.Id : 0;
                _genericAttributeService.SaveAttribute(this.CurrentUser,
                    SystemUserAttributeNames.LanguageId,
                    languageId, _storeContext.CurrentStore.Id);

                //reset cache
                _cachedLanguage = null;
            }
        }

        /// <summary>
        /// Get or set value indicating whether we're in admin area
        /// </summary>
        public virtual bool IsAdmin { get; set; }

        #endregion
    }
}
