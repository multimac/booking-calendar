using System.Collections.Generic;
using Booking.Common.Mvc.Models;
using FluentValidation;
using FluentValidation.Attributes;

namespace Booking.Website.Models.Connect
{
    [Validator(typeof(AuthorizeModelValidator))]
    public class AuthorizeModel : IModel
    {
        public string Application { get; set; }
        public IList<string> Scopes { get; set; }

        public IDictionary<string, string> Parameters { get; set; }

        public void Normalize() { }
    }
    public class AuthorizeModelValidator : AbstractValidator<AuthorizeModel>
    {
        public AuthorizeModelValidator() { }
    }
}