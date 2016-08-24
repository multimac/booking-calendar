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
            
        }
    }
}