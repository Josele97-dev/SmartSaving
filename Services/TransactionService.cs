using System;
using System.Linq;

namespace SmartSaving.Services
{
    public class TransactionService
    {
        private readonly User _user;

        public TransactionService(User user)
        {
            _user = user ?? throw new ArgumentNullException(nameof(user));
        }

        public void AddTransaction(Transaction Transaction)
        {
            if (Transaction == null)
                throw new ArgumentNullException(nameof(Transaction));

            if (string.IsNullOrWhiteSpace(Transaction.Description))
                throw new ArgumentException("Description is required.");

            if (Transaction.Amount == 0)
                throw new ArgumentException("Amount cannot be 0.");

            if (Transaction.Date > DateTime.Now)
                throw new ArgumentException("Date cannot be in the future.");

            _user.Transactions.Add(Transaction);
        }

        public bool DeleteTransaction(int id)
        {
            var mov = _user.Transactions.FirstOrDefault(m => m.Id == id);

            if (mov == null)
                return false;

            _user.Transactions.Remove(mov);
            return true;
        }

        public void UpdateTransaction(Transaction updatedTransaction)
        {
            if (updatedTransaction == null)
                throw new ArgumentNullException(nameof(updatedTransaction));

            var mov = _user.Transactions
                .FirstOrDefault(m => m.Id == updatedTransaction.Id);

            if (mov == null)
                throw new InvalidOperationException("Transaction not found.");

            if (string.IsNullOrWhiteSpace(updatedTransaction.Description))
                throw new ArgumentException("Description is required.");

            if (updatedTransaction.Amount == 0)
                throw new ArgumentException("Amount cannot be 0.");

            if (updatedTransaction.Date > DateTime.Now)
                throw new ArgumentException("Date cannot be in the future.");

            mov.Description = updatedTransaction.Description;
            mov.Amount = updatedTransaction.Amount;
            mov.Date = updatedTransaction.Date;
            mov.Type = updatedTransaction.Type;
            mov.Category = updatedTransaction.Category;
        }
    }
}