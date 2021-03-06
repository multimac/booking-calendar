﻿using System;
using System.IO;
using System.Threading.Tasks;
using Booking.Business;
using Booking.Common.Mvc.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Booking.Seeder
{
    public class Program
    {
        private static IConfigurationRoot Configuration { get; set; } = null;
        private static IServiceProvider Services { get; set; } = null;

        public static void Main(string[] args)
        {
            SetupConfiguration();
            SetupServices();

            using (var scope = Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<BookingContext>();
                MigrateAndSeed(context).GetAwaiter().GetResult();
            }
        }

        private static void SetupConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("seed.json", optional: true);

            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            if (environment?.ToLower() == "development")
                builder.AddUserSecrets();
            else
                builder.AddEnvironmentVariables();

            Configuration = builder.Build();
        }
        private static void SetupServices()
        {
            var services = new ServiceCollection();

            var connStringBuilder = new NpgsqlConnectionStringBuilder(
                Configuration.GetConnectionString("Booking")
            );

            connStringBuilder.Password = Configuration["BOOKING_PASSWORD"];

            services.AddEntityFrameworkNpgsql()
                .AddDbContext<BookingContext>(options =>
                    options.UseNpgsql(connStringBuilder.ConnectionString)
                );

            services.AddIdentity<IdentityUser<Guid>, IdentityRole<Guid>>()
                .AddEntityFrameworkStores<BookingContext, Guid>();

            // Set up seeding options
            services.Configure<Business.Options.IdentityOptions>(options =>
                Configuration.GetSection("Identity").Bind(options)
            );
            services.Configure<Business.Options.OAuthOptions>(options =>
                Configuration.GetSection("OAuth").Bind(options)
            );

            // Set up custom dependencies for injection
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IPasswordHasher<IdentityUser<Guid>>, PasswordHasher>();

            services.Configure<Common.Mvc.Options.PasswordHasherOptions>(options =>
                Configuration.GetSection("Identity:Hasher").Bind(options)
            );

            Services = services.BuildServiceProvider();
        }

        private static async Task MigrateAndSeed(BookingContext context)
        {
            await context.Database.MigrateAsync();
            await context.EnsureSeedData(Services);
        }
    }
}
