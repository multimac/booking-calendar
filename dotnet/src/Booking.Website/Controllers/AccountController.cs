using System;
using System.Threading.Tasks;
using Booking.Website.Logging.Controllers;
using Booking.Website.Models;
using Booking.Website.Options.Controllers;
using Booking.Common.Mvc.General;
using Booking.Common.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Booking.Website.Controllers
{
    [Route("[controller]")]
    public class AccountController : Controller
    {
        private ErrorResponseFactory ErrorResponseFactory { get; } = null;
        private AccountOptions Options { get; } = null;

        private IStringLocalizer Localizer { get; } = null;
        private ILogger<AccountController> Logger { get; } = null;

        private SignInManager<IdentityUser<Guid>> SignInManager { get; } = null;
        private UserManager<IdentityUser<Guid>> UserManager { get; } = null;

        public AccountController(
            ErrorResponseFactory errorResponseFactory,
            IOptions<AccountOptions> optionsAccessor,
            IStringLocalizer<AccountController> localizer,
            ILogger<AccountController> logger,
            SignInManager<IdentityUser<Guid>> signInManager,
            UserManager<IdentityUser<Guid>> userManager)
        {
            this.ErrorResponseFactory = errorResponseFactory;
            this.Options = optionsAccessor.Value;

            this.Localizer = localizer;
            this.Logger = logger;

            this.SignInManager = signInManager;
            this.UserManager = userManager;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]LoginModel model)
        {
            if (!ModelState.IsValid || model == null)
                return ErrorResponseFactory.InvalidModelState(ModelState);

            model.Normalize();

            var validationResult = await (new LoginModelValidator()).ValidateAsync(model);
            if (!validationResult.IsValid)
                return ErrorResponseFactory.ValidationFailed(validationResult.Errors);

            Logger.LoginAttempted(model.Email);

            var user = await UserManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                var error = ErrorResponseFactory.GenerateModel(ErrorCode.FailedLogin);
                return new ObjectResult(error) { StatusCode = 401 };
            }

            var signInResult = await SignInManager.PasswordSignInAsync(
                user, model.Password, model.RememberMe.Value,
                Options.LockoutOnFailedLogin
            );

            if (!signInResult.Succeeded)
            {
                ErrorModel error;
                if (signInResult.RequiresTwoFactor)
                    error = ErrorResponseFactory.GenerateModel(ErrorCode.TwoFactorRequired);
                else
                    error = ErrorResponseFactory.GenerateModel(ErrorCode.FailedLogin);

                return new ObjectResult(error) { StatusCode = 401 };
            }

            Logger.LoginSuccessful(model.Email);

            return new StatusCodeResult(200);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await SignInManager.SignOutAsync();
            return new StatusCodeResult(200);
        }

        [Authorize]
        [HttpPost("ping")]
        public IActionResult Ping()
        {
            return new StatusCodeResult(200);
        }
    }
}