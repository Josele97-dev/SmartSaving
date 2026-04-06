using System.Reflection;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartSaving.Models
{
    public enum TransactionType
    {
        Income,
        Expense
    }

    [Table("transactions")]
    public class Transaction
    {
        [Key]
        [Column("transaction_ID")]
        public int Id { get; set; }

        [Column("account_ID")]
        public int AccountId { get; set; }

        [Column("category_ID")]
        public int CategoryId { get; set; }

        [Column("amount")]
        public decimal Amount { get; set; }

        [Column("date")]
        public DateTime Date { get; set; } = DateTime.Now;

        [Column("description")]
        public string Description { get; set; } = string.Empty;

        // Navigation properties
        public Account Account { get; set; } = null!;
        public Category Category { get; set; } = null!;

        // Convenience property — type is determined by the Category
        [NotMapped]
        public TransactionType Type => Category?.Type ?? TransactionType.Expense;
    }
}