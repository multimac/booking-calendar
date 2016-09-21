using System.Collections.Generic;
using Booking.Common.Mvc.Models;
using FluentValidation;
using FluentValidation.Attributes;

namespace Booking.Website.Models
{
    [Validator(typeof(AuthorizeModelValidator))]
    public class AuthorizeModel : IModel
    {
        public IDictionary<string, string> Parameters { get; set; }

        public void Normalize() { }
    }
    public class AuthorizeModelValidator : AbstractValidator<AuthorizeModel>
    {
        public AuthorizeModelValidator() { }
    }
}