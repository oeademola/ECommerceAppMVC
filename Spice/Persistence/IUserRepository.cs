using Spice.Models;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Spice.Persistence
{
    public interface IUserRepository
    {
        Task<ApplicationUser> GetApplicationUserAsync(string id);
        Task<IEnumerable<ApplicationUser>> GetApplicationUsersAsync(Claim claim);
        Task<ApplicationUser> GetApplicationUserById(Claim claim);
    }
}