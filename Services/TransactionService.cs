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

        public async Task TransferAsync(User sender, User recipient, decimal amount, string description)
        {
            var senderAccount = await _accountRepository.GetDefaultByUserIdAsync(sender.Id);
            var recipientAccount = await _accountRepository.GetDefaultByUserIdAsync(recipient.Id);

            if (senderAccount == null || recipientAccount == null)
                throw new InvalidOperationException("Could not find accounts for the transfer.");

            if (amount <= 0)
                throw new ArgumentException("Transfer amount must be greater than zero.");

            var senderTransactions = await _transactionRepository.GetByAccountIdAsync(senderAccount.Id);
            var realBalance = senderTransactions
                .Where(t => t.Type == TransactionType.Income)
                .Sum(t => t.Amount)
                - senderTransactions
                .Where(t => t.Type == TransactionType.Expense)
                .Sum(t => t.Amount);

            if (realBalance < amount)
                throw new InvalidOperationException("Insufficient balance to complete this transfer.");
           
            
            // Get or create Transfer Out category for sender
            var senderCategories = await _categoryRepository.GetAllByAccountIdAsync(senderAccount.Id);
            var transferOutCategory = senderCategories.FirstOrDefault(c => c.Title == "Transfer Out");
            if (transferOutCategory == null)
            {
                transferOutCategory = new Category
                {
                    AccountId = senderAccount.Id,
                    Title = "Transfer Out",
                    Type = TransactionType.Expense
                };
                await _categoryRepository.AddAsync(transferOutCategory);
            }

            // Get or create Transfer In category for recipient
            var recipientCategories = await _categoryRepository.GetAllByAccountIdAsync(recipientAccount.Id);
            var transferInCategory = recipientCategories.FirstOrDefault(c => c.Title == "Transfer In");
            if (transferInCategory == null)
            {
                transferInCategory = new Category
                {
                    AccountId = recipientAccount.Id,
                    Title = "Transfer In",
                    Type = TransactionType.Income
                };
                await _categoryRepository.AddAsync(transferInCategory);
            }

            // Create expense transaction on sender's account
            var outTransaction = new Transaction
            {
                AccountId = senderAccount.Id,
                CategoryId = transferOutCategory.Id,
                Amount = amount,
                Date = DateTime.UtcNow,
                Description = string.IsNullOrWhiteSpace(description)
                    ? $"Transfer to {recipient.FirstName} {recipient.LastName}"
                    : description
            };

            // Create income transaction on recipient's account
            var inTransaction = new Transaction
            {
                AccountId = recipientAccount.Id,
                CategoryId = transferInCategory.Id,
                Amount = amount,
                Date = DateTime.UtcNow,
                Description = string.IsNullOrWhiteSpace(description)
                    ? $"Transfer from {sender.FirstName} {sender.LastName}"
                    : description
            };

            await _transactionRepository.AddAsync(outTransaction);
            await _transactionRepository.AddAsync(inTransaction);
        }


    }
}