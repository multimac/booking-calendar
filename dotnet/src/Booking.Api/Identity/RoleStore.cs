using System;
using System.Threading;
using System.Threading.Tasks;
using Booking.Business.Models.Identity;
using Microsoft.AspNetCore.Identity;

namespace Booking.Api
{
    public class RoleStore : IRoleStore<Role>
    {
        private ILookupNormalizer LookupNormalizer { get; } = null;

        public RoleStore(ILookupNormalizer lookupNormalizer)
        {
            this.LookupNormalizer = lookupNormalizer;
        }
        public void Dispose() { }
        
        public Task<IdentityResult> CreateAsync(Role role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        public Task<IdentityResult> UpdateAsync(Role role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        public Task<IdentityResult> DeleteAsync(Role role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Role> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        public Task<Role> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetRoleIdAsync(Role role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        public Task<string> GetRoleNameAsync(Role role, CancellationToken cancellationToken)
        {
            return Task.FromResult<string>(role.Name);
        }
        public Task<string> GetNormalizedRoleNameAsync(Role role, CancellationToken cancellationToken)
        {
            return Task.FromResult<string>(
                LookupNormalizer.Normalize(role.Name)
            );
        }
        public Task SetRoleNameAsync(Role role, string roleName, CancellationToken cancellationToken)
        {
            role.Name = roleName;
            return Task.FromResult<object>(null);
        }
        public Task SetNormalizedRoleNameAsync(Role role, string normalizedName, CancellationToken cancellationToken)
        {
            return Task.FromResult<object>(null);
        }
    }
}