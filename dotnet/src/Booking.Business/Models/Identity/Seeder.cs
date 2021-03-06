using System;
using System.Threading.Tasks;
using Booking.Business.Options;
using Booking.Data.Seeding;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Booking.Business.Models.Identity
{
    public class Seeder : EntitySeeder<BookingContext>
    {
        private static readonly IdentityRole<Guid> AdminRole = new IdentityRole<Guid> { Name = "Administrators" };
        private static readonly IdentityUser<Guid> AdminUser = new IdentityUser<Guid> { UserName = "admin" };

        public override async Task Seed(BookingContext context, IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

            if (await roleManager.FindByNameAsync(AdminRole.Name) == null)
                await roleManager.CreateAsync(AdminRole);

            await CreateAdminUser(services);
        }
        public override Task SeedDev(BookingContext context, IServiceProvider services) => Task.CompletedTask;

        private async Task CreateAdminUser(IServiceProvider services)
        {
            var logger = services.GetRequiredService<ILogger<Seeder>>();

            var optionsAccessor = services.GetRequiredService<IOptions<IdentityOptions>>();
            var options = optionsAccessor.Value;

            var userManager = services.GetRequiredService<UserManager<IdentityUser<Guid>>>();

            var existingAdmin = await userManager.FindByNameAsync(AdminUser.UserName);
            if (options.AdminEmail == null)
            {
                if (existingAdmin != null)
                    await userManager.DeleteAsync(existingAdmin);

                return;
            }

            var existingUser = await userManager.FindByEmailAsync(options.AdminEmail);
            if (existingUser != null)
            {
                if (existingAdmin == null || existingAdmin.Id != existingUser.Id)
                    throw new InvalidOperationException("User already exists with admin email, skipping...");

                return;
            }

            if (existingAdmin != null)
            {
                await userManager.SetEmailAsync(existingAdmin, options.AdminEmail);
                return;
            }

            AdminUser.Email = options.AdminEmail;
            await userManager.CreateAsync(AdminUser);

            await userManager.AddToRoleAsync(AdminUser, AdminRole.Name);

            if (options.AdminPassword != null)
                await userManager.AddPasswordAsync(AdminUser, options.AdminPassword);
        }
    }
}