using System.Collections.Generic;

namespace Booking.Common.Mvc.Options
{
    public class StringLocalizerFactoryOptions
    {
        public string DefaultPath { get; set; }

        public IReadOnlyDictionary<string, string> ResourcePaths { get; set; }
    }
}