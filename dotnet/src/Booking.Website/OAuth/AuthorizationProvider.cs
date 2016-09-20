using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Extensions;
using AspNet.Security.OpenIdConnect.Server;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Booking.Website.OAuth
{
    public class AuthorizationProvider : OpenIdConnectServerProvider
    {
        private static readonly string InvalidGrantMessage =
            "Only the authorization code and refresh token grants are accepted by this authorization server";
            
        private static readonly string InvalidResponseMessage =
            "Only the authorization code flow is supported by this server.";

        public override Task ValidateAuthorizationRequest(ValidateAuthorizationRequestContext context)
        {
            var request = context.Request;
            if(!request.IsAuthorizationCodeFlow())
            {
                context.Reject(
                    error: OpenIdConnectConstants.Errors.UnsupportedResponseType,
                    description: InvalidResponseMessage
                );
            }

            context.Validate();

            return Task.CompletedTask;
        }
        public override Task ValidateTokenRequest(ValidateTokenRequestContext context)
        {
            var request = context.Request;
            if (!request.IsAuthorizationCodeGrantType() || request.IsRefreshTokenGrantType())
            {
                context.Reject(
                    error: OpenIdConnectConstants.Errors.UnsupportedGrantType,
                    description: InvalidGrantMessage
                );
            }

            context.Skip();

            return Task.CompletedTask;
        }

        public override async Task HandleTokenRequest(HandleTokenRequestContext context)
        {
            var serviceProvider = context.HttpContext.RequestServices;
            var manager = serviceProvider.GetRequiredService<UserManager<IdentityUser<Guid>>>();

            if (!context.Request.IsPasswordGrantType())
                return;

            var user = await manager.FindByEmailAsync(context.Request.Username);
            if (user == null)
            {
                context.Reject(
                    error: OpenIdConnectConstants.Errors.InvalidGrant,
                    description: "Invalid credentials."
                );

                return;
            }

            if (manager.SupportsUserLockout && await manager.IsLockedOutAsync(user))
            {
                context.Reject(
                    error: OpenIdConnectConstants.Errors.InvalidGrant,
                    description: "Invalid credentials."
                );

                return;
            }

            if (!await manager.CheckPasswordAsync(user, context.Request.Password))
            {
                if (manager.SupportsUserLockout)
                    await manager.AccessFailedAsync(user);

                context.Reject(
                    error: OpenIdConnectConstants.Errors.InvalidGrant,
                    description: "Invalid credentials."
                );

                return;
            }

            if (manager.SupportsUserLockout)
                await manager.ResetAccessFailedCountAsync(user);

            if (manager.SupportsUserTwoFactor && await manager.GetTwoFactorEnabledAsync(user))
            {
                context.Reject(
                    error: OpenIdConnectConstants.Errors.InvalidGrant,
                    description: "Two-factor authentication is required for this account."
                );

                return;
            }

            var identity = new ClaimsIdentity(context.Options.AuthenticationScheme);
            identity.AddClaim(ClaimTypes.NameIdentifier, await manager.GetUserIdAsync(user));

            var ticket = new AuthenticationTicket(
                new ClaimsPrincipal(identity),
                new AuthenticationProperties(),
                context.Options.AuthenticationScheme
            );

            ticket.SetScopes(
                OpenIdConnectConstants.Scopes.OfflineAccess
            );

            ticket.SetResources("api.calend.ar");

            context.Validate(ticket);
        }
    }
}