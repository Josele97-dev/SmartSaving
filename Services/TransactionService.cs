using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SmartSaving.Models;
using SmartSaving.Repositories;

namespace SmartSaving.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ICategoryRepository _categoryRepository;

        public TransactionService(ITransactionRepository transactionRepository, IAccountRepository accountRepository, ICategoryRepository categoryRepository)
        {
            _transactionRepository = transactionRepository;
            _accountRepository = accountRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<List<Transaction>> GetTransactionsAsync(int userId)
        {
            var account = await GetDefaultAccountAsync(userId);
            return await _transactionRepository.GetAllByAccountAsync(account.Id);
        }

        public async Task<List<Transaction>> GetByTypeAsync(int userId, TransactionType type)
        {
            var account = await GetDefaultAccountAsync(userId);
            return await _transactionRepository.GetByTypeAsync(account.Id, type);
        }

        public async Task<bool> AddTransactionAsync(int userId, Transaction transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            if (string.IsNullOrWhiteSpace(transaction.Description))
                throw new ArgumentException("Description is required.");

            if (transaction.Amount <= 0)
                throw new ArgumentException("Amount must be greater than 0.");

            if (transaction.Date > DateTime.UtcNow)
                throw new ArgumentException("Date cannot be in the future.");

            var account = await GetDefaultAccountAsync(userId);
            transaction.AccountId = account.Id;

           

            return await _transactionRepository.AddAsync(transaction);
        }

        public async Task<bool> UpdateTransactionAsync(Transaction transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            return await _transactionRepository.UpdateAsync(transaction);
        }

        public async Task<bool> DeleteTransactionAsync(int id)
        {
            return await _transactionRepository.DeleteAsync(id);
        }

        private async Task<Account> GetDefaultAccountAsync(int userId)
        {
            var account = await _accountRepository.GetDefaultByUserIdAsync(userId);

            if (account == null)
                throw new InvalidOperationException($"No account found for user {userId}.");

            return account;
        }
       

    }
}