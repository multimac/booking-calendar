using System.Collections.Generic;
using System.Linq;
using Booking.Common.Extensions;
using Booking.Common.Mvc.Models;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Common.Mvc.General
{
    public static class ErrorResponseFactoryExtensions
    {
        /// <summary>
        /// Generates a response with a message for each error in the <see cref="ModelStateDictionary"/>
        /// </summary>
        /// <param name="state">The state containing all error messages</param>
        public static IActionResult InvalidModelState(this ErrorResponseFactory factory, Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary state)
        {
            var model = factory.GenerateModel(ErrorCode.ParseError,
                state.Where(kv => kv.Value.Errors.Count > 0)
                    .SelectMany(kv => kv.Value.Errors)
                    .Select(err => factory.GenerateModel(ErrorCode.ParseError, err.ToString()))
                    .ToArray()
            );
            
            return new ObjectResult(model) { StatusCode = 400 };
        }
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