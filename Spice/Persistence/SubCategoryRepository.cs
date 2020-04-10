using Microsoft.EntityFrameworkCore;
using Spice.Models;
using Spice.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Persistence
{
    public class SubCategoryRepository : ISubCategoryRepository
    {
        private readonly ApplicationDbContext context;

        public SubCategoryRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<SubCategory>> GetSubCategoriesAsync()
        {
            return await context.SubCategories.Include(s => s.Category).ToListAsync();
        }

        public async Task<SubCategory> GetSubCategoryAsync(int? id)
        {
            return await context.SubCategories.SingleOrDefaultAsync(m => m.Id == id);
        }

        public async Task<List<string>> GetSubCategoryList()
        {
            return await context.SubCategories.OrderBy(p => p.Name).Select(p => p.Name).Distinct().ToListAsync();
        }

        public async Task<List<SubCategory>> GetSubCategoriesByCategoryId(int id)
        {
            return await context.SubCategories.Where(s => s.CategoryId == id).ToListAsync();
        }

        public async Task<IEnumerable<SubCategory>> GetCategoryByIdAndName(int id, string name)
        {
            return await  context.SubCategories.Include(s => s.Category)
                .Where(s => s.Category.Id == id && s.Name == name)
                .ToListAsync();
        }

        public void Add(SubCategory subCategory)
        {
            context.SubCategories.Add(subCategory);
        }

        public void Remove(SubCategory subCategory)
        {
            context.Remove(subCategory);
        }
    }
}
