using System;
using Microsoft.EntityFrameworkCore;

namespace Booking.Data.Seeding
{
    public static class DbSetExtensions
    {
        public static void Seed<T>(this DbContext context, IServiceProvider services) where T : IEntitySeeder
        {
            var instance = (IEntitySeeder)Activator.CreateInstance(typeof(T));

            instance.Seed(context, services);
        }
    }
}