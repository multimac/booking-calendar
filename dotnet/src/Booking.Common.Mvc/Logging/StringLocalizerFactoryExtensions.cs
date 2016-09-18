using System;
using Booking.Common.Mvc.Localization;
using Microsoft.Extensions.Logging;

namespace Booking.Common.Mvc.Logging
{
    public static class StringLocalizerFactoryExtensions
    {
        private static Action<ILogger, string, string, Exception> keyNotFound = null;
        
        static StringLocalizerFactoryExtensions()
        {
            keyNotFound = LoggerMessage.Define<string, string>(
                logLevel: LogLevel.Information,
                eventId: 1,
                formatString: "Could not find path for {AssemblyName}, using default value ({Default})."
            );
        }
        
        public static void KeyNotFound(this ILogger<StringLocalizerFactory> logger, string assembly, string defaultPath)
        {
            keyNotFound(logger, assembly, defaultPath, null);
        }
    }
}