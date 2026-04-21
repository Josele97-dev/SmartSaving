using System.Collections.Generic;
using System.Threading.Tasks;
using SmartSaving.Models;

namespace SmartSaving.Services
{
    public interface ITransactionService
    {
        Task<List<Transaction>> GetTransactionsAsync(int userId);
        Task<List<Transaction>> GetByTypeAsync(int userId, TransactionType type);
        Task<bool> AddTransactionAsync(int userId, Transaction transaction);
        Task<bool> UpdateTransactionAsync(Transaction transaction);
        Task<bool> DeleteTransactionAsync(int id);
        Task TransferAsync(User sender, User recipient, decimal amount, string description);

    }
}