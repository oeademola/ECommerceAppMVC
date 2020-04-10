using Spice.Models;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Spice.Persistence
{
    public interface ICartRepository
    {
        Task<ShoppingCart> GetCartAsync(int id);
        Task<IEnumerable<ShoppingCart>> GetCartsById(int id);
        Task<List<ShoppingCart>> GetCartByUserId(Claim claim);
        Task<IEnumerable<ShoppingCart>> GetCartsAsync(string id);
        Task<ShoppingCart> GetCartByMenuItemId(string userId, int menuItemId);
        void Add(ShoppingCart cart);
        void Remove(ShoppingCart cart);
    }
}