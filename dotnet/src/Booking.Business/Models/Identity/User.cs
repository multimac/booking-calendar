namespace Booking.Business.Models.Identity
{
    public class User
    {
        public int Id { get; set; } = 0;
        
        public string Username { get; set; } = null;
        
        public string Email { get; set; } = null;
        public bool EmailConfirmed { get; set; } = false;
        
        public string PasswordHash { get; set; } = null;
        public string SecurityStamp { get; set; } = null;
    }
}