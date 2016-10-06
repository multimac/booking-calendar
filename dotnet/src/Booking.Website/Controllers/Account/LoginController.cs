using System;
using System.Threading.Tasks;
using Booking.Website.Logging.Controllers.Account;
using Booking.Website.Models;
using Booking.Website.Options.Controllers;
using Booking.Common.Mvc.General;
using Booking.Common.Mvc.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Booking.Common.Extensions;
using System.Linq;

namespace Booking.Website.Controllers.Account
{
    [Route("account/[controller]")]
    public class LoginController : Controller
    {
        private static readonly string ReturnUrlParam = "ReturnUrl";
        private static readonly string ViewName = "Views/Account/Login.cshtml";

        private ErrorResponseFactory ErrorResponseFactory { get; } = null;
        private LoginOptions Options { get; } = null;

        private ILogger<LoginController> Logger { get; } = null;

        private SignInManager<IdentityUser<Guid>> SignInManager { get; } = null;
        private UserManager<IdentityUser<Guid>> UserManager { get; } = null;

        public LoginController(
            ErrorResponseFactory errorResponseFactory,
            IOptions<LoginOptions> optionsAccessor,
            ILogger<LoginController> logger,
            SignInManager<IdentityUser<Guid>> signInManager,
            UserManager<IdentityUser<Guid>> userManager)
        {
            this.ErrorResponseFactory = errorResponseFactory;
            this.Options = optionsAccessor.Value;

            this.Logger = logger;

            this.SignInManager = signInManager;
            this.UserManager = userManager;
        }

        [HttpGet]
        public IActionResult Login([FromQuery]string returnUrl = null)
        {
            ViewData[ReturnUrlParam] = returnUrl;
            
            if(User.Identities.Any(i => i.IsAuthenticated))
                return Redirect(returnUrl.NullIfWhiteSpace() ?? "/");

            return View(ViewName);
        }

        [ValidateAntiForgeryToken, HttpPost]
        public async Task<IActionResult> Login([FromForm]LoginModel model, [FromQuery]string returnUrl = null)
        {
            ViewData[ReturnUrlParam] = returnUrl;

            if (!ModelState.IsValid || model == null)
                return View(model);

            model.Normalize();
            returnUrl = returnUrl.NullIfWhiteSpace() ?? "/";
            
            if(User.Identities.Any(i => i.IsAuthenticated))
                return Redirect(returnUrl);

            Logger.LoginAttempted(model.Email);

            var user = await UserManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                var error = ErrorResponseFactory.GenerateModel(ErrorCode.FailedLogin);
                return View(ViewName);
            }

            var signInResult = await SignInManager.PasswordSignInAsync(
                user, model.Password, model.RememberMe,
                Options.LockoutOnFailedLogin
            );

            if (!signInResult.Succeeded)
            {
                ErrorModel error;
                if (signInResult.RequiresTwoFactor)
                    error = ErrorResponseFactory.GenerateModel(ErrorCode.TwoFactorRequired);
                else
                    error = ErrorResponseFactory.GenerateModel(ErrorCode.FailedLogin);

                return View(ViewName);
            }

            Logger.LoginSuccessful(model.Email);

            return Redirect(returnUrl);
        }
    }
}