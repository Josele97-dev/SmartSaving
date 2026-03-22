namespace SmartSaving.Repositories
{
    public interface ITransactionRepository
    {
        Task<List<Transaction>> GetAllByUserAsync(int userId);
        Task<List<Transaction>> GetByTypeAsync(int userId, TransactionType type);
        Task<Transaction?> GetByIdAsync(int id);
        Task AddAsync(Transaction transaction);
        Task UpdateAsync(Transaction transaction);
        Task DeleteAsync(int id);
    }
}