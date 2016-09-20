using System.Collections.Generic;
using System.Linq;
using Booking.Common.Mvc.Models;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Common.Mvc.General
{
    public static class ErrorResponseFactoryExtensions
    {
        /// <summary>
        /// Generates a response with a message for each validation error in the <see cref="IList<ValidationFailure>"/>
        /// </summary>
        /// <param name="errors">The list of validation errors that occurred</param>
        public static IActionResult ValidationFailed(this ErrorResponseFactory factory, IList<FluentValidation.Results.ValidationFailure> errors)
        {
            var model = factory.GenerateModel(ErrorCode.ValidationError,
                errors.Select(err => factory.GenerateModel(ErrorCode.ValidationError, err.ToString()))
                    .ToArray()
            );

            return new ObjectResult(model) { StatusCode = 400 };
        }
    }
}