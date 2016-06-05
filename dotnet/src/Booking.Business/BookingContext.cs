using Booking.Business.Models.Identity;
using Booking.Data.Mapping;
using Microsoft.EntityFrameworkCore;

namespace Booking.Business
{
    public class BookingContext : DbContext
    {
        public BookingContext(DbContextOptions<BookingContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Configure<RoleConfiguration>();
            builder.Configure<UserConfiguration>();
        }

        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
    }
}