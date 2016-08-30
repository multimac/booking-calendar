using Booking.Data.Mapping;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Booking.Business.Models.Identity
{
    public class Mapping : IEntityMappingConfiguration
    {
        public void Map(ModelBuilder builder)
        {
            builder.Entity<IdentityRole<System.Guid>>().ToTable("Identity.Roles");
            builder.Entity<IdentityRoleClaim<System.Guid>>().ToTable("Identity.RoleClaims");
            
            builder.Entity<IdentityUser<System.Guid>>().ToTable("Identity.Users");
            builder.Entity<IdentityUserClaim<System.Guid>>().ToTable("Identity.UserClaims");
            builder.Entity<IdentityUserLogin<System.Guid>>().ToTable("Identity.UserLogins");
            builder.Entity<IdentityUserRole<System.Guid>>().ToTable("Identity.UserRoles");
            builder.Entity<IdentityUserToken<System.Guid>>().ToTable("Identity.UserTokens");
        }
    }
}