using Booking.Data.Mapping;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Business.Models.Identity
{
    public class User
    {
        public int Id { get; set; }
        
        public string Username { get; set; }
        
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
    }
    public class UserConfiguration : EntityMappingConfiguration<User>
    {
        public override void Map(EntityTypeBuilder<User> builder)
        {
            
        }
    }
}