using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Booking.Business;
using Booking.Common.Mvc.Filters;
using Booking.Common.Mvc.General;
using Booking.Common.Mvc.Identity;
using Booking.Common.Mvc.Localization;
using Booking.Common.Mvc.Options;
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

namespace Booking.Api
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
            var connStringBuilder = new NpgsqlConnectionStringBuilder(
                Configuration.GetConnectionString("Default")
            );

            connStringBuilder.Password = Configuration["BOOKING_PASSWORD"];

            services
                .AddEntityFrameworkNpgsql()
                .AddDbContext<BookingContext>(
                    options => options.UseNpgsql(connStringBuilder.ConnectionString)
                );

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
            services.AddCors();
            services.AddAuthentication();

            services.AddMvc(
                options => options.Filters.Add(typeof(GlobalExceptionFilter))
            );

            // Set up custom dependencies for injection
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IPasswordHasher<IdentityUser<Guid>>, PasswordHasher>();

            services.Configure<Common.Mvc.Options.PasswordHasherOptions>(options =>
                Configuration.GetSection("Identity:Hasher").Bind(options)
            );

            services.AddScoped<ErrorResponseFactory>();
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory logger)
        {
            logger.AddSerilog();

            // Set up pipeline
            if (HostingEnvironment.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

            app.UseOAuthIntrospection(options =>
            {
                Options.OAuthOptions oAuthOptions = new Options.OAuthOptions();
                Configuration.GetSection("OAuth").Bind(oAuthOptions);

                options.Audiences.Add("api.calend.ar");

                options.Authority = oAuthOptions.Authority;
                options.ClientId = oAuthOptions.ClientId;

                options.ClientSecret = Configuration["BOOKING_OAUTH_SECRET"];
            });

            app.UseMvc();
        }
    }
}