using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SmartSaving.Data;
using SmartSaving.Models;

namespace SmartSaving.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        public async Task<List<Transaction>> GetAllByAccountAsync(int accountId)
        {
            using (var context = new AppDbContext())
            {
                return await context.Transactions
                    .Include(t => t.Category)
                    .Where(t => t.AccountId == accountId)
                    .OrderByDescending(t => t.Date)
                    .ToListAsync();
            }
        }

        public async Task<List<Transaction>> GetByTypeAsync(int accountId, TransactionType type)
        {
            using (var context = new AppDbContext())
            {
                return await context.Transactions
                    .Include(t => t.Category)
                    .Where(t => t.AccountId == accountId && t.Category.Type == type)
                    .OrderByDescending(t => t.Date)
                    .ToListAsync();
            }
        }

        public async Task<Transaction?> GetByIdAsync(int id)
        {
            using (var context = new AppDbContext())
            {
                return await context.Transactions
                    .Include(t => t.Category)
                    .FirstOrDefaultAsync(t => t.Id == id);
            }
        }

        public async Task<bool> AddAsync(Transaction transaction)
        {
            using (var context = new AppDbContext())
            {
                context.Transactions.Add(transaction);
                return await context.SaveChangesAsync() > 0;
            }
        }

        public async Task<bool> UpdateAsync(Transaction transaction)
        {
            using (var context = new AppDbContext())
            {
                var existing = await context.Transactions.FindAsync(transaction.Id);
                if (existing == null)
                    return false;

                existing.Amount = transaction.Amount;
                existing.Description = transaction.Description;
                existing.Date = transaction.Date;
                existing.CategoryId = transaction.CategoryId;

                return await context.SaveChangesAsync() > 0;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using (var context = new AppDbContext())
            {
                var transaction = await context.Transactions.FindAsync(id);
                if (transaction == null)
                    return false;

                context.Transactions.Remove(transaction);
                return await context.SaveChangesAsync() > 0;
            }
        }
        public async Task<List<Transaction>> GetByMonthAsync(int categoryId, int month, int year)
        {
            using (var context = new AppDbContext())
            {
                return await context.Transactions
                    .Where(t => t.CategoryId == categoryId
                             && t.Date.Month == month
                             && t.Date.Year == year)
                    .ToListAsync();
            }
        }
    }
}
