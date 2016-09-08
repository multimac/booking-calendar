using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Booking.Api.General;
using Booking.Business;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            NpgsqlConnectionStringBuilder connStringBuilder = new NpgsqlConnectionStringBuilder(
                Configuration["BOOKING_API_CONNECTIONSTRING"]
            );

            connStringBuilder.Password = Configuration["BOOKING_API_PASSWORD"];

            services
                .AddEntityFrameworkNpgsql()
                .AddDbContext<BookingContext>(
                    options => options.UseNpgsql(connStringBuilder.ConnectionString)
                );

            services.Configure<Business.Options.IdentityOptions>(options => 
            {
                options.AdminEmail = Configuration["BOOKING_API_ADMIN_EMAIL"];
                options.AdminPassword = Configuration["BOOKING_API_ADMIN_PASSWORD"];
            });

            // Set up and configure Identity
            services
                .AddIdentity<IdentityUser<Guid>, IdentityRole<Guid>>()
                .AddEntityFrameworkStores<BookingContext, Guid>();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 8;
                options.User.RequireUniqueEmail = true;

                var appCookie = options.Cookies.ApplicationCookie;
                appCookie.CookieName = Configuration["Identity:CookieName"];
            });

            // Set up and configure Localization
            services.AddLocalization(
                options => options.ResourcesPath = Configuration["Localization:ResourcePath"]
            );

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
            services.AddMvc(
                options => options.Filters.Add(typeof(GlobalExceptionFilter))
            );

            // Set up custom dependencies for injection
            services.AddScoped<ErrorResponseFactory>();
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory logger)
        {
            logger.AddSerilog();

            // Seed any missing data
            if (HostingEnvironment.IsDevelopment())
            {
                using(var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<BookingContext>();
                    context.EnsureSeedData(scope.ServiceProvider).GetAwaiter().GetResult();
                }
            }

            // Set up pipeline
            if(HostingEnvironment.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseIdentity();

            app.UseMvc();
        }
    }
}