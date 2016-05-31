using System.Collections.Generic;
using System.Linq;
using Booking.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Booking.Api.General
{
    public class ResponseFactory
    {
        public IStringLocalizer Localizer { get; } = null;
        
        public ResponseFactory(
            IStringLocalizer<ResponseFactory> localizer)
        {
            this.Localizer = localizer;
        }
        
        public IActionResult InvalidModelState(Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary state)
        {
            var model = new ErrorModel(
                ErrorCode.ParseError, Localizer["ParseError"],
                state.Where(kv => kv.Value.Errors.Count > 0)
                    .SelectMany(kv => kv.Value.Errors)
                    .Select(err => new ErrorModel(ErrorCode.ParseError, err.ToString()))
                    .ToArray()
            );
            
            return new JsonResult(model) { StatusCode = 400 };
        }
        public IActionResult ValidationFailed(IList<FluentValidation.Results.ValidationFailure> errors)
        {
            var model = new ErrorModel(
                ErrorCode.ValidationError, Localizer["ValidationError"],
                errors.Select(err => new ErrorModel(ErrorCode.ValidationError, err.ToString()))
                    .ToArray()
            );
            return new JsonResult(model) { StatusCode = 400 };
        }
    }
}