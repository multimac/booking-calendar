using System;
using System.Threading.Tasks;
using Booking.Website.Logging.Controllers;
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

namespace Booking.Website.Controllers
{
    [Route("[controller]")]
    public class AccountController : Controller
    {
        private static readonly string ReturnUrlParam = "ReturnUrl";

        private ErrorResponseFactory ErrorResponseFactory { get; } = null;
        private AccountOptions Options { get; } = null;

        private ILogger<AccountController> Logger { get; } = null;

        private SignInManager<IdentityUser<Guid>> SignInManager { get; } = null;
        private UserManager<IdentityUser<Guid>> UserManager { get; } = null;

        public AccountController(
            ErrorResponseFactory errorResponseFactory,
            IOptions<AccountOptions> optionsAccessor,
            ILogger<AccountController> logger,
            SignInManager<IdentityUser<Guid>> signInManager,
            UserManager<IdentityUser<Guid>> userManager)
        {
            this.ErrorResponseFactory = errorResponseFactory;
            this.Options = optionsAccessor.Value;

            this.Logger = logger;

            this.SignInManager = signInManager;
            this.UserManager = userManager;
        }

        [HttpGet("login")]
        public IActionResult Login([FromQuery]string returnUrl = null)
        {
            ViewData[ReturnUrlParam] = returnUrl;
            
            if(User.Identities.Any(i => i.IsAuthenticated))
                return Redirect(returnUrl.NullIfWhiteSpace() ?? "/");

            return View();
        }

        [ValidateAntiForgeryToken, HttpPost("login")]
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
                return View();
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

                return View();
            }

            Logger.LoginSuccessful(model.Email);

            return Redirect(returnUrl);
        }
    }
}