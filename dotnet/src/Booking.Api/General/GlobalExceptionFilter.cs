using Booking.Api.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Booking.Api.General
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private ErrorResponseFactory ErrorResponseFactory { get; } = null;
        
        private IHostingEnvironment HostingEnvironment { get; } = null;
        private ILogger<GlobalExceptionFilter> Logger { get; } = null;
        
        public GlobalExceptionFilter(
            ErrorResponseFactory errorResponseFactory,
            IHostingEnvironment hostingEnvironment,
            ILogger<GlobalExceptionFilter> logger)
        {
            this.ErrorResponseFactory = errorResponseFactory;
            
            this.HostingEnvironment = hostingEnvironment;
            this.Logger = logger;
        }
        
        public void OnException(ExceptionContext context)
        {   
            var exception = context.Exception;
            Logger.CaughtException(exception);
         
            if(!HostingEnvironment.IsDevelopment())
                context.Result = ErrorResponseFactory.InternalServerError(exception);
        }
    }
}