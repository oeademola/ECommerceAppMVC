using Microsoft.EntityFrameworkCore;
using Spice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Persistence
{
    public class MenuItemRepository : IMenuItemRepository
    {
        private readonly ApplicationDbContext context;

        public MenuItemRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<MenuItem>> GetMenuItemsAsync()
        {
            return await context.MenuItems.Include(m => m.Category)
                .Include(m => m.SubCategory).ToListAsync();
        }

        public async Task<MenuItem> GetMenuItemAsync(int? id, bool includeRelated = true)
        {
            if (!includeRelated)
                return await context.MenuItems.SingleOrDefaultAsync(m => m.Id == id);

            return await context.MenuItems.Include(m => m.Category)
                .Include(m => m.SubCategory).SingleOrDefaultAsync(m => m.Id == id);
        }

        public void Add(MenuItem menuItem)
        {
            context.MenuItems.Add(menuItem);
        }

        public void Remove(MenuItem menuItem)
        {
            context.Remove(menuItem);
        }
    }
}
