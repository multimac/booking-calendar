using System.Reflection;
using System.Threading.Tasks;
using Booking.Data.Mapping;
using Booking.Data.Seeding;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace Booking.Business
{
    public class BookingContext : IdentityDbContext<IdentityUser<System.Guid>, IdentityRole<System.Guid>, System.Guid>
    {
        public DbSet<Models.OAuth.Application> Applications { get; set; }

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

        public async Task EnsureSeedData(System.IServiceProvider services)
        {
            var types = typeof(BookingContext).GetTypeInfo().Assembly.GetTypes();
            foreach (var type in types)
            {
                if (!typeof(IEntitySeeder).IsAssignableFrom(type))
                    continue;

                var seeder = System.Activator.CreateInstance(type);
                await ((IEntitySeeder)seeder).Seed(this, services);
            }
        }
    }
}