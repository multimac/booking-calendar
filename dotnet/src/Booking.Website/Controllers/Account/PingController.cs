using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Website.Controllers
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