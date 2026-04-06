using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SmartSaving.Data;
using SmartSaving.Models;

namespace SmartSaving.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        public async Task<Account?> GetByIdAsync(int id)
        {
            using (var context = new AppDbContext())
            {
                return await context.Accounts
                    .Include(a => a.Categories)
                    .FirstOrDefaultAsync(a => a.Id == id);
            }
        }

        public async Task<Account?> GetDefaultByUserIdAsync(int userId)
        {
            using (var context = new AppDbContext())
            {
                // Returns the first account for this user (the "default" account)
                return await context.Accounts
                    .Include(a => a.Categories)
                    .FirstOrDefaultAsync(a => a.UserId == userId);
            }
        }

        public async Task<List<Account>> GetByUserIdAsync(int userId)
        {
            using (var context = new AppDbContext())
            {
                return await context.Accounts
                    .Include(a => a.Categories)
                    .Where(a => a.UserId == userId)
                    .ToListAsync();
            }
        }

        public async Task<bool> AddAsync(Account account)
        {
            using (var context = new AppDbContext())
            {
                context.Accounts.Add(account);
                return await context.SaveChangesAsync() > 0;
            }
        }

        public async Task<bool> UpdateBalanceAsync(int accountId, decimal newBalance)
        {
            using (var context = new AppDbContext())
            {
                var account = await context.Accounts.FindAsync(accountId);
                if (account == null)
                    return false;

                account.CurrentBalance = newBalance;
                return await context.SaveChangesAsync() > 0;
            }
        }
    }
}
