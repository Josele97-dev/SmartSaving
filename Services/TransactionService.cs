using SmartSaving.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartSaving.Services
{
    public class TransactionService
    {
        private readonly ITransactionRepository _repository;

        public TransactionService(ITransactionRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task AddTransactionAsync(Transaction transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));
            if (string.IsNullOrWhiteSpace(transaction.Description))
                throw new ArgumentException("Es necesaria una descripción");
            if (transaction.Amount == 0)
                throw new ArgumentException("La cantidad no puede ser 0");
            if (transaction.Date > DateTime.Now)
                throw new ArgumentException("La fecha no puede ser futura");
            await _repository.AddAsync(transaction);
        }

        public async Task<bool> DeleteTransactionAsync(int id)
        {
            var transaction = await _repository.GetByIdAsync(id);
            if (transaction == null)
                return false;
            await _repository.DeleteAsync(id);
            return true;
        }

        public async Task UpdateTransactionAsync(Transaction updatedTransaction)
        {
            if (updatedTransaction == null)
                throw new ArgumentNullException(nameof(updatedTransaction));
            if (string.IsNullOrWhiteSpace(updatedTransaction.Description))
                throw new ArgumentException("Es necesaria una descripción");
            if (updatedTransaction.Amount == 0)
                throw new ArgumentException("La cantidad no puede ser 0");
            if (updatedTransaction.Date > DateTime.Now)
                throw new ArgumentException("La fecha no puede ser futura");
            var existing = await _repository.GetByIdAsync(updatedTransaction.Id);
            if (existing == null)
                throw new InvalidOperationException("Transacción no encontrada");
            await _repository.UpdateAsync(updatedTransaction);
        }

        public async Task<List<Transaction>> GetAllTransactionsAsync(int userId)
        {
            return await _repository.GetAllByUserAsync(userId);
        }

        public async Task<List<Transaction>> GetIncomesAsync(int userId)
        {
            return await _repository.GetByTypeAsync(userId, TransactionType.Income);
        }

        public async Task<List<Transaction>> GetExpensesAsync(int userId)
        {
            return await _repository.GetByTypeAsync(userId, TransactionType.Expense);
        }

        public async Task<decimal> GetBalanceAsync(int userId)
        {
            var transactions = await _repository.GetAllByUserAsync(userId);
            decimal balance = 0;
            foreach (var t in transactions)
            {
                if (t.Type == TransactionType.Income)
                    balance += t.Amount;
                else
                    balance -= t.Amount;
            }
            return balance;
        }
    }
}