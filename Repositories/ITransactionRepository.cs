using System.Collections.Generic;
using System.Threading.Tasks;
using SmartSaving.Models;

namespace SmartSaving.Repositories
{
    public interface ITransactionRepository
    {
        Task<List<Transaction>> GetAllByAccountAsync(int accountId);
        Task<List<Transaction>> GetByTypeAsync(int accountId, TransactionType type);
        Task<Transaction?> GetByIdAsync(int id);
        Task<bool> AddAsync(Transaction transaction);
        Task<bool> UpdateAsync(Transaction transaction);
        Task<bool> DeleteAsync(int id);
        Task<List<Transaction>> GetByMonthAsync(int categoryId, int month, int year);
        Task<List<Transaction>> SearchAsync(int userId, string? keyword, bool searchContains, int? categoryId, TransactionType? type, DateTime? from, DateTime? to);
        Task<List<Transaction>> GetByAccountIdAsync(int accountId);

    }
}
