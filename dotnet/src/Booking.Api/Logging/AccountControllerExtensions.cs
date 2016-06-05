using System;
using Booking.Api.Controllers;
using Microsoft.Extensions.Logging;

namespace Booking.Api.Logging
{
    public static class AccountControllerExtensions
    {
        private static Action<ILogger, string, Exception> loginAttempted = null;
        
        static AccountControllerExtensions()
        {
            loginAttempted = LoggerMessage.Define<string>(
                logLevel: LogLevel.Information,
                eventId: 1,
                formatString: "Login attempted by {User}."
            );
        }
        
        public static void LoginAttempted(this ILogger<AccountController> logger, string user)
        {
            loginAttempted(logger, user, null);
        }
    }
}