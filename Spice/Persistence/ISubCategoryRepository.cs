using Spice.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Spice.Persistence
{
    public interface ISubCategoryRepository
    {
        void Add(SubCategory subCategory);
        Task<IEnumerable<SubCategory>> GetSubCategoriesAsync();
        Task<List<SubCategory>> GetSubCategoriesByCategoryId(int id);
        Task<SubCategory> GetSubCategoryAsync(int? id);
        Task<List<string>> GetSubCategoryList();
        void Remove(SubCategory subCategory);
        Task<IEnumerable<SubCategory>> GetCategoryByIdAndName(int id, string name);
    }
}