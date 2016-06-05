using System;
using Microsoft.EntityFrameworkCore;

namespace Booking.Data.Mapping
{
    public static class ModelBuilderExtensions
    {
        public static void Configure<T>(this ModelBuilder builder) where T : IEntityMappingConfiguration
        {
             var instance = (IEntityMappingConfiguration)Activator.CreateInstance(typeof(T));
             
             instance.Map(builder);
        }
    }
}