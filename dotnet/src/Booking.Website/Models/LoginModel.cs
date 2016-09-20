using Booking.Common.Mvc.Models;
using FluentValidation;
using FluentValidation.Attributes;

namespace Booking.Website.Models
{
    [Validator(typeof(LoginModelValidator))]
    public class LoginModel : IModel
    {
        public string Email { get; set; } = null;
        public string Password { get; set; } = null;

        public bool RememberMe { get; set; } = false;

        public void Normalize() { }
    }
    public class LoginModelValidator : AbstractValidator<LoginModel>
    {
        public LoginModelValidator()
        {
            RuleFor(model => model.Email).NotEmpty();
            RuleFor(model => model.Password).NotEmpty();
        }
    }
}