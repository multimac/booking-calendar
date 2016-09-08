using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Api.Controllers
{
    [Route("[controller]")]
    public class IndexController : Controller
    {
        public IAntiforgery Antiforgery { get; } = null;

        public IndexController(IAntiforgery antiForgery)
        {
            Antiforgery = antiForgery;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var token = Antiforgery.GetAndStoreTokens(HttpContext);

            ViewData["AntiforgeryTokenName"] = token.FormFieldName;
            ViewData["AntiforgeryTokenValue"] = token.RequestToken;

            return View();
        }
    }
}