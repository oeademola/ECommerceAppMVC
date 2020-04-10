using Spice.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Spice.Persistence
{
    public interface IMenuItemRepository
    {
        void Add(MenuItem menuItem);
        Task<MenuItem> GetMenuItemAsync(int? id, bool includeRelated = true);
        Task<IEnumerable<MenuItem>> GetMenuItemsAsync();
        void Remove(MenuItem menuItem);
    }
}