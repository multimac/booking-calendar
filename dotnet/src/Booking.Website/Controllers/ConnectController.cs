using System;
using System.Threading.Tasks;
using Booking.Website.Logging.Controllers;
using Booking.Website.Models;
using Booking.Common.Mvc.General;
using Booking.Common.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AspNet.Security.OpenIdConnect.Server;

namespace Booking.Website.Controllers
{
    [Route("connect")]
    public class ConnectController : Controller
    {
        private ErrorResponseFactory ErrorResponseFactory { get; } = null;

        private ILogger<ConnectController> Logger { get; } = null;
        private UserManager<IdentityUser<Guid>> UserManager { get; } = null;

        public ConnectController(
            ErrorResponseFactory errorResponseFactory,
            ILogger<ConnectController> logger,
            UserManager<IdentityUser<Guid>> userManager)
        {
            this.ErrorResponseFactory = errorResponseFactory;

            this.Logger = logger;
            this.UserManager = userManager;
        }

        [Authorize, HttpGet("authorize")]
        public async Task<IActionResult> Authorize()
        {
            return null;
        }

        [Authorize, ValidateAntiForgeryToken, HttpPost("authorize/allow")]
        public async Task<IActionResult> Allow()
        {
            return null;
        }

        [Authorize, ValidateAntiForgeryToken, HttpPost("authorize/deny")]
        public IActionResult Deny()
        {
            return Forbid(OpenIdConnectServerDefaults.AuthenticationScheme);
        }
    }
}