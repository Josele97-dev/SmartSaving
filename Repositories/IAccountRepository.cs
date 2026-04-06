using System.Collections.Generic;
using System.Threading.Tasks;
using SmartSaving.Models;

namespace SmartSaving.Repositories
{
    public interface IAccountRepository
    {
        Task<Account?> GetByIdAsync(int id);
        Task<Account?> GetDefaultByUserIdAsync(int userId);
        Task<List<Account>> GetByUserIdAsync(int userId);
        Task<bool> AddAsync(Account account);
        Task<bool> UpdateBalanceAsync(int accountId, decimal newBalance);
    }
}
