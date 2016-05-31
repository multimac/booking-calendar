using System.Threading.Tasks;
using Booking.Api.General;
using Booking.Api.Models;
using Booking.Api.Options;
using Booking.Business.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace Booking.Api.Controllers
{
    [Route("[controller]")]
    public class AccountController : Controller
    {
        private ErrorResponseFactory ErrorResponseFactory { get; } = null;
        
        private AccountOptions Options { get; } = null;
        private IStringLocalizer Localizer { get; } = null;
        
        private SignInManager<User> SignInManager { get; } = null;
        private UserManager<User> UserManager { get; } = null;
        
        public AccountController(
            ErrorResponseFactory errorResponseFactory,
            IOptions<AccountOptions> optionsAccessor,
            IStringLocalizer<AccountController> localizer,
            SignInManager<User> signInManager,
            UserManager<User> userManager)
        {
            this.ErrorResponseFactory = errorResponseFactory;
            
            this.Options = optionsAccessor.Value;
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
            
            var user = await UserManager.FindByEmailAsync(model.Email);
            if(user == null)
            {
                var error = ErrorResponseFactory.GenerateModel(ErrorCode.FailedLogin);
                return new JsonResult(error) { StatusCode = 401 };
            }
            
            var signInResult = await SignInManager.PasswordSignInAsync(
                user, model.Password, model.RememberMe.Value,
                Options.LockoutOnFailedLogin
            );
            
            if(!signInResult.Succeeded)
            {
                ErrorModel error;
                if (signInResult.RequiresTwoFactor)
                    error = new ErrorModel(ErrorCode.TwoFactorRequired);
                else
                    error = ErrorResponseFactory.GenerateModel(ErrorCode.FailedLogin);
                    
                return new JsonResult(error) { StatusCode = 401 };
            }
            
            return new JsonResult(null) { StatusCode = 200 };
        }
        
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await SignInManager.SignOutAsync();
            return new JsonResult(null) { StatusCode = 200 };
        }
    }
}