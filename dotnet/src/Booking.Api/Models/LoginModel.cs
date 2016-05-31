using Booking.Common.Extensions;
using FluentValidation;

namespace Booking.Api.Models
{
    public class LoginModel : IModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        
        public bool? RememberMe { get; set; }

        public void Normalize()
        {
            Email = Email.NullIfWhiteSpace();
            Password = Password.NullIfWhiteSpace();
            
            RememberMe = RememberMe ?? false;
        }
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