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
                .AddJsonFile($"appsettings-{env.EnvironmentName}.json", optional: true);

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
            services.AddEntityFrameworkNpgsql()
                .AddDbContext<BookingContext>(options =>
                {
                    options.UseNpgsql(Configuration.GetConnectionString("Default"));
                });

            // Set up and configure Identity
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<BookingContext>();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 8;

                options.User.RequireUniqueEmail = true;
            });

            // Set up and configure Localization
            services.AddLocalization(
                options => { options.ResourcesPath = Configuration["Localization:ResourcePath"]; }
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

                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });

            // Set up and configure MVC
            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(GlobalExceptionFilter));
            });

            // Set up custom dependencies for injection
            services.AddScoped<ErrorResponseFactory>();
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory logger)
        {
            logger.AddSerilog();

            if(HostingEnvironment.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseIdentity();

            app.UseMvc();
        }
    }
}