using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Extensions;
using AspNet.Security.OpenIdConnect.Server;
using Booking.Business;
using Booking.Website.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Booking.Website.Controllers
{
    [Route("connect/[controller]")]
    public class AuthorizeController : Controller
    {
        private ILogger<AuthorizeController> Logger { get; } = null;

        private BookingContext BookingContext { get; } = null;

        public AuthorizeController(ILogger<AuthorizeController> logger, BookingContext bookingContext)
        {
            this.Logger = logger;
            
            this.BookingContext = bookingContext;
        }

        [Authorize, HttpGet]
        public async Task<IActionResult> Authorize()
        {
            var request = HttpContext.GetOpenIdConnectRequest();

            var application = await BookingContext.Applications
                .SingleAsync((app) => app.Id == request.ClientId);

            var model = new AuthorizeModel
            {
                Application = application.Id,
                Scopes = request.GetScopes().ToList(),

                Parameters = request.Parameters
            };

            return View(model);
        }

        [Authorize, ValidateAntiForgeryToken, HttpPost("allow")]
        public IActionResult Allow()
        {
            var request = HttpContext.GetOpenIdConnectRequest();

            var identity = new ClaimsIdentity(OpenIdConnectServerDefaults.AuthenticationScheme);
            identity.AddClaim(ClaimTypes.NameIdentifier, User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var ticket = new AuthenticationTicket(
                new ClaimsPrincipal(identity),
                new AuthenticationProperties(),
                OpenIdConnectServerDefaults.AuthenticationScheme
            );

            ticket.SetScopes(
                OpenIdConnectConstants.Scopes.OpenId,
                OpenIdConnectConstants.Scopes.Profile,
                OpenIdConnectConstants.Scopes.Email
            );

            return SignIn(ticket.Principal, ticket.Properties, ticket.AuthenticationScheme);
        }

        [Authorize, ValidateAntiForgeryToken, HttpPost("deny")]
        public IActionResult Deny()
        {
            return Forbid(OpenIdConnectServerDefaults.AuthenticationScheme);
        }
    }
}