using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SmartSaving.Data;
using SmartSaving.Models;

namespace SmartSaving.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        public async Task<Category?> GetByIdAsync(int id)
        {
            using (var context = new AppDbContext())
            {
                return await context.Categories.FindAsync(id);
            }
        }

        public async Task<bool> UpdateAsync(Category category)
        {
            using (var context = new AppDbContext())
            {
                var existing = await context.Categories.FindAsync(category.Id);
                if (existing == null) return false;

                existing.BudgetLimit = category.BudgetLimit;
                return await context.SaveChangesAsync() > 0;
            }
        }
    }
}
