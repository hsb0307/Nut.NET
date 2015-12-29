using System;
using System.Collections.Generic;
using System.Linq;
using Nut.Core.Caching;
using Nut.Core.Data;
using Nut.Core.Domain.Localization;
using Nut.Services.Configuration;
using Nut.Services.Stores;

namespace Nut.Services.Localization {
    /// <summary>
    /// Language service
    /// </summary>
    public partial class LanguageService : ILanguageService {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// </remarks>
        private const string LANGUAGES_BY_ID_KEY = "Nop.language.id-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>
        private const string LANGUAGES_ALL_KEY = "Nop.language.all-{0}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string LANGUAGES_PATTERN_KEY = "Nop.language.";

        #endregion

        #region Fields

        private readonly IRepository<Language> _languageRepository;
        private readonly IStoreMappingService _storeMappingService;
        private readonly ICacheManager _cacheManager;
        private readonly ISettingService _settingService;
        private readonly LocalizationSettings _localizationSettings;
        private readonly ISignals _signals;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="languageRepository">Language repository</param>
        /// <param name="storeMappingService">Store mapping service</param>
        /// <param name="settingService">Setting service</param>
        /// <param name="localizationSettings">Localization settings</param>
        /// <param name="eventPublisher">Event published</param>
        public LanguageService(ICacheManager cacheManager,
            IRepository<Language> languageRepository,
            IStoreMappingService storeMappingService,
            ISettingService settingService,
            LocalizationSettings localizationSettings,
            ISignals signals) {
            this._cacheManager = cacheManager;
            this._languageRepository = languageRepository;
            this._storeMappingService = storeMappingService;
            this._settingService = settingService;
            this._localizationSettings = localizationSettings;
            this._signals = signals;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a language
        /// </summary>
        /// <param name="language">Language</param>
        public virtual void DeleteLanguage(Language language) {
            if (language == null)
                throw new ArgumentNullException("language");

            //update default admin area language (if required)
            if (_localizationSettings.DefaultAdminLanguageId == language.Id) {
                foreach (var activeLanguage in GetAllLanguages()) {
                    if (activeLanguage.Id != language.Id) {
                        _localizationSettings.DefaultAdminLanguageId = activeLanguage.Id;
                        _settingService.SaveSetting(_localizationSettings);
                        break;
                    }
                }
            }

            _languageRepository.Delete(language);

            //cache
            _signals.Trigger(LANGUAGES_PATTERN_KEY);
        }

        /// <summary>
        /// Gets all languages
        /// </summary>
        /// <param name="storeId">Load records allowed only in a specified store; pass 0 to load all records</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Languages</returns>
        public virtual IList<Language> GetAllLanguages(bool showHidden = false, int storeId = 0) {
            string key = string.Format(LANGUAGES_ALL_KEY, showHidden);
            var languages = _cacheManager.Get(key, ctx => {
                ctx.Monitor(_signals.When(LANGUAGES_PATTERN_KEY));

                var query = _languageRepository.Table;
                if (!showHidden)
                    query = query.Where(l => l.Published);
                query = query.OrderBy(l => l.DisplayOrder);
                return query.ToList();
            });

            //store mapping
            if (storeId > 0) {
                languages = languages
                    .Where(l => _storeMappingService.Authorize(l, storeId))
                    .ToList();
            }
            return languages;
        }

        /// <summary>
        /// Gets a language
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Language</returns>
        public virtual Language GetLanguageById(int languageId) {
            if (languageId == 0)
                return null;

            string key = string.Format(LANGUAGES_BY_ID_KEY, languageId);
            return _cacheManager.Get(key, ctx => {
                ctx.Monitor(_signals.When(LANGUAGES_PATTERN_KEY));
                return _languageRepository.GetById(languageId);
            });
        }

        /// <summary>
        /// Inserts a language
        /// </summary>
        /// <param name="language">Language</param>
        public virtual void InsertLanguage(Language language) {
            if (language == null)
                throw new ArgumentNullException("language");

            _languageRepository.Insert(language);

            //cache
            _signals.Trigger(LANGUAGES_PATTERN_KEY);
        }

        /// <summary>
        /// Updates a language
        /// </summary>
        /// <param name="language">Language</param>
        public virtual void UpdateLanguage(Language language) {
            if (language == null)
                throw new ArgumentNullException("language");

            //update language
            _languageRepository.Update(language);

            //cache
            _signals.Trigger(LANGUAGES_PATTERN_KEY);
        }

        #endregion
    }
}
