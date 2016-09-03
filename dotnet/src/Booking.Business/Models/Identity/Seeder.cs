using System;
using System.Threading.Tasks;
using Booking.Data.Seeding;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Booking.Business.Models.Identity
{
    public class Seeder : EntitySeeder<BookingContext>
    {
        private static readonly IdentityRole<Guid> AdminRole = new IdentityRole<Guid>
        {
            Name = "Administrators"
        };
        private static readonly IdentityUser<Guid> AdminUser = new IdentityUser<Guid>
        {
            UserName = "david.symons",
            Email = "david@symons.io"
        };

        public override async Task Seed(BookingContext context, IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            var userManager = services.GetRequiredService<UserManager<IdentityUser<Guid>>>();

            if (await roleManager.FindByNameAsync(AdminRole.Name) == null)
                await roleManager.CreateAsync(AdminRole);
                
            if (await userManager.FindByEmailAsync(AdminUser.Email) == null)
            {
                await userManager.CreateAsync(AdminUser);
                
                await userManager.AddPasswordAsync(AdminUser, "P@ssw0rd");
                await userManager.AddToRoleAsync(AdminUser, AdminRole.Name);
            }
        }
    }
}