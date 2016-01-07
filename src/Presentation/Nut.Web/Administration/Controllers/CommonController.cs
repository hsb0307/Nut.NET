using Nut.Admin.Extensions;
using Nut.Admin.Models.Common;
using Nut.Core;
using Nut.Services.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nut.Admin.Controllers
{
    public class CommonController : BaseAdminController
    {
        #region Fields
        private readonly ILanguageService _languageService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        #endregion

        #region Ctor
        public CommonController(ILanguageService languageService,
            IWorkContext workContext,
            IStoreContext storeContext) {
            this._languageService = languageService;
            this._storeContext = storeContext;
            this._workContext = workContext;
        }
        #endregion



        [ChildActionOnly]
        public ActionResult LanguageSelector() {
            var model = new LanguageSelectorModel();
            model.CurrentLanguage = _workContext.WorkingLanguage.ToModel();
            model.AvailableLanguages = _languageService
                .GetAllLanguages(storeId: _storeContext.CurrentStore.Id)
                .Select(x => x.ToModel())
                .ToList();
            return PartialView(model);
        }

        public ActionResult SetLanguage(int langid, string returnUrl = "") {
            var language = _languageService.GetLanguageById(langid);
            if (language != null) {
                _workContext.WorkingLanguage = language;
            }

            //home page
            if (String.IsNullOrEmpty(returnUrl))
                returnUrl = Url.Action("Index", "Home", new { area = "Admin" });
            //prevent open redirection attack
            if (!Url.IsLocalUrl(returnUrl))
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            return Redirect(returnUrl);
        }
    }
}