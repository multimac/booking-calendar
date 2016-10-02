using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Website.Controllers.Account
{
    [Route("account/[controller]")]
    public class PingController : Controller
    {
        [Authorize, HttpPost]
        public IActionResult Ping()
        {
            return new StatusCodeResult(200);
        }
    }
}