using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Api
{
    public static class ResponseFactory
    {
        public static readonly string ParseErrorMessage = "Could not parse the request into the required model.";
        public static readonly string ValidationErrorMessage = "Failed validating the request model.";
        
        public static IActionResult InvalidModelState(Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary state)
        {
            var model = new ErrorModel(
                ErrorCode.ParseError, ParseErrorMessage,
                state.Where(kv => kv.Value.Errors.Count > 0)
                    .SelectMany(kv => kv.Value.Errors)
                    .Select(err => new ErrorModel(ErrorCode.ParseError, err.ToString()))
                    .ToArray()
            );
            
            return new JsonResult(model) { StatusCode = 400 };
        }
        public static IActionResult ValidationFailed(IList<FluentValidation.Results.ValidationFailure> errors)
        {
            var model = new ErrorModel(
                ErrorCode.ValidationError, ValidationErrorMessage,
                errors.Select(err => new ErrorModel(ErrorCode.ValidationError, err.ToString()))
                    .ToArray()
            );
            return new JsonResult(model) { StatusCode = 400 };
        }
    }
}