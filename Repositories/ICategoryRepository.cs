using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartSaving.Models;

namespace SmartSaving.Repositories
{
    public  interface ICategoryRepository
    {
        Task<Category?> GetByIdAsync(int id);
        Task<bool> UpdateAsync(Category category);

        Task<List<Category>> GetByAccountIdAsync(int accountId);

        Task<List<Category>> GetAllByAccountIdAsync(int accountId);
        Task<bool> AddAsync(Category category);
    }
}
