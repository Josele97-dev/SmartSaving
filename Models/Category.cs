using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartSaving.Models
{
    [Table("category")]
    public class Category
    {
        [Key]
        [Column("category_ID")]
        public int Id { get; set; }

        [Column("account_ID")]
        public int AccountId { get; set; }

        [Column("title")]
        public string Title { get; set; } = string.Empty;

        [Column("transaction_type")]
        public TransactionType Type { get; set; }

        // Navigation properties
        public Account Account { get; set; } = null!;
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}