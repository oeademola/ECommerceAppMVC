using Microsoft.EntityFrameworkCore;
using Spice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Persistence
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext context;

        public CategoryRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            return await context.Categories.ToListAsync();
        }

        public IEnumerable<Category> GetCategories()
        {
            return context.Categories.ToList();
        }

        public async Task<Category> GetCategoryAsync(int? id)
        {
            return await context.Categories.FindAsync(id);
        }

        public void Add(Category category)
        {
            context.Categories.Add(category);
        }

        public void Remove(Category category)
        {
            context.Remove(category);
        }

    }
}
