using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Booking.Common.Mvc.Identity
{
    public class PasswordHasher : IPasswordHasher, IPasswordHasher<IdentityUser<Guid>>
    {
        public Options.PasswordHasherOptions Options { get; set; } = null;

        public PasswordHasher(IOptions<Options.PasswordHasherOptions> optionsAccessor)
        {
            this.Options = optionsAccessor.Value;
        }

        public string HashPassword(string password)
        {
            if (password == null)
                throw new ArgumentNullException(nameof(password));

            return BCrypt.Net.BCrypt.HashPassword(password, Options.WorkFactor);
        }
        public string HashPassword(IdentityUser<Guid> user, string password)
        {
            return HashPassword(password);
        }

        public bool VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            if (hashedPassword == null)
                throw new ArgumentNullException(nameof(hashedPassword));

            if (providedPassword == null)
                throw new ArgumentNullException(nameof(providedPassword));

            return BCrypt.Net.BCrypt.Verify(providedPassword, hashedPassword);
        }
        public PasswordVerificationResult VerifyHashedPassword(IdentityUser<Guid> user, string hashedPassword, string providedPassword)
        {
            if (!VerifyHashedPassword(hashedPassword, providedPassword))
                return PasswordVerificationResult.Failed;

            return PasswordVerificationResult.Success;
        }
    }
}