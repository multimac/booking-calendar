using Booking.Data.Mapping;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Business.Models.Identity
{
    public class Role
    {
        public int Id { get; set; }
        
        public string Name { get; set; }
        public string NormalizedName { get; set; }
    }
    public class RoleConfiguration : EntityMappingConfiguration<Role>
    {
        public override void Map(EntityTypeBuilder<Role> builder)
        {
            
        }
    }
}