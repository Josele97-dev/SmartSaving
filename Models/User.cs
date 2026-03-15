using System;
using System.Collections.Generic;
using System.Linq;

public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string InitialBalance { get; set; }

    public List<Transaction> Transactions { get; set; } = new List<Transaction>();

    public List<Transaction> GetIncomes()
    {
        return Transactions
            .Where(m => m.Type == TransactionType.Income)
            .ToList();
    }

    public List<Transaction> GetExpenses()
    {
        return Transactions
            .Where(m => m.Type == TransactionType.Expense)
            .ToList();
    }

    public List<Transaction> GetAllTransactions()
    {
        return Transactions.ToList();
    }

    public decimal GetBalance()
    {
        return Transactions.Sum(m => m.Type == TransactionType.Income ? m.Amount : -m.Amount);
    }
}
