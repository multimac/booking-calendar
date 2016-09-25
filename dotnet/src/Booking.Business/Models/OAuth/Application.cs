using System;
using System.Threading.Tasks;
using Booking.Data.Mapping;
using Booking.Data.Seeding;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Business.Models.OAuth
{
    public class Application
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public ApplicationType? Type { get; set; }

        public string RedirectUrl { get; set; }
        public string Secret { get; set; }
    }

    public class ApplicationMapping : EntityMappingConfiguration<Application>
    {
        public override void Map(EntityTypeBuilder<Application> builder)
        {
            builder.ToTable("OAuth.Applications");

            builder.Property(p => p.Id).ValueGeneratedOnAdd();

            builder.Property(p => p.Name).IsRequired();
            builder.Property(p => p.Type).IsRequired();
            builder.Property(p => p.RedirectUrl).IsRequired();
        }
    }

    public class ApplicationSeeder : EntitySeeder<Application, BookingContext>
    {
        public override async Task Seed(DbSet<Application> dbSet, BookingContext context, IServiceProvider services)
        {
            dbSet.Add(new Application
            {
                Name = "Calend.ar",
                Type = ApplicationType.Public,

                RedirectUrl = "http://*.calend.ar/auth",
                Secret = null
            });

            await context.SaveChangesAsync();
        }

        public override Task SeedDev(DbSet<Application> dbSet, BookingContext context, IServiceProvider services)
            => Task.CompletedTask;
    }
}