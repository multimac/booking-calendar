using System;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Extensions;
using AspNet.Security.OpenIdConnect.Server;
using Booking.Business;
using Booking.Business.Models.OAuth;
using Booking.Common.Mvc.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Booking.Website.OAuth
{
    public class AuthorizationProvider : OpenIdConnectServerProvider
    {
        private static readonly string InvalidClientMessage =
            "The given client failed identification/authentication";
        private static readonly string InvalidGrantMessage =
            "Only the authorization code and refresh token grants are accepted by this authorization server";
        private static readonly string InvalidResponseMessage =
            "Only the authorization code flow is supported by this server.";

        private static readonly string[] ValidAuthorizeEndpoints
            = new string[] { "/allow", "/deny" };

        private BookingContext BookingContext { get; } = null;
        private IPasswordHasher PasswordHasher { get; } = null;

        public AuthorizationProvider(BookingContext bookingContext, IPasswordHasher passwordHasher)
        {
            BookingContext = bookingContext;
            PasswordHasher = passwordHasher;
        }

        private bool MatchRedirectUrl(Application application, string url)
        {
            Uri uri;
            if (!Uri.TryCreate(url, UriKind.Absolute, out uri))
                return false;

            // Only allow "https", or "http" if specifically allowed
            if (uri.Scheme != "https")
            {
                if (uri.Scheme != "http" && application.RedirectAllowHttp)
                    return false;
            }

            var cleanedUrl = uri.GetComponents(
                UriComponents.HttpRequestUrl & ~UriComponents.Scheme, UriFormat.Unescaped
            );

            var escapedTemplate = Regex.Escape(application.RedirectUrl);

            var subdomain = (application.RedirectAllowSubdomains ? @".*\." : string.Empty);
            var subpath = (application.RedirectAllowSubpaths ? @"/.*" : string.Empty);

            var template = new Regex($"^{subdomain}{escapedTemplate}{subpath}$");

            return template.IsMatch(cleanedUrl);
        }

        public override Task MatchEndpoint(MatchEndpointContext context)
        {
            if (!context.Options.AuthorizationEndpointPath.HasValue)
                return Task.CompletedTask;

            var requestPath = context.Request.Path;
            var authorizePath = context.Options.AuthorizationEndpointPath;

            if (ValidAuthorizeEndpoints.Any((endpoint) => (requestPath == authorizePath.Add(endpoint))))
                context.MatchesAuthorizationEndpoint();

            return Task.CompletedTask;
        }

        public override async Task ValidateAuthorizationRequest(ValidateAuthorizationRequestContext context)
        {
            var request = context.Request;
            if (!request.IsAuthorizationCodeFlow())
            {
                context.Reject(
                    error: OpenIdConnectConstants.Errors.UnsupportedResponseType,
                    description: InvalidResponseMessage
                );

                return;
            }

            if (string.IsNullOrWhiteSpace(context.ClientId))
            {
                context.Reject(
                    error: OpenIdConnectConstants.Errors.InvalidClient,
                    description: InvalidClientMessage
                );

                return;
            }

            var application = await BookingContext.Applications
                .Where(a => a.Id == context.ClientId)
                .SingleOrDefaultAsync(context.HttpContext.RequestAborted);

            if (application == null || application.Type == ApplicationType.Introspection)
            {
                context.Reject(
                    error: OpenIdConnectConstants.Errors.InvalidClient,
                    description: InvalidClientMessage
                );

                return;
            }

            if (!MatchRedirectUrl(application, context.RedirectUri))
            {
                context.Reject(
                    error: OpenIdConnectConstants.Errors.InvalidClient,
                    description: InvalidClientMessage
                );

                return;
            }

            context.Validate();
        }
        public override async Task ValidateIntrospectionRequest(ValidateIntrospectionRequestContext context)
        {
            if (string.IsNullOrWhiteSpace(context.ClientId))
            {
                context.Reject(
                    error: OpenIdConnectConstants.Errors.InvalidClient,
                    description: InvalidClientMessage
                );

                return;
            }

            var application = await BookingContext.Applications
                .Where(a => a.Id == context.ClientId)
                .SingleOrDefaultAsync(context.HttpContext.RequestAborted);
                
            if (application == null || application.Type != ApplicationType.Introspection)
            {
                context.Reject(
                    error: OpenIdConnectConstants.Errors.InvalidClient,
                    description: InvalidClientMessage
                );

                return;
            }

            if (!PasswordHasher.VerifyHashedPassword(application.Secret, context.ClientSecret))
            {
                context.Reject(
                    error: OpenIdConnectConstants.Errors.InvalidClient,
                    description: InvalidClientMessage
                );

                return;
            }

            context.Validate();
        }
        public override async Task ValidateTokenRequest(ValidateTokenRequestContext context)
        {
            var request = context.Request;
            if (!request.IsAuthorizationCodeGrantType() && !request.IsRefreshTokenGrantType())
            {
                context.Reject(
                    error: OpenIdConnectConstants.Errors.UnsupportedGrantType,
                    description: InvalidGrantMessage
                );

                return;
            }

            if (string.IsNullOrWhiteSpace(context.ClientId))
            {
                context.Reject(
                    error: OpenIdConnectConstants.Errors.InvalidClient,
                    description: InvalidClientMessage
                );

                return;
            }

            var application = await BookingContext.Applications
                .Where(a => a.Id == context.ClientId)
                .SingleOrDefaultAsync(context.HttpContext.RequestAborted);

            if (application == null || application.Type == ApplicationType.Introspection)
            {
                context.Reject(
                    error: OpenIdConnectConstants.Errors.InvalidClient,
                    description: InvalidClientMessage
                );

                return;
            }

            if (application.Type == ApplicationType.Public)
            {
                if (!string.IsNullOrEmpty(context.ClientSecret))
                {
                    context.Reject(
                        error: OpenIdConnectConstants.Errors.InvalidClient,
                        description: InvalidClientMessage
                    );

                    return;
                }

                context.Skip();
                return;
            }

            if (!PasswordHasher.VerifyHashedPassword(application.Secret, context.ClientSecret))
            {
                context.Reject(
                    error: OpenIdConnectConstants.Errors.InvalidClient,
                    description: InvalidClientMessage
                );

                return;
            }

            context.Validate();
        }
    }
}