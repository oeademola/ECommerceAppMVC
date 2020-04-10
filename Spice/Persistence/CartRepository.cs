using Microsoft.EntityFrameworkCore;
using Spice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Spice.Persistence
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext context;

        public CartRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<ShoppingCart>> GetCartsAsync(string id)
        {
            return await context.ShoppingCarts.Where(u => u.ApplicationUserId == id).ToListAsync();
        }

        public async Task<IEnumerable<ShoppingCart>> GetCartsById(int id)
        {
            return await context.ShoppingCarts.Where(c => c.Id == id).ToListAsync();
        }

        public async Task<ShoppingCart> GetCartAsync(int id)
        {
            return await context.ShoppingCarts.SingleOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<ShoppingCart>> GetCartByUserId(Claim claim)
        {
            return await context.ShoppingCarts.Where(c => c.ApplicationUserId == claim.Value).ToListAsync();
        }

        public async Task<ShoppingCart> GetCartByMenuItemId(string userId, int menuItemId)
        {
            return await context.ShoppingCarts.
                Where(c => c.ApplicationUserId == userId
                && c.MenuItemId == menuItemId).FirstOrDefaultAsync();
        }
        public void Add(ShoppingCart cart)
        {
            context.Add(cart);
        }

        public void Remove(ShoppingCart cart)
        {
            context.Remove(cart);
        }
    }
}
