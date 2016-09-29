using System;
using System.Threading.Tasks;
using Booking.Data.Mapping;
using Booking.Data.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Business.Models.OAuth
{
    public class Application
    {
        public string Id { get; set; }
        public ApplicationType Type { get; set; }

        public bool RedirectAllowSubdomains { get; set; }
        public bool RedirectAllowSubpaths { get; set; }
        public bool RedirectAllowHttp { get; set; }
        public string RedirectUrl { get; set; }

        public string Secret { get; set; }
        public string Salt { get; set; }
    }

    public class ApplicationMapping : EntityMappingConfiguration<Application>
    {
        public override void Map(EntityTypeBuilder<Application> builder)
        {
            builder.ToTable("OAuth.Applications");

            builder.Property(p => p.Id).IsRequired();
            builder.Property(p => p.Type).IsRequired();

            builder.Property(p => p.RedirectAllowSubdomains).IsRequired();
            builder.Property(p => p.RedirectAllowSubpaths).IsRequired();
            builder.Property(p => p.RedirectAllowHttp).IsRequired();
            builder.Property(p => p.RedirectUrl).IsRequired();
        }
    }

    public class ApplicationSeeder : EntitySeeder<Application, BookingContext>
    {
        private static readonly Application[] Applications = new Application[]
        {
            new Application { Id = "calend.ar", Type = ApplicationType.Public, RedirectUrl = "calend.ar/auth" },
            new Application { Id = "postman", Type = ApplicationType.Public, RedirectUrl = "www.getpostman.com/oauth/callback" }
        };

        public override async Task Seed(DbSet<Application> applications, BookingContext context, IServiceProvider services)
        {
            foreach (var seedApp in Applications)
            {
                if (await applications.AnyAsync((app) => app.Id == seedApp.Id))
                    continue;

                applications.Add(seedApp);
            }

            await context.SaveChangesAsync();
        }

        public override Task SeedDev(DbSet<Application> dbSet, BookingContext context, IServiceProvider services)
            => Task.CompletedTask;
    }
}