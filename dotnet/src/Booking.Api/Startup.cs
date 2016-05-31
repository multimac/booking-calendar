using System.Globalization;
using System.Linq;
using Booking.Api.General;
using Booking.Api.Identity;
using Booking.Business.Models.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Booking.Api
{
    public class Startup
    {
        private IConfigurationRoot Configuration { get; } = null;
        
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings-{env.EnvironmentName}.json", optional: true);
                
            Configuration = builder.Build();
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            // Set up and configure Identity
            services.AddIdentity<User, Role>();
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
            services.AddMvc();
            
            // Set up custom dependencies for injection
            services.AddScoped<IRoleStore<Role>, RoleStore>();
            services.AddScoped<IUserStore<User>, UserStore>();
            
            services.AddScoped<ResponseFactory>();
        }
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory logger)
        {
            logger.AddConsole(Configuration.GetSection("Logging"));
            
            if(env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseIdentity();
            
            app.UseMvc();
        }
    }
}