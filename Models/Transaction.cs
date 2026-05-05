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
        [Column("transaction_id")]
        public int Id { get; set; }

        [Column("account_id")]
        public int AccountId { get; set; }

        [Column("category_id")]
        public int CategoryId { get; set; }

        [Column("amount")]
        public decimal Amount { get; set; }

        [Column("date")]
        public DateTime Date { get; set; } = DateTime.Now;

        [Column("description")]
        public string Description { get; set; } = string.Empty;

        [Column("is_read")]
        public bool IsRead { get; set; } = true;


        
        public Account Account { get; set; } = null!;
        public Category Category { get; set; } = null!;

        
        [NotMapped]
        public TransactionType Type => Category?.Type ?? TransactionType.Expense;
    }
}