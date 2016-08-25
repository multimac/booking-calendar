using System;
using System.Collections.Generic;
using System.Linq;
using Booking.Api.Models;
using Booking.Common.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Booking.Api.General
{
    public class ErrorResponseFactory
    {
        public IStringLocalizer Localizer { get; } = null;
        
        public ErrorResponseFactory(
            IStringLocalizer<ErrorResponseFactory> localizer)
        {
            this.Localizer = localizer;
        }
        
        public ErrorModel GenerateModel(ErrorCode code)
        {
            return GenerateModel(code, null, null);
        }
        public ErrorModel GenerateModel(ErrorCode code, string message)
        {
            return GenerateModel(code, message, null);
        }
        public ErrorModel GenerateModel(ErrorCode code, ErrorModel[] innerErrors)
        {
            return GenerateModel(code, null, innerErrors);
        }
        public ErrorModel GenerateModel(ErrorCode code, string message, ErrorModel[] innerErrors)
        {
            string reason = code.ToString();
            return new ErrorModel()
            {
                Code = code,
                Reason = reason,
                Message = message ?? Localizer[reason],
                
                InnerErrors = innerErrors
            };
        }
        
        public IActionResult InvalidModelState(Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary state)
        {
            var model = GenerateModel(ErrorCode.ParseError,
                state.Where(kv => kv.Value.Errors.Count > 0)
                    .SelectMany(kv => kv.Value.Errors)
                    .Select(err => GenerateModel(ErrorCode.ParseError, err.ToString()))
                    .ToArray()
            );
            
            return new ObjectResult(model) { StatusCode = 400 };
        }
        public IActionResult InternalServerError(System.Exception ex)
        {
            var model = GenerateModel(ErrorCode.InternalServerError,
                String.Join(" => ",
                    ex.Yield().Traverse(e => e.InnerException?.Yield())
                        .Select(e => e.GetType().Name)
                )
            );
            
            return new ObjectResult(model) { StatusCode = 500 };
        }
        public IActionResult ValidationFailed(IList<FluentValidation.Results.ValidationFailure> errors)
        {
            var model = GenerateModel(ErrorCode.ValidationError,
                errors.Select(err => new ErrorModel(ErrorCode.ValidationError, err.ToString()))
                    .ToArray()
            );

            return new ObjectResult(model) { StatusCode = 400 };
        }
    }
}