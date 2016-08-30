using System.Reflection;
using Booking.Data.Mapping;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace Booking.Business
{
    public class BookingContext : IdentityDbContext<IdentityUser<System.Guid>, IdentityRole<System.Guid>, System.Guid>
    {
        public BookingContext(DbContextOptions<BookingContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.HasDefaultSchema("booking");
            builder.HasPostgresExtension("uuid-ossp");

            var types = typeof(BookingContext).GetTypeInfo().Assembly.GetTypes();
            foreach (var type in types)
            {
                if (!typeof(IEntityMappingConfiguration).IsAssignableFrom(type))
                    continue;

                var mapping = System.Activator.CreateInstance(type);
                ((IEntityMappingConfiguration)mapping).Map(builder);
            }
        }
    }
}