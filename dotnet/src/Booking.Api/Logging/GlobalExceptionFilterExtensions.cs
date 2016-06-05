using System;
using Booking.Api.General;
using Microsoft.Extensions.Logging;

namespace Booking.Api.Logging
{
    public static class GlobalExceptionFilterExtensions
    {
        private static Action<ILogger, string, Exception> caughtException = null;
        
        static GlobalExceptionFilterExtensions()
        {
            caughtException = LoggerMessage.Define<string>(
                logLevel: LogLevel.Error,
                eventId: 1,
                formatString: "Caught unhandled exception: {Message}"
            );
        }
        
        public static void CaughtException(this ILogger<GlobalExceptionFilter> logger, Exception ex)
        {
            caughtException(logger, ex.Message, ex);
        }
    }
}