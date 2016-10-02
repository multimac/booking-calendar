using System;
using Booking.Website.Controllers;
using Microsoft.Extensions.Logging;

namespace Booking.Website.Logging.Controllers
{
    public static class LoginControllerExtensions
    {
        private static Action<ILogger, string, Exception> loginAttempted = null;
        private static Action<ILogger, string, Exception> loginSuccessful = null;
        
        static LoginControllerExtensions()
        {
            loginAttempted = LoggerMessage.Define<string>(
                logLevel: LogLevel.Information, eventId: 1,
                formatString: "Login attempted by {User}..."
            );

            loginSuccessful = LoggerMessage.Define<string>(
                logLevel: LogLevel.Information, eventId: 2,
                formatString: "Login successful by {User}"
            );
        }
        
        public static void LoginAttempted(this ILogger<LoginController> logger, string user)
        {
            loginAttempted(logger, user, null);
        }
        public static void LoginSuccessful(this ILogger<LoginController> logger, string user)
        {
            loginSuccessful(logger, user, null);
        }
    }
}