using Spice.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Spice.Persistence
{
    public interface ICategoryRepository
    {
        void Add(Category category);
        Task<IEnumerable<Category>> GetCategoriesAsync();
        IEnumerable<Category> GetCategories();
        Task<Category> GetCategoryAsync(int? id);
        void Remove(Category category);
    }
}