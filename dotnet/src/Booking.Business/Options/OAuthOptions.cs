using System.Collections.Generic;
using Booking.Business.Models.OAuth;

namespace Booking.Business.Options
{
    public class OAuthOptions
    {
        public List<Application> Applications { get; set; } = null;
    }
}