using Nut.Web.Framework.Security;
using Nut.Web.Models.User;
using System;
using System.Web.Mvc;
using Nut.Services.Authentication;
using Nut.Services.Users;
using Nut.Core.Domain.Users;
using Nut.Services.Localization;

namespace Nut.Web.Controllers {
    public class UserController : BasePublicController {

        private readonly IUserRegistrationService _userRegistrationService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IUserService _userService;
        private readonly ILocalizationService _localizationService;

        public UserController(IUserRegistrationService userRegistrationService, 
            IUserService userService,
            IAuthenticationService authenticationService,
            ILocalizationService localizationService) {
            this._userRegistrationService = userRegistrationService;
            this._userService = userService;
            this._authenticationService = authenticationService;
            this._localizationService = localizationService;
        }

        // GET: User
        public ActionResult Index() {
            return View();
        }

        #region Login / logout

        [NopHttpsRequirement(SslRequirement.Yes)]
        public ActionResult Login(bool? checkoutAsGuest) {
            var model = new LoginModel();
            model.CheckoutAsGuest = checkoutAsGuest.GetValueOrDefault();
            return View(model);
        }

        [HttpPost]
        public ActionResult Login(LoginModel model, string returnUrl) {
            //validate CAPTCHA

            if (ModelState.IsValid) {

                model.Username = model.Username.Trim();

                var loginResult = _userRegistrationService.ValidateUser(model.Username, model.Password);
                switch (loginResult) {
                    case UserLoginResults.Successful:
                        {
                            var customer = _userService.GetUserByUsername(model.Username);

                            //sign in new customer
                            _authenticationService.SignIn(customer, model.RememberMe);


                            if (String.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
                                return RedirectToRoute("HomePage");

                            return Redirect(returnUrl);
                        }
                    case UserLoginResults.CustomerNotExist:
                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.CustomerNotExist"));
                        break;
                    case UserLoginResults.Deleted:
                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.Deleted"));
                        break;
                    case UserLoginResults.NotActive:
                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.NotActive"));
                        break;
                    case UserLoginResults.NotRegistered:
                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.NotRegistered"));
                        break;
                    case UserLoginResults.WrongPassword:
                    default:
                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials"));
                        break;
                }
            }
            return View(model);
        }

        public ActionResult Logout() {

            _authenticationService.SignOut();
            return RedirectToRoute("HomePage");
        }

        #endregion
    }
}