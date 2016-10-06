using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Extensions;
using AspNet.Security.OpenIdConnect.Server;
using Booking.Business;
using Booking.Business.Models.OAuth;
using Booking.Website.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Booking.Website.Controllers.Connect
{
    [Route("connect/[controller]")]
    public class AuthorizeController : Controller
    {
        private static readonly string ViewName = "Views/Connect/Authorize.cshtml";

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

            if(application == null || application.Type == ApplicationType.Introspection)
            {
                return Forbid(OpenIdConnectServerDefaults.AuthenticationScheme);
            }

            var model = new AuthorizeModel
            {
                Application = application.Id,
                Scopes = request.GetScopes().ToList(),

                Parameters = request.Parameters
            };

            return View(ViewName, model);
        }

        [Authorize, ValidateAntiForgeryToken, HttpPost("allow")]
        public async Task<IActionResult> Allow()
        {
            var request = HttpContext.GetOpenIdConnectRequest();

            var application = await BookingContext.Applications
                .Where(a => a.Id == request.ClientId)
                .SingleOrDefaultAsync(HttpContext.RequestAborted);
            
            if(application == null || application.Type == ApplicationType.Introspection)
            {
                return Forbid(OpenIdConnectServerDefaults.AuthenticationScheme);
            }

            var identity = new ClaimsIdentity(OpenIdConnectServerDefaults.AuthenticationScheme);
            identity.AddClaim(ClaimTypes.NameIdentifier, User.FindFirstValue(ClaimTypes.NameIdentifier));

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

            ticket.SetResources("api.calend.ar");
            
            return SignIn(ticket.Principal, ticket.Properties, ticket.AuthenticationScheme);
        }

        [Authorize, ValidateAntiForgeryToken, HttpPost("deny")]
        public IActionResult Deny()
        {
            return Forbid(OpenIdConnectServerDefaults.AuthenticationScheme);
        }
    }
}