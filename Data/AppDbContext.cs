using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SmartSaving.Models;

namespace SmartSaving.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // TODO: Move this connection string to a config file for production
            string connectionString = "server=localhost;port=3306;database=smartsaving;user=root;password=admin123";
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Convert TransactionType enum to/from the MySQL enum strings ('income'/'expense')
            var transactionTypeConverter = new ValueConverter<TransactionType, string>(
                v => v == TransactionType.Income ? "income" : "expense",
                v => v == "income" ? TransactionType.Income : TransactionType.Expense
            );

            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(c => c.Type)
                      .HasConversion(transactionTypeConverter);

                entity.HasOne(c => c.Account)
                      .WithMany(a => a.Categories)
                      .HasForeignKey(c => c.AccountId);
            });

            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasOne(a => a.User)
                      .WithMany(u => u.Accounts)
                      .HasForeignKey(a => a.UserId);
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasOne(t => t.Account)
                      .WithMany(a => a.Transactions)
                      .HasForeignKey(t => t.AccountId);

                entity.HasOne(t => t.Category)
                      .WithMany(c => c.Transactions)
                      .HasForeignKey(t => t.CategoryId);
            });
        }
    }
}
