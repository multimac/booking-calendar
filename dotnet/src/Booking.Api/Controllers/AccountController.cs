using System.Threading.Tasks;
using Booking.Api.General;
using Booking.Api.Logging;
using Booking.Api.Models;
using Booking.Api.Options;
using Booking.Business.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Booking.Api.Controllers
{
    [Route("[controller]")]
    public class AccountController : Controller
    {
        private ErrorResponseFactory ErrorResponseFactory { get; } = null;
        private AccountOptions Options { get; } = null;
        
        private ILogger<AccountController> Logger { get; } = null;
        private IStringLocalizer Localizer { get; } = null;
        
        private SignInManager<User> SignInManager { get; } = null;
        private UserManager<User> UserManager { get; } = null;
        
        public AccountController(
            ErrorResponseFactory errorResponseFactory,
            IOptions<AccountOptions> optionsAccessor,
            ILogger<AccountController> logger,
            IStringLocalizer<AccountController> localizer,
            SignInManager<User> signInManager,
            UserManager<User> userManager)
        {
            this.ErrorResponseFactory = errorResponseFactory;
            this.Options = optionsAccessor.Value;
            
            this.Logger = logger;
            this.Localizer = localizer;
            
            this.SignInManager = signInManager;
            this.UserManager = userManager;
        }
        
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]LoginModel model)
        {            
            if(!ModelState.IsValid || model == null)
                return ErrorResponseFactory.InvalidModelState(ModelState);
            
            model.Normalize();
            
            var validationResult = await (new LoginModelValidator()).ValidateAsync(model);
            if(!validationResult.IsValid)
                return ErrorResponseFactory.ValidationFailed(validationResult.Errors);
            
            Logger.LoginAttempted(model.Email);
            
            var user = await UserManager.FindByEmailAsync(model.Email);
            if(user == null)
            {
                var error = ErrorResponseFactory.GenerateModel(ErrorCode.FailedLogin);
                return new ObjectResult(error) { StatusCode = 401 };
            }
            
            var signInResult = await SignInManager.PasswordSignInAsync(
                user, model.Password, model.RememberMe.Value,
                Options.LockoutOnFailedLogin
            );
            
            if(!signInResult.Succeeded)
            {
                ErrorModel error;
                if (signInResult.RequiresTwoFactor)
                    error = ErrorResponseFactory.GenerateModel(ErrorCode.TwoFactorRequired);
                else
                    error = ErrorResponseFactory.GenerateModel(ErrorCode.FailedLogin);
                    
                return new ObjectResult(error) { StatusCode = 401 };
            }
            
            return new StatusCodeResult(200);
        }
        
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await SignInManager.SignOutAsync();
            return new StatusCodeResult(200);
        }
    }
}