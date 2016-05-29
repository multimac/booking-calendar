using FluentValidation;

namespace Booking.Api
{
    public class LoginModel
    {
        public string Email { get; set; } = null;
        public string Password { get; set; } = null;
        
        public bool? RememberMe { get; set; } = false;
    }
    public class LoginModelValidator : AbstractValidator<LoginModel>
    {
        public LoginModelValidator()
        {
            RuleFor(model => model.Email).NotEmpty();
            RuleFor(model => model.Password).NotEmpty();
            
            RuleFor(model => model.RememberMe).NotNull();
        }
    }
}