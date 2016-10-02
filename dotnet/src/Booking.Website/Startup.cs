using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AspNet.Security.OpenIdConnect.Server;
using Booking.Business;
using Booking.Common.Mvc.Filters;
using Booking.Common.Mvc.General;
using Booking.Common.Mvc.Identity;
using Booking.Common.Mvc.Localization;
using Booking.Common.Mvc.Options;
using Booking.Website.OAuth;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Npgsql;
using Serilog;

namespace Booking.Website
{
    public class Startup
    {
        private IConfigurationRoot Configuration { get; } = null;
        private IHostingEnvironment HostingEnvironment { get; } = null;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings-{env.EnvironmentName.ToLower()}.json", optional: true);

            if (env.IsDevelopment())
                builder.AddUserSecrets();
            else
                builder.AddEnvironmentVariables();

            Configuration = builder.Build();
            HostingEnvironment = env;

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.ColoredConsole()
                .CreateLogger();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Set up and configure Entity Framework
            NpgsqlConnectionStringBuilder connStringBuilder = new NpgsqlConnectionStringBuilder(
                Configuration["BOOKING_CONNECTIONSTRING"]
            );

            connStringBuilder.Password = Configuration["BOOKING_PASSWORD"];

            services.AddEntityFrameworkNpgsql()
                .AddDbContext<BookingContext>(
                    options => options.UseNpgsql(connStringBuilder.ConnectionString)
                );

            services.Configure<Business.Options.IdentityOptions>(options =>
            {
                options.AdminEmail = Configuration["BOOKING_ADMIN_EMAIL"];
                options.AdminPassword = Configuration["BOOKING_ADMIN_PASSWORD"];
            });

            // Set up and configure Identity
            services.AddIdentity<IdentityUser<Guid>, IdentityRole<Guid>>()
                .AddEntityFrameworkStores<BookingContext, Guid>();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 8;
                options.User.RequireUniqueEmail = true;

                var appCookie = options.Cookies.ApplicationCookie;
                appCookie.CookieName = Configuration["Identity:CookieName"];

                appCookie.LoginPath = "/account/login";
                appCookie.ReturnUrlParameter = "returnurl";
            });

            // Set up and configure Localization
            services.AddSingleton<IStringLocalizerFactory, StringLocalizerFactory>();
            services.Configure<StringLocalizerFactoryOptions>(options =>
                Configuration.GetSection("Localization:Factory").Bind(options)
            );

            services.AddLocalization();
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var defaultCulture = Configuration["Localization:DefaultCulture"];

                var supportedCulturesConfig = Configuration.GetSection("Localization:SupportedCultures");
                var supportedCultures = supportedCulturesConfig.GetChildren()
                    .Select(child => new CultureInfo(child.ToString()))
                    .ToArray();

                options.DefaultRequestCulture = new RequestCulture(
                    culture: defaultCulture, uiCulture: defaultCulture
                );

                options.RequestCultureProviders = new List<IRequestCultureProvider>()
                {
                    new CookieRequestCultureProvider() { CookieName = Configuration["Localization:CookieName"] },
                    new AcceptLanguageHeaderRequestCultureProvider()
                };

                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });

            // Set up and configure MVC
            services.AddAntiforgery();
            services.AddRouting(options => options.LowercaseUrls = true);

            services.AddMvc(options => options.Filters.Add(typeof(GlobalExceptionFilter)))
                .AddFluentValidation(options => options.RegisterValidatorsFromAssemblyContaining<Startup>());

            // Set up custom dependencies for injection
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IPasswordHasher<IdentityUser<Guid>>, PasswordHasher>();

            services.Configure<Common.Mvc.Options.PasswordHasherOptions>(options =>
                Configuration.GetSection("Identity:Hasher").Bind(options)
            );

            services.AddScoped<ErrorResponseFactory>();

            services.AddSingleton<IOpenIdConnectServerProvider, AuthorizationProvider>();
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory logger)
        {
            logger.AddSerilog();

            // Seed any missing data
            if (HostingEnvironment.IsDevelopment())
            {
                using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<BookingContext>();
                    context.EnsureSeedData(scope.ServiceProvider).GetAwaiter().GetResult();
                }
            }

            // Set up pipeline
            if (HostingEnvironment.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseIdentity();

            app.UseOpenIdConnectServer(options =>
            {
                options.Provider = app.ApplicationServices
                    .GetRequiredService<IOpenIdConnectServerProvider>();

                options.AuthorizationEndpointPath = "/connect/authorize";
                options.TokenEndpointPath = "/connect/token";
                options.RevocationEndpointPath = "/connect/revoke";

                options.AllowInsecureHttp = true;
            });

            app.UseMvc();
        }
    }
}