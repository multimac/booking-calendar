using System;
using System.Threading;
using System.Threading.Tasks;
using Booking.Business.Models.Identity;
using Microsoft.AspNetCore.Identity;

namespace Booking.Api.Identity
{
    public class UserStore : IUserStore<User>,
                                IUserEmailStore<User>,
                                IUserPasswordStore<User>,
                                IUserSecurityStampStore<User>
    {
        private ILookupNormalizer LookupNormalizer { get; } = null;

        public UserStore(ILookupNormalizer lookupNormalizer)
        {
            this.LookupNormalizer = lookupNormalizer;
        }
        public void Dispose() { }
        
        public Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        public Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        public Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        
        public Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        public Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        
        public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
        {
            return GetEmailAsync(user, cancellationToken);
        }
        public Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return GetNormalizedUserNameAsync(user, cancellationToken);
        }
        public Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult<string>(user.Username);
        }
        public Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
        {
            return SetNormalizedUserNameAsync(
                user, LookupNormalizer.Normalize(userName), cancellationToken
            );
        }
        public Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
        {
            user.Username = normalizedName;
            return Task.FromResult<object>(null);
        }


        public Task<User> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        
        public Task<string> GetEmailAsync(User user, CancellationToken cancellationToken)
        {
            return GetNormalizedEmailAsync(user, cancellationToken);
        }
        public Task<bool> GetEmailConfirmedAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult<bool>(user.EmailConfirmed);
        }
        public Task<string> GetNormalizedEmailAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult<string>(user.Email);
        }
        public Task SetEmailAsync(User user, string email, CancellationToken cancellationToken)
        {
            return SetNormalizedEmailAsync(
                user, LookupNormalizer.Normalize(email), cancellationToken
            );
        }
        public Task SetEmailConfirmedAsync(User user, bool confirmed, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        public Task SetNormalizedEmailAsync(User user, string normalizedEmail, CancellationToken cancellationToken)
        {
            user.Email = normalizedEmail;
            return Task.FromResult<object>(null);
        }


        public Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash != null);
        }
        
        public Task<string> GetPasswordHashAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult<string>(user.PasswordHash);
        }
        public Task SetPasswordHashAsync(User user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            return Task.FromResult<object>(null);
        }


        public Task<string> GetSecurityStampAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult<string>(user.SecurityStamp);
        }
        public Task SetSecurityStampAsync(User user, string stamp, CancellationToken cancellationToken)
        {
            user.SecurityStamp = stamp;
            return Task.FromResult<object>(null);
        }
    }
}