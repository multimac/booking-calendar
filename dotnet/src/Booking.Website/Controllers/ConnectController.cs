using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AspNet.Security.OpenIdConnect.Server;
using Microsoft.AspNetCore.Builder;
using System.Security.Claims;
using AspNet.Security.OpenIdConnect.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Authentication;
using Booking.Website.Models;

namespace Booking.Website.Controllers
{
    [Route("[controller]")]
    public class ConnectController : Controller
    {
        private ILogger<ConnectController> Logger { get; } = null;
        private UserManager<IdentityUser<Guid>> UserManager { get; } = null;

        public ConnectController(
            ILogger<ConnectController> logger,
            UserManager<IdentityUser<Guid>> userManager)
        {
            this.Logger = logger;
            this.UserManager = userManager;
        }

        [Authorize, HttpGet("authorize")]
        public IActionResult Authorize()
        {
            var request = HttpContext.GetOpenIdConnectRequest();
            var model = new AuthorizeModel { Parameters = request.Parameters };

            return View(model);
        }

        [Authorize, ValidateAntiForgeryToken, HttpPost("authorize/allow")]
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

        [Authorize, ValidateAntiForgeryToken, HttpPost("authorize/deny")]
        public IActionResult Deny()
        {
            return Forbid(OpenIdConnectServerDefaults.AuthenticationScheme);
        }
    }
}