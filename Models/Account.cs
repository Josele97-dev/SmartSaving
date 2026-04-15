using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartSaving.Models
{
    [Table("accounts")]
    public class Account
    {
        [Key]
        [Column("account_id")]
        public int Id { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("account_name")]
        public string AccountName { get; set; } = string.Empty;

        [Column("current_balance")]
        public decimal CurrentBalance { get; set; }

        // Navigation properties
        public User User { get; set; } = null!;
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();
        public List<Category> Categories { get; set; } = new List<Category>();
    }
}
