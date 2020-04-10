using Microsoft.EntityFrameworkCore;
using Spice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Spice.Persistence
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext context;

        public UserRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<ApplicationUser>> GetApplicationUsersAsync(Claim claim)
        {
            return await context.ApplicationUsers.Where(u => u.Id != claim.Value).ToListAsync();
        }

        public async Task<ApplicationUser> GetApplicationUserAsync(string id)
        {
            return await context.ApplicationUsers.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<ApplicationUser> GetApplicationUserById(Claim claim)
        {
            return await context.ApplicationUsers.Where(u => u.Id == claim.Value).FirstOrDefaultAsync();
        }
    }
}
